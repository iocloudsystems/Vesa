namespace vesa.Blob.Abstractions
{
    public interface IBlobContainerClientConfiguration
    {
        string ConnectionStringKey { get; set; }
        string ContainerName { get; set; }
    }
}