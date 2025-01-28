using EasyArguments.Attributes;
using System.Reflection;
using System.Text;

namespace EasyArguments.Helper;

/// <summary>
/// Small helper class to tie together a property and its <see cref="ArgumentAttribute"/>.
/// </summary>
public class PropertyBinding
{
	internal static readonly int PAD_SIZE = 50;
	
	/// <summary>
	/// Initializes a new instance of the <see cref="PropertyBinding"/> class.
	/// </summary>
	/// <param name="property">The property to bind.</param>
	/// <param name="argumentAttr">The argument attribute associated with the property.</param>
	/// <param name="parent">The parent property binding, if any.</param>
	public PropertyBinding(PropertyInfo property, ArgumentAttribute argumentAttr, PropertyBinding? parent = null)
	{
		Property = property;
		ArgumentAttr = argumentAttr;
		Parent = parent;
		Children = [];
	}

	/// <summary>
	/// Gets the property being bound.
	/// </summary>
	public PropertyInfo Property { get; }

	/// <summary>
	/// Gets the argument attribute associated with the property.
	/// </summary>
	public ArgumentAttribute ArgumentAttr { get; }

	/// <summary>
	/// Gets the parent property binding, if any.
	/// </summary>
	public PropertyBinding? Parent { get; }

	/// <summary>
	/// Gets the list of child property bindings.
	/// </summary>
	public List<PropertyBinding> Children { get; }

	/// <summary>
	/// Returns true if <paramref name="arg"/> matches either this property's
	/// short name or long name (without considering any '=' or other characters).
	/// </summary>
	public bool Matches(string arg)
	{
		// We handle both "arg=" and separate usage. So let's strip out anything after an '='
		var separatorIndex = arg.IndexOf('=');
		string justKey = separatorIndex >= 0 ? arg[..separatorIndex] : arg;

		return
			(ArgumentAttr.ShortName != null && justKey == ArgumentAttr.ShortName) ||
			(ArgumentAttr.LongName != null && justKey == ArgumentAttr.LongName);
	}

	/// <summary>
	/// Builds a usage string for the current property argument.
	/// </summary>
	public string Usage(string indentation = "")
	{
		var builder = new StringBuilder();
		var prefix = string.Empty;

		if (Parent != null)
			prefix = $"{indentation}{Parent.GetName()} ";

		prefix += GetName();
		builder.Append($"{prefix.PadRight(PAD_SIZE)}{ArgumentAttr.HelpMessage}{(ArgumentAttr.Required ? " (Required)" : "")}");

		// Add a new line if has children
		if (Children.Count != 0)
			builder.AppendLine();

		indentation += "    ";
		foreach (var child in Children)
			builder.AppendLine(child.Usage(indentation));

		return builder.ToString();
	}

	/// <summary>
	/// Returns a nice formatting of <see cref="ArgumentAttribute.ShortName"/> and <see cref="ArgumentAttribute.LongName"/>.
	/// Example: [-v, --version] if optional, or -v, --version if required.
	/// </summary>
	public string GetName()
	{
		var sb = new StringBuilder();
		var required = ArgumentAttr.Required;

		if (!required) sb.Append('[');

		if (!string.IsNullOrWhiteSpace(ArgumentAttr.ShortName))
		{
			sb.Append(ArgumentAttr.ShortName);
			
			if (!string.IsNullOrWhiteSpace(ArgumentAttr.LongName))
				sb.Append(", ");
		}
		
		if (!string.IsNullOrWhiteSpace(ArgumentAttr.LongName))
			sb.Append(ArgumentAttr.LongName);
			
		if (!required) sb.Append(']');

		return sb.ToString();
	}
}