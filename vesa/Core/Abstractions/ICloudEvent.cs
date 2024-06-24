namespace vesa.Core.Abstractions;

public interface ICloudEvent
{
    string Id { get; }
    string Source { get; init; }
    string SpecVersion { get; init; }
    string Type { get; init; }
    string Subject { get; init; }
    DateTimeOffset Time { get; init; }
    string? DataSchema { get; init; }
}

public interface ICloudEvent<TEventData> : ICloudEvent
    where TEventData : IEvent
{
    TEventData Data { get; init; }
}
