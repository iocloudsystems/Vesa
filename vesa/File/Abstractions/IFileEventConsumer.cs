using vesa.Core.Abstractions;

namespace vesa.File.Abstractions;

public interface IFileEventConsumer : IEventConsumer
{
    string Path { get; init; }
}

