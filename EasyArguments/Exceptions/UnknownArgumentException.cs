namespace EasyArguments.Exceptions;

/// <summary>
/// The exception that is thrown when an unknown argument is encountered during argument parsing.
/// </summary>
public class UnknownArgumentException(string argumentName) 
    : Exception($"An unknown argument was encountered: '{argumentName}'.")
{
    /// <summary>
    /// Gets the name of the unknown argument that caused the exception.
    /// </summary>
    public string ArgumentName { get; } = argumentName;
}