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
	private readonly IEnumerable<PropertyBinding> _rootProperties;

	private int _position = 0;

	/// <summary>
	/// Initializes a new instance of the <see cref="ArgumentsController{T}"/> class with the specified command-line arguments.
	/// </summary>
	/// <param name="args">The command-line arguments to parse.</param>
	public ArgumentsController(string[] args)
	{
		_rootType = typeof(T);
		_controllerAttribute = _rootType.GetCustomAttribute<ArgumentsControllerAttribute>()
			?? throw new MissingControllerException(_rootType);
			
		_tokens = string.Join(' ', args).Tokenize(_controllerAttribute.Separator);
		_rootProperties = _rootType.ExtractProperties();
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ArgumentsController{T}"/> class with the specified command-line arguments.
	/// </summary>
	/// <param name="argLine">The command-line arguments to parse.</param>
	public ArgumentsController(string argLine)
	{
		_rootType = typeof(T);
		_controllerAttribute = _rootType.GetCustomAttribute<ArgumentsControllerAttribute>()
			?? throw new MissingControllerException(_rootType);
			
		_tokens = argLine.Tokenize(_controllerAttribute.Separator);
		_rootProperties = _rootType.ExtractProperties();
	}

	/// <summary>
	/// Gets or sets the color of the application name in the usage text.
	/// </summary>
	public ConsoleColor ApplicationNameColor { get; set; } = ConsoleColor.Cyan;

	/// <summary>
	/// Gets or sets the color used to highlight required arguments in the usage text.
	/// </summary>
	public ConsoleColor RequiredArgumentsHighlightColor { get; set; } = ConsoleColor.Magenta;

	/// <summary>
	/// Gets or sets the color used to highlight optional arguments in the usage text.
	/// </summary>
	public ConsoleColor OptionalArgumentsHighlightColor { get; set; } = ConsoleColor.Green;

	private string? Current
	{
		get
		{
			if (_position >= _tokens.Count)
				return null;

			return _tokens[_position];
		}
	}

	private bool CurrentIsHelp => Current == "-h" || Current == "--help";

	private bool HelpExist => _tokens.Any(t => t.Contains("-h") || t.Contains("--help"));

	/// <summary>
	/// Parses the command-line arguments into an instance of type <typeparamref name="T"/>.
	/// </summary>
	/// <param name="helpMessageDisplayed">Indicates whether the help message was displayed.</param>
	/// <returns>An instance of type <typeparamref name="T"/> with the parsed arguments.</returns>
	public T Parse(out bool helpMessageDisplayed)
	{
		var instance = new T();
		_position = 0;

		helpMessageDisplayed = ParseObject(instance, _rootProperties);

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
	/// <returns>A <see cref="StringBuilder"/> containing the usage text.</returns>
	public StringBuilder GetUsageText() => _rootProperties.GetUsage();

	private bool ParseObject(object target, IEnumerable<PropertyBinding> propertyBindings)
	{
		foreach (var binding in propertyBindings)
		{
			if (Current == null)
			{
				ValidateRequirement(binding);
				SetupBooleanDefault(target, binding);
			}
			else if (CurrentIsHelp && _controllerAttribute.AutoHelpArgument)
			{
				DisplayUsage(propertyBindings);
				return true;
			}
			else if (binding.Matches(Current, _controllerAttribute.Separator))
			{
				if (ParseMatchedArgument(target, binding))
					return true;
			}
			else if (propertyBindings.Any(p => p.Matches(Current, _controllerAttribute.Separator)))
			{
				CheckIfIsBooleanAndAssignIt(target, binding);
			}
			else
			{
				binding.AssignValue(target, Current, HelpExist);
				_position++;
			}
		}

		return false;
	}

	private bool ParseMatchedArgument(object target, PropertyBinding binding)
	{
		// if is a nested argument
		if (binding.Property.PropertyType.IsNestedArgument())
		{
			// if help was displayed
			if (ParseNestedArgument(target, binding))
				return true;
		}
		else // Otherwise is a normal argument
			ParseValueToArgument(target, binding);

		return false;
	}

	private void ParseValueToArgument(object target, PropertyBinding binding)
	{
		var argument = Current!;
		_position++;

		// If current is null, means that our 'argument' is the last one
		if (Current == null)
		{
			// If has a separator (--arg=value), we must parse into two parts
			if (argument.Contains(_controllerAttribute.Separator))
			{
				var parts = argument.Split(_controllerAttribute.Separator, 2);
				binding.AssignValue(target, parts[1], HelpExist);
			}

			// No separator (--verbose), should be a boolean flag
			else if (binding.Property.PropertyType.IsBoolean())
				binding.AssignValue(target, true, HelpExist);

			// If no separator and it's not a boolean, throw error
			else
				throw new ArgumentException($"No value found for provided argument '{argument}'");
		}
		// If has a separator (--arg=value), we must parse into two parts
		else if (argument.Contains(_controllerAttribute.Separator))
		{
			var parts = argument.Split(_controllerAttribute.Separator, 2);
			binding.AssignValue(target, parts[1], HelpExist);
		}
		// Check if the current is a separator
		else if (Current.Contains(_controllerAttribute.Separator))
		{
			// Current can be the separator with the value (=value)
			if (Current.Length > 1)
			{
				var value = Current.Split(_controllerAttribute.Separator)[1];
				binding.AssignValue(target, value, HelpExist);
				_position++;
				return;
			}
			
			// Advance one to get the value
			_position++;

			if (Current == null)
				throw new ArgumentException($"No value found for provided argument '{argument}'");

			binding.AssignValue(target, Current, HelpExist);
			_position++;
		}
		// If matches with an argument, then is missing a value 
		else if (binding.Matches(Current, _controllerAttribute.Separator))
			throw new ArgumentException($"No value found for provided argument '{argument}'");
		else if (binding.Property.PropertyType.IsBoolean())
			binding.AssignValue(target, true, HelpExist);
		else
		{
			binding.AssignValue(target, Current, HelpExist);
			_position++;
		}
	}

	private bool ParseNestedArgument(object target, PropertyBinding binding)
	{
		var nestedInstance = Activator.CreateInstance(binding.Property.PropertyType)!;

		binding.AssignNested(target, nestedInstance);
		_position++;
		
		var helpDisplayed = ParseObject(nestedInstance, binding.Children);
		
		if (binding.WaitingForLazyExecution)
			binding.Execute(target, HelpExist);
			
		return helpDisplayed;
	}

	private void ValidateRequirement(PropertyBinding binding)
	{
		if (binding.ArgumentAttr.Required && !HelpExist)
			throw new ArgumentException($"Argument '{binding.GetName()}' is required.");
	}

	private void CheckIfIsBooleanAndAssignIt(object target, PropertyBinding binding)
	{
		if (binding.Property.PropertyType == typeof(bool) && binding.ArgumentAttr.InvertBoolean)
			binding.AssignValue(target, !binding.ArgumentAttr.InvertBoolean, HelpExist);
		else
			ValidateRequirement(binding);
	}

	private void DisplayUsage(IEnumerable<PropertyBinding> propertyBindings)
	{
		var requiredArguments = propertyBindings.Where(p => p.ArgumentAttr.Required);
		var optionalArguments = propertyBindings.Where(p => !p.ArgumentAttr.Required);
		var currentName = propertyBindings.Any(p => p.Parent != null)
						? propertyBindings.First().Parent!.GetName()
						: _controllerAttribute.Name;

		Console.WriteLine();
		Console.WriteLine("Usage:");
		Console.ForegroundColor = ApplicationNameColor;
		Console.Write($"  {currentName}");

		if (requiredArguments.Any())
		{
			Console.ForegroundColor = RequiredArgumentsHighlightColor;
			Console.Write(" <arguments>");
		}

		if (optionalArguments.Any())
		{
			Console.ForegroundColor = OptionalArgumentsHighlightColor;
			Console.Write(" [options]");
		}

		Console.WriteLine("\n");

		WriteRequiredUsage(requiredArguments);
		WriteOptionalUsage(optionalArguments);

		Console.ResetColor();
		Console.WriteLine();

		if (_controllerAttribute.AutoHelpArgument)
			Console.WriteLine("Use '-h, --help' with a command to see its details.");
	}

	private void WriteRequiredUsage(IEnumerable<PropertyBinding> required)
	{
		if (!required.Any())
			return;

		Console.ForegroundColor = RequiredArgumentsHighlightColor;
		Console.WriteLine("  Required arguments:");
		Console.ResetColor();

		foreach (var req in required)
			Console.WriteLine($"    {req.Usage()}");

		Console.WriteLine();
	}

	private void WriteOptionalUsage(IEnumerable<PropertyBinding> options)
	{
		if (!options.Any())
			return;

		Console.ForegroundColor = OptionalArgumentsHighlightColor;
		Console.WriteLine("  Available options:");
		Console.ResetColor();

		foreach (var opt in options)
			Console.WriteLine($"    {opt.Usage()}");

		Console.WriteLine();
	}

	private void SetupBooleanDefault(object target, PropertyBinding binding)
	{
		// If no argument is provided and the property is a boolean with inversion, set its default value
		if (binding.Property.PropertyType == typeof(bool) && binding.ArgumentAttr.InvertBoolean)
			binding.AssignValue(target, !binding.ArgumentAttr.InvertBoolean, HelpExist);
	}
}