using vesa.Core.Abstractions;
using vesa.Core.Infrastructure;

namespace eShop.Ordering.Administration.Service.BuildStateViewInstance;

public class BuildStateViewInstanceHandler : CommandHandler<BuildStateViewInstanceCommand>
{
    public BuildStateViewInstanceHandler
    (
        IServiceProvider serviceProvider,
        IDomain<BuildStateViewInstanceCommand> domain,
        IEventStore eventStore
    )
        : base(serviceProvider, domain, eventStore)
    {
    }
}
