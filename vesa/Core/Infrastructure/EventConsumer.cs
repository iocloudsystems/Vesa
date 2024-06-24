using vesa.Core.Abstractions;
using vesa.Core.Constants;
using vesa.Core.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace vesa.Core.Infrastructure;

public abstract class EventConsumer : IEventConsumer
{
    protected IDictionary<string, Type> _eventMappings = new Dictionary<string, Type>();

    protected EventConsumer(IEnumerable<IEventMapping> eventMappings)
    {
        foreach (var eventMapping in eventMappings)
        {
            MapEventType(eventMapping.SourceType, eventMapping.TargetType);
        }
    }

    public abstract Task<IEnumerable<IEvent>> ConsumeEventsAsync(CancellationToken cancellationToken);

    public void MapEventType(string sourceType, string targetType)
    {
        _eventMappings.Add(sourceType, TypeHelper.GetType(targetType));
    }

    protected IEvent HydrateEvent(string eventJson)
    {
        var eventTypeName = "";
        var jevent = JsonConvert.DeserializeObject<JObject>(eventJson);
        if (jevent.ContainsKey(JsonPropertyName.EventTypeName))
        {
            eventTypeName = (string)jevent[JsonPropertyName.EventTypeName];
        }
        else if (jevent.ContainsKey(JsonPropertyName.Type)) // CloudEvent format
        {
            eventTypeName = jevent[JsonPropertyName.Type].ToString();
            jevent = (JObject)jevent[JsonPropertyName.Data];
            eventJson = jevent.ToString();
        }
        else
        {
            throw new ArgumentException("Unknown event message format");
        }
        return HydrateEvent(eventJson, eventTypeName);
    }

    protected IEvent HydrateEvent(string eventJson, string eventTypeName)
    {
        Type eventType = default;
        if (_eventMappings.ContainsKey(eventTypeName))
        {
            eventType = _eventMappings[eventTypeName];
        }
        else
        {
            eventType = TypeHelper.GetType(eventTypeName);
        }
        if (eventType == null)
        {
            throw new Exception("Event type not found");
        }
        return (IEvent)JsonConvert.DeserializeObject(eventJson, eventType);
    }
}
