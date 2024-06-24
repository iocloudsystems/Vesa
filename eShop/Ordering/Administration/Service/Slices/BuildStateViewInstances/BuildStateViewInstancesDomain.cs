using eShop.Ordering.Administration.Events;
using Microsoft.Extensions.DependencyInjection;
using vesa.Core.Abstractions;
using vesa.Core.Extensions;
using vesa.Core.Helpers;

namespace eShop.Ordering.Administration.Service.BuildStateViewInstances;

public class BuildStateViewInstancesDomain : IDomain<BuildStateViewInstancesCommand>
{
    private readonly IServiceProvider _serviceProvider;

    public BuildStateViewInstancesDomain(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<IEnumerable<IEvent>> ProcessAsync(BuildStateViewInstancesCommand command, CancellationToken cancellationToken = default)
    {
        var stateViewType = TypeHelper.GetType(command.StateViewName);
        Type stateViewBuilderType = typeof(IStateViewBuilder<>);
        Type genericStateViewBuilderType = stateViewBuilderType.MakeGenericType(stateViewType);
        var genericStateViewBuilder = _serviceProvider.GetRequiredService(genericStateViewBuilderType);
        var buildAsyncMethod = genericStateViewBuilderType.GetMethod("BuildStateViewsAsync");
        await buildAsyncMethod.InvokeAsync(genericStateViewBuilder, stateViewType);
        var @event = new StateViewInstanceBuiltEvent(command.StateViewName, null, command.TriggeredBy, command.Id);
        return new IEvent[] { @event };
    }
}
