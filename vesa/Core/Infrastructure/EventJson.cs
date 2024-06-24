using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using vesa.Core.Abstractions;
using vesa.Core.Helpers;

namespace vesa.Core.Infrastructure;

public class EventJson
{
    public EventJson()
    {
    }

    public EventJson(IEvent @event)
    {
        Event = @event;
    }

    public string Id { get; set; }
    public string Subject { get; set; }
    public string EventTypeName { get; set; }
    public DateTimeOffset EventDate { get; set; }
    public string TriggeredBy { get; set; }
    public int SequenceNumber { get; set; }
    public string IdempotencyToken { get; set; }

    [JsonIgnore]
    public string Json { get; set; }

    [NotMapped]
    public IEvent Event
    {
        get => (IEvent)JsonConvert.DeserializeObject(Json, TypeHelper.GetType(EventTypeName));
        set
        {
            Id = value.Id;
            Subject = value.Subject;
            EventTypeName = value.EventTypeName;
            EventDate = value.EventDate;
            TriggeredBy = value.TriggeredBy;
            SequenceNumber = value.SequenceNumber;
            IdempotencyToken = value.IdempotencyToken;
            Json = JsonConvert.SerializeObject(value);
        }
    }
}
