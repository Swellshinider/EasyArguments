namespace EasyArguments.Exceptions;

/// <summary>
/// The exception that is thrown when a required argument is missing.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MissingRequiredArgumentException"/> class with a specified error message.
/// </remarks>
/// <param name="argumentName">The name (or set of names) of the missing required argument.</param>
public sealed class MissingRequiredArgumentException(string argumentName) : Exception($"A required argument '{argumentName}' is missing.") { }