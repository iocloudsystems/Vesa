using vesa.Core.Abstractions;
using vesa.Core.Infrastructure;

namespace eShop.Ordering.Administration.Service.BuildAllStateViews;

public class BuildAllStateViewsHandler : CommandHandler<BuildAllStateViewsCommand>
{
    public BuildAllStateViewsHandler
    (
        IServiceProvider serviceProvider,
        IDomain<BuildAllStateViewsCommand> domain,
        IEventStore eventStore
    )
        : base(serviceProvider, domain, eventStore)
    {
    }
}
