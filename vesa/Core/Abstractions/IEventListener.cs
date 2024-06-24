namespace vesa.Core.Abstractions;
public interface IEventListener
{
    Task StartAsync(CancellationToken stoppingToken);
    Task StopAsync();

}
