using EasyArguments.Attributes;
using System.Reflection;
using System.Text;

namespace EasyArguments.Helper;

/// <summary>
/// Small helper class to tie together a property and its <see cref="ArgumentAttribute"/>.
/// </summary>
public record PropertyBinding
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
	public IEnumerable<PropertyBinding> Children { get; internal set; }

	/// <summary>
	/// Gets a value indicating whether the property is waiting for lazy execution.
	/// </summary>
	public bool WaitingForLazyExecution { get; private set; } = false;

	/// <summary>
	/// Gets a value indicating whether this property binding has child bindings.
	/// </summary>
	public bool IsParent => Children.Any();

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
	public string Usage()
	{
		var builder = new StringBuilder();

		builder.Append(GetName().PadRight(PAD_SIZE));
		builder.Append(ArgumentAttr.HelpMessage);

		return builder.ToString();
	}

	/// <summary>
	/// Returns a nice formatting of <see cref="ArgumentAttribute.ShortName"/> and <see cref="ArgumentAttribute.LongName"/>.
	/// </summary>
	/// <remarks> 
	/// Example: <c>-v, --version</c>, <c>-v</c> or <c>--version</c>
	/// </remarks>
	public string GetName()
	{
		var sb = new StringBuilder();

		if (!string.IsNullOrWhiteSpace(ArgumentAttr.ShortName))
		{
			sb.Append(ArgumentAttr.ShortName);

			if (!string.IsNullOrWhiteSpace(ArgumentAttr.LongName))
				sb.Append(", ");
		}

		if (!string.IsNullOrWhiteSpace(ArgumentAttr.LongName))
			sb.Append(ArgumentAttr.LongName);

		return sb.ToString();
	}

	internal string GetAvailableName()
	{
		if (!string.IsNullOrWhiteSpace(ArgumentAttr.ShortName))
			return ArgumentAttr.ShortName;

		return ArgumentAttr.LongName ?? "";
	}

	/// <summary>
	/// Assigns a value to the property of the target object, converting the value if necessary.
	/// </summary>
	/// <param name="target">The object whose property value is to be set.</param>
	/// <param name="value">The value to assign to the property. Can be null for boolean properties.</param>
	/// <exception cref="ArgumentException">
	/// Thrown if the property is not a boolean and the value is null, or if the value cannot be converted to the property type.
	/// </exception>
	public void AssignValue(object target, object? value)
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

	/// <summary>
	/// Assigns a nested value to the property of the target object, converting the value if necessary.
	/// </summary>
	/// <param name="target">The object whose property value is to be set.</param>
	/// <param name="value">The value to assign to the property.</param>
	public void AssignNested(object target, object? value) 
	{
		var propType = Property.PropertyType;
		
		try
		{
			var converterValue = Convert.ChangeType(value, Nullable.GetUnderlyingType(propType) ?? propType);
			Property.SetValue(target, converterValue);
			WaitingForLazyExecution = true;
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
			var methodInfo = execAttrib.MethodInfo;
			var parametersCount = methodInfo.GetParameters().Length;
			object? resultValue;

			if (parametersCount == 0)
				resultValue = methodInfo.Invoke(target, null);
			else if (parametersCount == 1)
				resultValue = methodInfo.Invoke(target, [currentValue]);
			else
				throw new TargetParameterCountException($"Execution of the method '{methodInfo.Name}' failed! Expected 0 or 1 parameter, but found {parametersCount} parameters.");

			if (execAttrib.AssignResultToProperty)
				AssignValue(target, resultValue);
		}
		
		WaitingForLazyExecution = false;
	}

	/// <inheritdoc/>
	public override string ToString() => GetName();
}