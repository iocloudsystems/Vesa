using vesa.Core.Abstractions;

namespace vesa.Core.Infrastructure;

public class GenericFactory<T> : IFactory<T>
    where T : class, new()
{
    public T Create() => new T();
}
