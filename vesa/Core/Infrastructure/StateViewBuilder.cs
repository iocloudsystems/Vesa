using vesa.Core.Abstractions;
using vesa.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace vesa.Core.Infrastructure;

public class StateViewBuilder : IStateViewBuilder
{
    private readonly IServiceProvider _serviceProvider;
    protected readonly ILogger<StateViewBuilder> _logger;

    public StateViewBuilder
    (
        IServiceProvider serviceProvider,
        ILogger<StateViewBuilder> logger
    )
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task BuildAllStateViewsAsync()
    {
        var stateViewTypes = typeof(IStateView).GetTypesOf().Where(t => !t.IsAbstract);
        foreach (var stateViewType in stateViewTypes)
        {
            Type stateViewBuilderType = typeof(IStateViewBuilder<>);
            Type genericStateViewBuilderType = stateViewBuilderType.MakeGenericType(stateViewType);
            var genericStateViewBuilder = _serviceProvider.GetRequiredService(genericStateViewBuilderType);
            var buildAsyncMethod = genericStateViewBuilderType.GetMethod(nameof(IStateViewBuilder.BuildAllStateViewsAsync));
            await buildAsyncMethod.InvokeAsync(genericStateViewBuilder, null);
        }
    }
}

public class StateViewBuilder<TStateView> : IStateViewBuilder<TStateView>
    where TStateView : class, IStateView
{
    private readonly IStateViewStore<TStateView> _stateViewStore;
    private readonly IEventStore _eventStore;
    private readonly IEventProcessor _eventProcessor;
    protected readonly ILogger<StateViewBuilder<TStateView>> _logger;

    public StateViewBuilder
    (
        IStateViewStore<TStateView> stateViewStore,
        IEventStore eventStore,
        IEventProcessor eventProcessor,
        ILogger<StateViewBuilder<TStateView>> logger
    )
    {
        _stateViewStore = stateViewStore;
        _eventStore = eventStore;
        _eventProcessor = eventProcessor;
        _logger = logger;
    }


    public async Task BuildStateViewsAsync(Type stateViewType)
    {
        var fullSubject = $"{stateViewType.Name.Replace("StateView", "")}";
        var rawSubjects = await _eventStore.GetSubjectsAsync();

        var subjects = rawSubjects.Where(subject =>
        {
            var splitSubject = subject.Split("\\");
            var lastSegment = splitSubject[splitSubject.Length - 1];
            var underscorePointer = lastSegment.IndexOf("_");
            if (underscorePointer > -1)
            {
                lastSegment = lastSegment.Substring(0, underscorePointer);
            }
            return lastSegment.Equals(fullSubject);
        });

        foreach (var subject in subjects)
        {
            var coreSubject = subject.Split(@"\").Last();   // Remove any paths if exists
            await BuildStateViewAsync(coreSubject);
        }
    }

    public async Task BuildStateViewAsync(string subject)
    {
        await _stateViewStore.DeleteStateViewAsync(subject);
        var events = await _eventStore.GetEventsAsync(subject);
        await ProcessAsync(events);
    }

    protected async Task ProcessAsync(IEnumerable<IEvent> events)
    {
        foreach (var @event in events)
        {
            await _eventProcessor.ProcessAsync(@event);
        }
    }
}
