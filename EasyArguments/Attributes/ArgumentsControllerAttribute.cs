namespace EasyArguments.Attributes;

/// <summary>
/// An attribute to configure the behavior of a class that defines a group of command-line arguments.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class ArgumentsControllerAttribute : Attribute
{
	/// <summary>
	/// Gets or sets a value indicating whether the order of arguments should be respected during parsing.
	/// </summary>
	/// <remarks>
	/// When set to <c>true</c>, arguments must appear in the specified order. Defaults to <c>true</c>.
	/// </remarks>
	public bool RespectOrder { get; set; } = true;
	
	/// <summary>
    /// Gets or sets a value indicating whether an automatic help argument should be included.
    /// </summary>
    /// <remarks>
    /// When set to <c>true</c>, a help argument (e.g., <c>-h</c> and <c>--help</c>) is automatically generated to display usage information. Defaults to <c>true</c>.
    /// </remarks>
	public bool AutoHelpArgument { get; set; } = true;
	
	/// <summary>
	/// Gets or sets the character used as a separator for parsing arguments.
	/// </summary>
	/// <remarks>
	/// Defaults to <c>'='</c>
	/// </remarks>
	public char Separator { get; set; } = '=';
}