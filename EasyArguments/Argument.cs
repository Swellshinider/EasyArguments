using EasyArguments.Attributes;
using System.Reflection;

namespace EasyArguments;

internal readonly struct Argument(PropertyInfo propertyInfo, ArgumentBaseAttribute attribute)
{
    public readonly PropertyInfo PropertyInfo { get; } = propertyInfo;
    public readonly ArgumentBaseAttribute Attribute { get; } = attribute;
}