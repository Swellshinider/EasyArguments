namespace EasyArguments.Attributes;

/// <summary>
/// An attribute to configure the behavior of a class that defines a group of command-line arguments.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class ArgumentsControllerAttribute : Attribute
{
	private char _separator = '=';
	
	/// <summary>
	/// Gets or sets a value indicating whether an automatic help argument should be included.
	/// </summary>
	/// <remarks>
	/// When set to <c>true</c>, a help argument (e.g., <c>-h</c> and <c>--help</c>) is automatically generated to display usage information. Defaults to <c>true</c>.
	/// </remarks>
	public bool AutoHelpArgument { get; set; } = true;
	
	/// <summary>
	/// Gets or sets a value indicating whether the command should be executed when parsing.
	/// </summary>
	/// <remarks>
	/// Defaults to <c>false</c>.
	/// </remarks>
	public bool ExecuteWhenParsing { get; set; } = false;
	
	/// <summary>
	/// Gets or sets the character used as a separator for parsing arguments.
	/// </summary>
	/// <remarks>
	/// Defaults to <c>'='</c>
	/// </remarks>
	public char Separator 
	{
		get => _separator;
		set 
		{
			if (value == '\0')
				throw new ArgumentException($"Invalid separator value '\\0'");
			
			_separator = value;
		}
	}
}