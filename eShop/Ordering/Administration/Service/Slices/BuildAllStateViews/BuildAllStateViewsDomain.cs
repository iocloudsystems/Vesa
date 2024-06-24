using eShop.Ordering.Administration.Events;
using Microsoft.Extensions.DependencyInjection;
using vesa.Core.Abstractions;

namespace eShop.Ordering.Administration.Service.BuildAllStateViews;

public class BuildAllStateViewsDomain : IDomain<BuildAllStateViewsCommand>
{
    private readonly IServiceProvider _serviceProvider;

    public BuildAllStateViewsDomain(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<IEnumerable<IEvent>> ProcessAsync(BuildAllStateViewsCommand command, CancellationToken cancellationToken = default)
    {
        var stateViewBuilder = _serviceProvider.GetRequiredService<IStateViewBuilder>();
        await stateViewBuilder.BuildAllStateViewsAsync();
        var @event = new AllStateViewsBuiltEvent(command.TriggeredBy, command.Id);
        return new IEvent[] { @event };
    }
}
