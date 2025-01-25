namespace EasyArguments.Attributes;

/// <summary>
/// Represents an attribute that can be applied to properties to specify command-line argument metadata.
/// </summary>
/// <remarks>
/// This attribute is designed for properties that are used to map command-line arguments.
/// It allows specifying short and long argument names, help messages, and a separator character for parsing.
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class ArgumentAttribute : Attribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ArgumentAttribute"/> class with specified short name, long name, and help message.
	/// </summary>
	/// <param name="shortName">The short name for the argument (e.g., "-s"). Can be <c>null</c>.</param>
	/// <param name="longName">The long name for the argument (e.g., "--sample"). Can be <c>null</c>.</param>
	/// <param name="helpMessage">A description of the argument's purpose, displayed in help output.</param>
	/// <remarks>
	/// If both <paramref name="shortName"/> and <paramref name="longName"/> are <c>null</c> (Not recommended), 
	/// the long name will default to the lowercase form of the property name prefixed with a dash 
	/// (e.g., for a property named <c>Name</c>, the long name would be <c>--name</c>).
	/// </remarks>
	public ArgumentAttribute(string? shortName, string? longName, string? helpMessage)
	{
		ShortName = shortName;
		LongName = longName;
		HelpMessage = helpMessage;
	}

	/// <summary>
	/// Gets or sets the short name for the argument.
	/// </summary>
	public string? ShortName { get; internal set; }

	/// <summary>
	/// Gets or sets the long name for the argument.
	/// </summary>
	public string? LongName { get; internal set; }

	/// <summary>
	/// Gets the help message for the argument, describing its purpose.
	/// </summary>
	public string? HelpMessage { get; }

	/// <summary>
	/// Gets or sets if this argument is required.
	/// </summary>
	/// <remarks>
	/// Defaults to <c>false</c>.
	/// </remarks>
	public bool Required { get; set; } = false;

	/// <summary>
	/// Gets or sets a value indicating whether the argument value should be inverted for boolean properties.
	/// (only works for boolean types)
	/// </summary>
	/// <remarks>
	/// When set to <c>true</c>, passing this argument will result in the associated boolean property being set to <c>false</c> instead of <c>true</c>.
	/// Defaults to <c>false</c>.
	/// </remarks>
	public bool InvertBoolean { get; set; }
}