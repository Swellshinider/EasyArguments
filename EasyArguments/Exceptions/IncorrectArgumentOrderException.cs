namespace EasyArguments.Exceptions;

/// <summary>
/// The exception that is thrown when the order of arguments does not match the expected sequence.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="IncorrectArgumentOrderException"/> class.
/// </remarks>
/// <param name="expectedArgumentNames">A string representing the expected sequence of argument names.</param>
/// <param name="receivedArgument">The argument that was received out of order.</param>
public sealed class IncorrectArgumentOrderException(string expectedArgumentNames, string receivedArgument) : Exception($"Incorrect argument order. Expected arguments: {expectedArgumentNames}, but received '{receivedArgument}'.")
{
    /// <summary>
    /// Gets the expected sequence of argument names.
    /// </summary>
    public string ExpectedArgumentNames { get; } = expectedArgumentNames;

    /// <summary>
    /// Gets the argument that was received out of order.
    /// </summary>
    public string ReceivedArgument { get; } = receivedArgument;
}