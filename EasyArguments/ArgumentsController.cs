using EasyArguments.Attributes;
using EasyArguments.Exceptions;
using EasyArguments.Helper;
using System.Reflection;

namespace EasyArguments;

/// <summary>
/// A re-usable, instantiable class that can parse command-line arguments into type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">A class annotated with <see cref="ArgumentsControllerAttribute"/>.</typeparam>
public class ArgumentsController<T> where T : new()
{
	private readonly Type _rootType;
	private readonly List<string> _tokens;
	private readonly ArgumentsControllerAttribute _controllerAttribute;

	private int _position = 0;

	/// <summary>
	/// Initializes a new instance of the <see cref="ArgumentsController{T}"/> class with the specified command-line arguments.
	/// </summary>
	/// <param name="args">The command-line arguments to parse.</param>
	public ArgumentsController(string[] args)
	{
		_rootType = typeof(T);
		_tokens = string.Join(' ', args).Tokenize();
		_controllerAttribute = _rootType.GetCustomAttribute<ArgumentsControllerAttribute>()
			?? throw new MissingControllerException(_rootType);
	}

	private string? Current
	{
		get
		{
			if (_position >= _tokens.Count)
				return null;

			return _tokens[_position];
		}
	}

	/// <summary>
	/// Parses the command-line arguments and returns an instance of type <typeparamref name="T"/>.
	/// </summary>
	/// <returns>An instance of type <typeparamref name="T"/> populated with the parsed arguments.</returns>
	public T Parse()
	{
		var instance = new T();
		_position = 0;

		_ = ParseObject(instance);

		return instance;
	}

	/// <summary>
	/// Extracts the properties bound to arguments in the type <typeparamref name="T"/>.
	/// </summary>
	/// <returns>An enumerable of <see cref="PropertyBinding"/> representing the bound properties.</returns>
	public IEnumerable<PropertyBinding> ExtractBoundProperties() => _rootType.ExtractProperties();

	/// <summary>
	/// Generates the usage text for the command-line arguments.
	/// </summary>
	/// <returns>A string representing the usage text.</returns>
	public string GetUsageText() => ExtractBoundProperties().GetUsage(_controllerAttribute.AutoHelpArgument);

	private bool ParseObject(object target)
	{
		var propertyBindings = target.GetType().ExtractProperties();
		var separator = _controllerAttribute.Separator;
		var helpPrinted = false;

		foreach (var binding in propertyBindings)
		{
			var property = binding.Property;
			var attribute = binding.ArgumentAttr;
			
			// If there's no more args but the argument is required, throw error
			if (Current == null)
			{
				if (attribute.Required)
					throw new ArgumentException($"Argument '{binding.GetName()}' is required.");

				continue;
			}
			
			// If auto help is set to true and help wasn't showed yet, display help
			if ((Current == "-h" || Current == "--help") && _controllerAttribute.AutoHelpArgument && !helpPrinted)
			{
				Console.WriteLine(binding.Usage());
				return true;
			}
			
			// If matches and it's not a primitive type, must be a nested argument
			if (binding.Matches(Current, separator) && property.PropertyType.IsNestedArgument())
				helpPrinted = CreateAndParseNestedInstance(target, binding, property);
			
			else if (binding.Matches(Current, separator))
				ParseArgumentValue(target, separator, binding, property);
		}
		
		return helpPrinted;
	}

	private void ParseArgumentValue(object target, char separator, PropertyBinding binding, PropertyInfo property)
	{
		var argument = Current!;
		_position++;

		// If current is null, means that our 'argument' is the last one
		if (Current == null)
		{
			// If has a separator (--arg=value), we must parse into two parts
			if (argument.Contains(separator))
			{
				var parts = argument.Split(separator, 2);
				ApplyValue(target, binding, ConvertValue(binding, parts[1]));
			}

			// No separator (--verbose), should be a boolean flag
			else if (property.PropertyType.IsBoolean())
				ApplyValue(target, binding, true);

			// If no separator and it's not a boolean, throw error
			else
				throw new ArgumentException($"No value found for provided argument '{argument}'");
		}

		// Check if the current is a separator
		else if (Current.Equals(separator))
		{
			_position++;
			ApplyValue(target, binding, ConvertValue(binding, Current));
			_position++;
		}
		
		// If matches with an argument, then is missing a value 
		else if (binding.Matches(Current, separator))
			throw new ArgumentException($"No value found for provided argument '{argument}'");
			
		// is a value itself
		else 
			ApplyValue(target, binding, ConvertValue(binding, Current));
	}

	private bool CreateAndParseNestedInstance(object target, PropertyBinding binding, PropertyInfo property)
	{
		bool helpPrinted;
		var nestedInstance = Activator.CreateInstance(property.PropertyType)!;

		ApplyValue(target, binding, nestedInstance);

		_position++;
		helpPrinted = ParseObject(nestedInstance);
		return helpPrinted;
	}

	private void ApplyValue(object target, PropertyBinding binding, object? value, bool recurrence = false)
	{
		binding.Property.SetValue(target, value);
		
		if (!_controllerAttribute.ExecuteWhenParsing || recurrence)
			return;
		
		foreach (var execAttrib in binding.Property.GetCustomAttributes<ExecutorAttributeAttribute>())
		{
			var currentValue = binding.Property.GetValue(target);
			var resultValue = execAttrib.MethodInfo.Invoke(target, [currentValue]);
			
			if (execAttrib.AssignResultToProperty)
				ApplyValue(target, binding, resultValue, true);
		}
	}

	private static object? ConvertValue(PropertyBinding binding, string? value)
	{
		var propType = binding.Property.PropertyType;
		var argAttr = binding.ArgumentAttr;

		// If property is bool or bool?, no value means just set it to true and check if InvertBoolean is set.
		if (propType.IsBoolean())
		{
			var boolValue = string.IsNullOrEmpty(value) || value.ToBoolean();

			if (argAttr.InvertBoolean)
				boolValue = !boolValue;

			// If property is bool? (nullable), box the bool into bool? 
			if (propType == typeof(bool?))
				return (bool?)boolValue;
			else
				return boolValue;
		}

		// If it’s a string property, just set it directly:
		if (propType == typeof(string))
			return value;

		// If no value was provided but it's not a boolean -> might be an error
		if (string.IsNullOrEmpty(value))
			throw new ArgumentException($"{binding.GetName()} must receive a value");

		// Attempt to convert to other known types (int, float, enum, etc.) 
		// For simplicity, we'll just let Convert.ChangeType handle primitives.
		try
		{
			return Convert.ChangeType(value, Nullable.GetUnderlyingType(propType) ?? propType);
		}
		catch (Exception ex)
		{
			throw new ArgumentException($"Failed to convert value '{value}' to type {propType.Name}: {ex.Message}", ex);
		}
	}
}