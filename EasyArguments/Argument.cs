using EasyArguments.Attributes;
using System.Reflection;

namespace EasyArguments;

public readonly struct Argument
{
    public Argument(PropertyInfo propertyInfo, ArgumentAttribute attribute)
    {
        PropertyInfo = propertyInfo;
        Attribute = attribute;
    }

    public readonly PropertyInfo PropertyInfo { get; }
    public readonly ArgumentAttribute Attribute { get; }
}