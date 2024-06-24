namespace vesa.Core.Extensions;

public static class TypeExtensions
{
    public static IEnumerable<Type> GetTypesOf(this Type type)
    =>
        AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => type.IsAssignableFrom(p));
}
