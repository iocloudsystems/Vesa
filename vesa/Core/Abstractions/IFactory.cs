namespace vesa.Core.Abstractions;

public interface IFactory<T>
    where T : class, new()
{
    T Create();
}
