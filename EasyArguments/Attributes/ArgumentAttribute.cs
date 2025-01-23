namespace EasyArguments.Attributes;

/// <summary>
/// An attribute used to associate one or more argument names with a property.
/// </summary>
/// <remarks>
/// This attribute can be applied to properties to specify argument names, a description and if it's required.
/// </remarks>
/// <param name="argumentNames">An array of argument names associated with the property.</param>
[AttributeUsage(AttributeTargets.Property)]
public class ArgumentAttribute(params string[] argumentNames) : Attribute
{
	public string[] ArgumentNames { get; } = argumentNames;

	public required string Description { get; set; }

	public bool Required { get; set; } = false;
}