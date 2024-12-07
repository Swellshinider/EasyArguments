namespace EasyArguments.Exceptions;

/// <summary>
/// The exception that is thrown when the value of an argument does not match the expected type.
/// </summary>
public class InvalidArgumentTypeException(string argumentName, Type expectedType) 
    : Exception($"The argument '{argumentName}' does not match the expected type '{expectedType}'.")
{
    /// <summary>
    /// Gets the name of the argument that caused the exception.
    /// </summary>
    public string ArgumentName { get; } = argumentName;

    /// <summary>
    /// Gets the expected type for the argument.
    /// </summary>
    public Type ExpectedType { get; } = expectedType;
}