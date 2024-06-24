using vesa.Core.Abstractions;

namespace vesa.File.Abstractions;

public interface IFileEventStore : IEventStore
{
    string Path { get; }

    Task<IEvent> GetEventFromFileAsync(string eventFilePath, CancellationToken cancellationToken = default);
}
