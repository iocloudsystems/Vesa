using vesa.Core.Abstractions;
using vesa.Core.Infrastructure;

namespace eShop.Ordering.Administration.Service.BuildStateViewInstances;

public class BuildStateViewInstancesHandler : CommandHandler<BuildStateViewInstancesCommand>
{
    public BuildStateViewInstancesHandler
    (
        IServiceProvider serviceProvider,
        IDomain<BuildStateViewInstancesCommand> domain,
        IEventStore eventStore
    )
        : base(serviceProvider, domain, eventStore)
    {
    }
}
