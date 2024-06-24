using vesa.Core.Infrastructure;
using eShop.Ordering.Inquiry.Worker.Extensions;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.Configure(hostContext.Configuration);
        services.AddHostedService<EventListenerWorker>();
    })
    .Build();

await host.RunAsync();
