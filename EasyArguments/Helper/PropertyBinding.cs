using EasyArguments.Attributes;
using System.Reflection;
using System.Text;

namespace EasyArguments.Helper;

/// <summary>
/// Small helper class to tie together a property and its <see cref="ArgumentAttribute"/>.
/// </summary>
public class PropertyBinding
{
	/// <summary>
	/// The padding size used for formatting the usage string.
	/// </summary>
	public static readonly int PAD_SIZE = 50;
	
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
	public bool Matches(string arg, char separator)
	{
		var separatorIndex = arg.IndexOf(separator);
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
	/// </summary>
	/// <remarks> 
	/// Example: <c>[-v, --version]</c> if optional, or <c>-v, --version</c> if required.
	/// </remarks>
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

	internal void AssignValue(object target, object? value)
	{
		var propType = Property.PropertyType;
		var argAttr = ArgumentAttr;
		
		// If property is bool or bool?, no value means just set it to true and check if InvertBoolean is set.
		if (propType.IsBoolean())
		{
			bool bValue;
			
			if (value is string str)
				bValue = str.ToBoolean();
			else if (value is bool bl)
				bValue = bl;
			else 
				bValue = value == null;

			if (argAttr.InvertBoolean)
				bValue = !bValue;

			// If property is bool? (nullable), box the bool into bool? 
			if (propType == typeof(bool?))
				Property.SetValue(target, (bool?)bValue);
			else
				Property.SetValue(target, bValue);
			
			Execute(target);
			return;
		}

		// If no value was provided but it's not a boolean -> might be an error
		if (value == null)
			throw new ArgumentException($"{GetName()} must receive a value");

		// If it’s a string property, just set it directly:
		if (propType == typeof(string)) 
		{
			Property.SetValue(target, value);
			Execute(target);
			return;
		}

		// Attempt to convert to other known types (int, float, enum, etc.) 
		// For simplicity, we'll just let Convert.ChangeType handle primitives.
		try
		{
			var converterValue = Convert.ChangeType(value, Nullable.GetUnderlyingType(propType) ?? propType);
			Property.SetValue(target, converterValue);
			Execute(target);
		}
		catch (Exception ex)
		{
			throw new ArgumentException($"Failed to convert value '{value}' to type {propType.Name}: {ex.Message}", ex);
		}
	}

	internal void Execute(object target)
	{
		foreach (var execAttrib in Property.GetCustomAttributes<ExecutorAttribute>())
		{
			var currentValue = Property.GetValue(target);
			var resultValue = execAttrib.MethodInfo.Invoke(target, [currentValue]);
			
			if (execAttrib.AssignResultToProperty)
				AssignValue(target, resultValue);
		}
	}
}