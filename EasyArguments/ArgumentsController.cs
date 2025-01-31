using EasyArguments.Attributes;
using EasyArguments.Exceptions;
using EasyArguments.Helper;
using System.Reflection;
using System.Text;

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

	public T Parse()
	{
		var instance = new T();
		_position = 0;

		_ = ParseObject(instance);

		return instance;
	}

	private bool ParseObject(object argObject)
	{
		var arguments = argObject.GetType().ExtractProperties();
		var helpPrinted = false;

		foreach (var argument in arguments)
		{
			// If there's no more args but the argument is required
			if (Current == null)
			{
				if (argument.ArgumentAttr.Required)
					throw new ArgumentException($"Argument '{argument.GetName()}' is required.");

				continue;
			}
			
			if (argument.Matches(Current, _controllerAttribute.Separator) && 
				argument.Property.PropertyType.IsClass &&
				argument.Property.PropertyType != typeof(string))
			{
				var subInstance = Activator.CreateInstance(argument.Property.PropertyType)!;
				argument.Property.SetValue(argObject, subInstance);

				_position++;
				helpPrinted = ParseObject(subInstance);
				continue;
			}
			
			if (argument.Matches(Current, _controllerAttribute.Separator))
			{
				var argDetected = Current;
				_position++;

				// If next is null and is not a flag, throw error
				if (Current == null)
				{
					// Contains separator, so its something like this: --arg=value
					if (argDetected.Contains(_controllerAttribute.Separator))
					{
						var parts = argDetected.Split(_controllerAttribute.Separator, 2);
						var arg = parts[0];
						var value = parts[1];
						var converted = ConvertValue(argument, value);
						argument.Property.SetValue(argObject, converted);
						continue;
					}
					
					if (!argument.Property.PropertyType.IsBoolean())
						throw new ArgumentException($"No value found for provided argument '{argDetected}'");
					else
						argument.Property.SetValue(argObject, true);

					continue;
				}

				// Check if is the separator
				if (Current.Equals(_controllerAttribute.Separator))
				{
					_position++;
					argument.Property.SetValue(argObject, ConvertValue(argument, Current));
					_position++;
					continue;
				}
				
				// If matches with an argument, then is missing a value
				if (argument.Matches(Current, _controllerAttribute.Separator))
					throw new ArgumentException($"No value found for provided argument '{argDetected}'");
					
				
				var val = ConvertValue(argument, Current);
				argument.Property.SetValue(argObject, val);
			}
		}
		
		return helpPrinted;
	}

	/// <summary>
	/// Builds a usage string for the current object.
	/// </summary>
	public string GetUsageText() => typeof(T).ExtractProperties().GetUsage(_controllerAttribute.AutoHelpArgument);

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