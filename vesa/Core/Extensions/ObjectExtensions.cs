using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace vesa.Core.Extensions
{
    public static class ObjectExtensions
    {
        public static T Clone<T>(this object instance)
        {
            var json = JsonConvert.SerializeObject(instance);
            var clone = JsonConvert.DeserializeObject<T>(json);
            return clone;
        }

        public static T SetNewInstanceProperty<T>(this T instance, string propertyName, JToken propertyValue)
        {
            var json = JsonConvert.SerializeObject(instance);
            var jobject = JObject.Parse(json);
            jobject[propertyName] = propertyValue;
            var type = instance.GetType();
            return (T)JsonConvert.DeserializeObject(jobject.ToString(), type);
        }
    }
}
