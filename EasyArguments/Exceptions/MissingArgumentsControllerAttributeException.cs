using EasyArguments.Attributes;

namespace EasyArguments.Exceptions;

/// <summary>
/// The exception that is thrown when a class does not have the required <see cref="ArgumentsControllerAttribute"/> attribute.
/// </summary>
public class MissingArgumentsControllerAttributeException : Exception
{
	public MissingArgumentsControllerAttributeException(Type type)
		: base($"The target class '{type.Name}' must be decorated with the 'ArgumentsControllerAttribute' attribute.") { }
}