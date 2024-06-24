using vesa.Core.Constants;
using vesa.Core.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Parquet;

namespace vesa.Core.Infrastructure;

public class ParquetEventStreamSerializer<IEvent> : ParquetStreamSerializer<IEvent>
{

    //public override async Task<IEvent> DeserializeAsync(Stream stream)
    //{
    //    var eventJObject = (await ParquetConvert.DeserializeAsync<JObject>(stream)).FirstOrDefault();
    //    var eventTypeName = (string)eventJObject[JsonPropertyName.EventTypeName];
    //    return (IEvent)JsonConvert.DeserializeObject(eventJObject.ToString(), TypeHelper.GetType(eventTypeName));
    //}
}
