﻿using EasyArguments.Attributes;
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

		foreach (var binding in propertyBindings)
		{
			if (Current == null)
			{
				ValidateRequirement(binding);
				SetupBooleanDefault(target, binding);
			}
			else if (IsHelp && _controllerAttribute.AutoHelpArgument)
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
				binding.AssignValue(target, Current);
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
				binding.AssignValue(target, parts[1]);
			}

			// No separator (--verbose), should be a boolean flag
			else if (binding.Property.PropertyType.IsBoolean())
				binding.AssignValue(target, true);

			// If no separator and it's not a boolean, throw error
			else
				throw new ArgumentException($"No value found for provided argument '{argument}'");
		}
		// Check if the current is a separator
		else if (Current.Contains(_controllerAttribute.Separator))
		{
			// Advance one to get the value
			_position++;

			if (Current == null)
				throw new ArgumentException($"No value found for provided argument '{argument}'");

			binding.AssignValue(target, Current);
			_position++;
		}
		// If matches with an argument, then is missing a value 
		else if (binding.Matches(Current, _controllerAttribute.Separator))
			throw new ArgumentException($"No value found for provided argument '{argument}'");
		else if (binding.Property.PropertyType.IsBoolean())
			binding.AssignValue(target, true);
		else
		{
			binding.AssignValue(target, Current);
			_position++;
		}
	}

	private bool ParseNestedArgument(object target, PropertyBinding binding)
	{
		var nestedInstance = Activator.CreateInstance(binding.Property.PropertyType)!;

		binding.AssignValue(target, nestedInstance);

		_position++;
		return ParseObject(nestedInstance);
	}

	private static void ValidateRequirement(PropertyBinding binding)
	{
		if (binding.ArgumentAttr.Required)
			throw new ArgumentException($"Argument '{binding.GetName()}' is required.");
	}
	
	private static void CheckIfIsBooleanAndAssignIt(object target, PropertyBinding binding)
	{
		if (binding.Property.PropertyType == typeof(bool) && binding.ArgumentAttr.InvertBoolean)
			binding.AssignValue(target, !binding.ArgumentAttr.InvertBoolean);
	}

	private static void DisplayUsage(IEnumerable<PropertyBinding> propertyBindings)
	{
		foreach (var prop in propertyBindings)
			Console.WriteLine(prop.Usage());
	}

	private static void SetupBooleanDefault(object target, PropertyBinding binding)
	{
		// If no argument is provided and the property is a boolean with inversion, set its default value
		if (binding.Property.PropertyType == typeof(bool) && binding.ArgumentAttr.InvertBoolean)
			binding.AssignValue(target, !binding.ArgumentAttr.InvertBoolean);
	}
}