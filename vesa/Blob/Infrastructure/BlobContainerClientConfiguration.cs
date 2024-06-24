using vesa.Blob.Abstractions;

namespace vesa.Blob.Infrastructure
{
    public class BlobContainerClientConfiguration : IBlobContainerClientConfiguration
    {
        public string ConnectionStringKey { get; set; }
        public string ContainerName { get; set; }
    }
}
