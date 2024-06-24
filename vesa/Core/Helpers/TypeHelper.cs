using System.Reflection;

namespace vesa.Core.Helpers;

public static class TypeHelper
{
    static Assembly[] currentDomainAssemblies = AppDomain.CurrentDomain.GetAssemblies();

    public static Type GetType(string typeName)
    {
        var type = Type.GetType(typeName);
        if (type != null) return type;
        foreach (var a in currentDomainAssemblies)
        {
            type = a.GetType(typeName);
            if (type != null)
                return type;
        }
        return null;
    }
}
