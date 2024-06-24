using vesa.Core.Abstractions;
using vesa.Core.Extensions;
using Newtonsoft.Json;
using System.Text;

namespace vesa.Core.Infrastructure;

public class TextStreamSerializer<T> : IStreamSerializer<T>
{
    public string FileExtension => "txt";

    public Task<Stream> SerializeAsync(T item)
    {
        var itemJson = JsonConvert.SerializeObject(item);
        return Task.FromResult((Stream)(new MemoryStream(Encoding.Default.GetBytes(itemJson))));
    }

    public virtual Task<T> DeserializeAsync(Stream stream)
    {
        var itemJson = stream.Deserialize<string>();
        return Task.FromResult(JsonConvert.DeserializeObject<T>(itemJson));
    }
}
