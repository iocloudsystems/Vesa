using Confluent.Kafka;

namespace vesa.Kafka.Infrastructure
{
    public class IgnoreSerializer : ISerializer<Ignore>
    {
        public byte[] Serialize(Ignore data, SerializationContext context)
        {
            return null;
        }
    }
}
