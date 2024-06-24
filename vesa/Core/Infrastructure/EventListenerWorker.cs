using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using vesa.Core.Abstractions;

namespace vesa.Core.Infrastructure;

public class EventListenerWorker : BackgroundService
{
    private IEnumerable<IEventListener> _eventListeners;
    private readonly ILogger<EventListenerWorker> _logger;

    public EventListenerWorker(IEnumerable<IEventListener> eventListeners, ILogger<EventListenerWorker> logger)
    {
        _eventListeners = eventListeners;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var tasks = new List<Task>();
            foreach (var listener in _eventListeners)
            {
                tasks.Add(listener.StartAsync(stoppingToken));
            }
            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Consume Scoped Service Hosted Service is stopping.");
        foreach (var listener in _eventListeners)
        {
            listener.StopAsync();
        }
        await base.StopAsync(stoppingToken);
    }
}