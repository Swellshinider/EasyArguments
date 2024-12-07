using EasyArguments.Attributes;

namespace EasyArguments.Exceptions;

/// <summary>
/// The exception that is thrown when a class does not have the required <see cref="ArgumentsControllerAttribute"/> attribute.
/// </summary>
public class MissingArgumentsControllerAttributeException()
    : Exception("The target class must be decorated with the 'ArgumentsControllerAttribute' attribute.") { }