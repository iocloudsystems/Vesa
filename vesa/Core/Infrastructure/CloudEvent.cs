using vesa.Core.Abstractions;

namespace vesa.Core.Infrastructure;

public class CloudEvent<TEventData> : CloudEvent, ICloudEvent<TEventData>
    where TEventData : IEvent
{
    public CloudEvent
    (
        string id,
        string source,
        string type,
        string subject,
        DateTimeOffset time,
        TEventData data,
        string specVersion = "1.0",
        string? dataSchema = null
    )
        : base(id, source, type, subject, time, specVersion, dataSchema)
    {
        Data = data;
    }

    public CloudEvent
    (
        string source,
        string type,
        string subject,
        DateTimeOffset time,
        TEventData data,
        string specVersion = "1.0",
        string? dataSchema = null
    )
        : base(source, type, subject, time, specVersion, dataSchema)
    {
        Data = data;
    }

    public TEventData Data { get; init; }
}

public abstract class CloudEvent
{
    protected CloudEvent
    (
        string id,
        string source,
        string type,
        string subject,
        DateTimeOffset time,
        string specVersion = "1.0",
        string? dataSchema = null
    )
    {
        Id = id;
        Source = source;
        Type = type;
        Subject = subject;
        Time = time;
        SpecVersion = specVersion;
        DataSchema = dataSchema;
    }

    protected CloudEvent
    (
        string source,
        string type,
        string subject,
        DateTimeOffset time,
        string specVersion = "1.0",
        string? dataSchema = null
    )
    {
        Source = source;
        Type = type;
        Subject = subject;
        Time = time;
        SpecVersion = specVersion;
        DataSchema = dataSchema;
    }

    public virtual string Id { get; init; } = Guid.NewGuid().ToString();
    public string Source { get; init; }
    public string SpecVersion { get; init; } = "1.0";
    public string Type { get; init; }
    public virtual string Subject { get; init; }
    public DateTimeOffset Time { get; init; }
    public string? DataSchema { get; init; }
}
