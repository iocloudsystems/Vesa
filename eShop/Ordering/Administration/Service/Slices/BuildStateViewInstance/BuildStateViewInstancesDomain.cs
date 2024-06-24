using eShop.Ordering.Administration.Events;
using Microsoft.Extensions.DependencyInjection;
using vesa.Core.Abstractions;
using vesa.Core.Extensions;
using vesa.Core.Helpers;

namespace eShop.Ordering.Administration.Service.BuildStateViewInstance;

public class BuildStateViewInstanceDomain : IDomain<BuildStateViewInstanceCommand>
{
    private readonly IServiceProvider _serviceProvider;

    public BuildStateViewInstanceDomain(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<IEnumerable<IEvent>> ProcessAsync(BuildStateViewInstanceCommand command, CancellationToken cancellationToken = default)
    {
        var stateViewType = TypeHelper.GetType(command.StateViewName);
        var subject = command.Subject;
        Type stateViewBuilderType = typeof(IStateViewBuilder<>);
        Type genericStateViewBuilderType = stateViewBuilderType.MakeGenericType(stateViewType);
        var genericStateViewBuilder = _serviceProvider.GetRequiredService(genericStateViewBuilderType);
        var buildAsyncMethod = genericStateViewBuilderType.GetMethod("BuildStateViewAsync");
        await buildAsyncMethod.InvokeAsync(genericStateViewBuilder, subject);
        var @event = new StateViewInstancesBuiltEvent(command.StateViewName, command.TriggeredBy, command.Id);
        return new IEvent[] { @event };
    }
}
