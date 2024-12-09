namespace EasyArguments.Attributes;

/// <summary>
/// Attribute to specify a method to be executed for a property.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class ArgumentExecutorAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArgumentExecutorAttribute"/> class.
    /// </summary>
    /// <param name="executorType">The type that contains the method to be executed.</param>
    /// <param name="methodName">The name of the method that will be executed.</param>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="executorType"/> is not a class or <paramref name="methodName"/> is null/empty.
    /// </exception>
    public ArgumentExecutorAttribute(Type executorType, string methodName)
    {
        if (!executorType.IsClass)
            throw new ArgumentException("ExecutorType must be a class.", nameof(executorType));

        if (string.IsNullOrWhiteSpace(methodName))
            throw new ArgumentException("MethodName cannot be null or empty.", nameof(methodName));

        ExecutorType = executorType;
        MethodName = methodName;
    }

    /// <summary>
    /// The type that contains the method to be executed.
    /// </summary>
    public Type ExecutorType { get; }

    /// <summary>
    /// Gets the name of the method that will be executed.
    /// </summary>
    public string MethodName { get; }
}