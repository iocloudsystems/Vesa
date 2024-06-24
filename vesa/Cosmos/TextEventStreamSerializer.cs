using vesa.Core.Abstractions;
using vesa.Core.Constants;
using vesa.Core.Extensions;
using vesa.Core.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace vesa.Core.Infrastructure;

public class TextEventStreamSerializer : TextStreamSerializer<IEvent>
{
    public override Task<IEvent> DeserializeAsync(Stream stream)
    {
        var eventJson = stream.Deserialize<string>();
        var eventJObject = JsonConvert.DeserializeObject<JObject>(eventJson);
        var eventTypeName = (string)eventJObject[JsonPropertyName.EventTypeName];
        return Task.FromResult((IEvent)JsonConvert.DeserializeObject(eventJson, TypeHelper.GetType(eventTypeName)));
    }
}
