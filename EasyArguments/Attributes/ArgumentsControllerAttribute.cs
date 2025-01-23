using EasyArguments.Enums;

namespace EasyArguments.Attributes;

/// <summary>
/// Specifies that a class is an arguments controller, which is responsible for handling and managing arguments.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class ArgumentsControllerAttribute : Attribute
{
	/// <summary>
	/// Gets or sets a value indicating whether the order of arguments should be respected.
	/// Defaults to <c>true</c>.
	/// </summary>
	public bool RespectOrder { get; set; } = true;
	
	/// <summary>
	/// Gets os sets a value indicating whether the -h and --help argument should be automatically implemented
	/// Defaults to <c>true</c>
	/// </summary>
	public bool AutoHelpArgument { get; set; } = true;
	
	/// <summary>
	/// Gets os sets a value indicating the separators that you wanna use in your argument parser
	/// Defaults to <c>SeparatorTypes.Equals | SeparatorTypes.Space</c>
	/// </summary>
	/// <remarks>
	/// Check <see cref="SeparatorTypes"/> for more information
	/// </remarks>
	public SeparatorTypes Separators { get; set; } = SeparatorTypes.Equals | SeparatorTypes.Space;
}