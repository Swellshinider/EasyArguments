using EasyArguments.Attributes;

namespace EasyArguments.Exceptions;

/// <summary>
/// The exception that is thrown when a class does not have the required <see cref="ArgumentsControllerAttribute"/> attribute.
/// </summary>
public class MissingControllerException : Exception
{
	/// <summary>
	/// Initializes a new instance of the <see cref="MissingControllerException"/> class with a specified error message.
	/// </summary>
	/// <param name="type">The type that is missing the <see cref="ArgumentsControllerAttribute"/> attribute.</param>
	public MissingControllerException(Type type)
		: base($"The target class '{type.Name}' must be decorated with the '{nameof(ArgumentsControllerAttribute)}' attribute.") { }
}