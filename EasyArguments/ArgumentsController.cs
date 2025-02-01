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

	/// <summary>
	/// Initializes a new instance of the <see cref="ArgumentsController{T}"/> class with the specified command-line arguments.
	/// </summary>
	/// <param name="argLine">The command-line arguments to parse.</param>
	public ArgumentsController(string argLine)
	{
		_rootType = typeof(T);
		_tokens = argLine.Tokenize();
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

	private bool IsHelp => Current == "-h" || Current == "--help";

	/// <summary>
	/// Parses the command-line arguments into an instance of type <typeparamref name="T"/>.
	/// </summary>
	/// <param name="helpMessageDisplayed">Indicates whether the help message was displayed.</param>
	/// <returns>An instance of type <typeparamref name="T"/> with the parsed arguments.</returns>
	public T Parse(out bool helpMessageDisplayed)
	{
		var instance = new T();
		_position = 0;

		helpMessageDisplayed = ParseObject(instance);

		return instance;
	}

	/// <summary>
	/// Parses the command-line arguments into an instance of type <typeparamref name="T"/>.
	/// </summary>
	/// <returns>An instance of type <typeparamref name="T"/> with the parsed arguments.</returns>
	public T Parse() => Parse(out var _);

	/// <summary>
	/// Extracts the properties bound to arguments in the type <typeparamref name="T"/>.
	/// </summary>
	/// <remarks>
	/// Useful if you wanna build your own parser.
	/// </remarks>
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

		foreach (var binding in propertyBindings)
		{
			var property = binding.Property;
			var attribute = binding.ArgumentAttr;

			if (Current == null)
			{
				ValidateRequired(binding, attribute);
				SetupBooleanDefault(target, binding, property, attribute);
			}
			else if (IsHelp && _controllerAttribute.AutoHelpArgument)
			{
				DisplayUsage(propertyBindings);
				return true;
			}
			else if (binding.Matches(Current, separator))
			{
				// If matches and it's not a primitive type, must be a nested argument
				if (property.PropertyType.IsNestedArgument()) 
				{
					// help was displayed
					if (ParseNestedArguments(target, binding, property))
						return true;
				}
				else // Otherwise is a normal argument
					ParseArgumentValue(target, separator, binding, property);
			}
			else if (propertyBindings.Any(p => p.Matches(Current, separator)) && property.PropertyType == typeof(bool))
			{
				if (attribute.InvertBoolean)
					binding.AssignValue(target, !attribute.InvertBoolean);
			}
			else
			{
				// If 'Current' does not matches with the current binding and any other binding, assign the value directly
				// Means it's a positional argument
				binding.AssignValue(target, Current);
				_position++;
			}
		}

		return false;
	}

	private static void DisplayUsage(IEnumerable<PropertyBinding> propertyBindings)
	{
		foreach (var prop in propertyBindings)
			Console.WriteLine(prop.Usage());
	}

	private static void SetupBooleanDefault(object target, PropertyBinding binding, PropertyInfo property, ArgumentAttribute attribute)
	{
		// If no argument is provided and the property is a boolean with inversion, set its default value
		if (property.PropertyType == typeof(bool) && attribute.InvertBoolean)
			binding.AssignValue(target, !attribute.InvertBoolean);
	}

	private static void ValidateRequired(PropertyBinding binding, ArgumentAttribute attribute)
	{
		if (attribute.Required)
			throw new ArgumentException($"Argument '{binding.GetName()}' is required.");
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
				binding.AssignValue(target, parts[1]);
			}

			// No separator (--verbose), should be a boolean flag
			else if (property.PropertyType.IsBoolean())
				binding.AssignValue(target, true);

			// If no separator and it's not a boolean, throw error
			else
				throw new ArgumentException($"No value found for provided argument '{argument}'");
		}

		// Check if the current is a separator
		else if (Current.Contains(separator))
		{
			// Advance one to get the value
			_position++;

			if (Current == null)
				throw new ArgumentException($"No value found for provided argument '{argument}'");

			binding.AssignValue(target, Current);
			_position++;
		}

		// If matches with an argument, then is missing a value 
		else if (binding.Matches(Current, separator))
			throw new ArgumentException($"No value found for provided argument '{argument}'");

		if (binding.Property.PropertyType.IsBoolean())
			binding.AssignValue(target, true);
		else
		{
			binding.AssignValue(target, Current);
			_position++;
		}
	}

	private bool ParseNestedArguments(object target, PropertyBinding binding, PropertyInfo property)
	{
		var nestedInstance = Activator.CreateInstance(property.PropertyType)!;

		binding.AssignValue(target, nestedInstance);

		_position++;
		return ParseObject(nestedInstance);
	}
}