namespace vesa.Core.Abstractions;
public interface IStreamSerializer<T>
{
    string FileExtension { get; }
    Task<Stream> SerializeAsync(T item);
    Task<T> DeserializeAsync(Stream stream);
}
