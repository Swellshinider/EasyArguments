namespace EasyArguments.Enums;

/// <summary>
/// Specifies the types of separators that can be used in arguments.
/// </summary>
[Flags]
public enum SeparatorTypes 
{
	/// <summary>
	/// Equals separator, example: --output="C:\\MyFolder"
	/// </summary>
	Equals,
	
	/// <summary>
	/// Space separator, example: --output "C:\\MyFolder"
	/// </summary>
	Space,
	
	/// <summary>
	/// Space separator, example: --output->"C:\\MyFolder"
	/// </summary>
	Arrow,
	
	/// <summary>
	/// Space separator, example: --output."C:\\MyFolder"
	/// </summary>
	Dot
}