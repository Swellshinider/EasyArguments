using EasyArguments.Attributes;
using System.Reflection;

namespace EasyArguments;

internal readonly struct ExecutableArgument<T>(PropertyInfo propertyInfo, object? value, ArgumentExecutorAttribute<T> attribute)
{
    public readonly PropertyInfo PropertyInfo { get; } = propertyInfo;
    public readonly object? Value { get; } = value;
    public readonly ArgumentExecutorAttribute<T> Attribute { get; } = attribute;
}