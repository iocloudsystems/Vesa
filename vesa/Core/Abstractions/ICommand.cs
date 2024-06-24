namespace vesa.Core.Abstractions;

public interface ICommand
{
    string Id { get; }
    DateTimeOffset CommandDate { get; }
    string TriggeredBy { get; }
    int LastEventSequenceNumber { get; }
    string Version { get; }
}
