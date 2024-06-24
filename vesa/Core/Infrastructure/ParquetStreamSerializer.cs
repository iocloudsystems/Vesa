using vesa.Core.Abstractions;
using vesa.Core.Constants;
using vesa.Core.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Parquet;
using Parquet.Schema;
using System.IO;
using System.Reflection;
using System.Text;

namespace vesa.Core.Infrastructure;


public class ParquetStreamSerializer<T> : IStreamSerializer<T>
{
    public string FileExtension => "parquet";

    public async Task<Stream> SerializeAsync(T item)
    {

        using (var stream = new MemoryStream())
        {
            try
            {
                Type itemType = item.GetType();

                // Serialize item to JSON
                var itemJson = JsonConvert.SerializeObject(item);

                // Deserialize JSON back to JObject
                var itemJObject = JsonConvert.DeserializeObject<JObject>(itemJson);

                // Deserialize JObject to the specific item type
                var typedItem = JsonConvert.DeserializeObject(itemJObject.ToString(), itemType);

                // Create a generic List of the specific item type
                Type listType = typeof(List<>);
                Type genericListType = listType.MakeGenericType(itemType);
                object genericList = Activator.CreateInstance(genericListType, new  { typedItem });

                // Add the deserialized item to the generic list
               // MethodInfo addMethod = genericListType.GetMethod("Add");
                //addMethod.Invoke(genericList, new[] { typedItem });

                // Get the SerializeAsync method
                MethodInfo[] methods = typeof(ParquetConvert).GetMethods(BindingFlags.Static | BindingFlags.Public)
                        .Where(m => m.Name == "SerializeAsync").ToArray();
                MethodInfo serializeAsyncMethodInfo = methods[0];

                // Make the SerializeAsync method generic
                MethodInfo genericSerializeAsyncMethodInfo = serializeAsyncMethodInfo.MakeGenericMethod(itemType);

                // Create a new MemoryStream to write the data
                using MemoryStream newStream = new MemoryStream();

                // Call SerializeAsync using dynamic to avoid casting issues
                //dynamic task = genericSerializeAsyncMethodInfo.Invoke(null, new object?[] { genericList, newStream, null, CompressionMethod.Snappy, 5000, false });
                //await task;

                //Type itemType = item.GetType();

                //// Serialize item to JSON
                //var itemJson = JsonConvert.SerializeObject(item);

                //// Deserialize JSON back to JObject
                //var itemJObject = JsonConvert.DeserializeObject<JObject>(itemJson);

                //// Deserialize JObject to the specific item type
                //var typedItem = (object)JsonConvert.DeserializeObject(itemJObject.ToString(), itemType);

                //// Create a generic List of the specific item type
                //Type listType = typeof(List<>);
                //Type genericListType = listType.MakeGenericType(itemType);
                //object genericList = Activator.CreateInstance(genericListType);

                //// Add the deserialized item to the generic list
                //MethodInfo addMethod = genericListType.GetMethod("Add");
                //addMethod.Invoke(genericList, new[] { typedItem });


                //MethodInfo[] methods = typeof(ParquetConvert).GetMethods(BindingFlags.Static | BindingFlags.Public)
                //        .Where(m => m.Name == "SerializeAsync").ToArray();


                //MethodInfo serializeAsyncMethodInfo = methods[0];



                //MethodInfo intMethodInfo = serializeAsyncMethodInfo.MakeGenericMethod(itemType);
                //Stream newStream = default;



                //var task = await (Task<ParquetSchema>)intMethodInfo.Invoke(null, new object?[] { genericList, stream, null, CompressionMethod.Snappy, 5000, false });

                //await ParquetConvert.SerializeAsync(genericItems, stream);
                return stream;
            }
            catch(Exception ex)
            {

            }
        }
        return null;
  
    }

    public virtual async Task<T> DeserializeAsync(Stream stream)
    { 
        var jObject = (await ParquetConvert.DeserializeAsync<JObject>(stream)).FirstOrDefault();
        var typeName = (string)jObject[JsonPropertyName.EventTypeName];
        return (T)JsonConvert.DeserializeObject(jObject.ToString(), TypeHelper.GetType(typeName));

    }
}
