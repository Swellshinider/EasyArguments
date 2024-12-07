namespace EasyArguments.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class ArgumentExecutorAttribute<T>(string methodName) : Attribute
{
    public string ExecutorName { get; } = methodName;
}