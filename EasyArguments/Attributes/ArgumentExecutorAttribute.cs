namespace EasyArguments.Attributes;

using System;

/// <summary>
/// Enables the execution of a method, passing the property value as an argument.
/// </summary>
/// <typeparam name="T">
/// The class type that contains the method to be executed. 
/// Must have a parameterless constructor.
/// </typeparam>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class ArgumentExecutorAttribute<T> : Attribute where T : new()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArgumentExecutorAttribute{T}"/> class.
    /// Enables the execution of a method, passing the property value as an argument.
    /// </summary>
    /// <param name="methodName">The name of the method to be executed.</param>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="methodName"/> is null, empty, or whitespace.
    /// </exception>
    public ArgumentExecutorAttribute(string methodName)
    {
        if (string.IsNullOrWhiteSpace(methodName))
            throw new ArgumentException("Method name cannot be null, empty, or whitespace.", nameof(methodName));

        ExecutorName = methodName;
        Instance = new T();
    }

    /// <summary>
    /// Gets the name of the method that will be executed.
    /// </summary>
    public string ExecutorName { get; }

    /// <summary>
    /// Gets an instance of the class <typeparamref name="T"/>.
    /// </summary>
    public T Instance { get; }
}