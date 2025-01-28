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
	private readonly ArgumentsControllerAttribute _controllerAttribute;
	private List<PropertyBinding>? _rootBindings;

	/// <summary>
	/// Create instance of <see cref="ArgumentsController{T}"/>
	/// </summary>
	/// <exception cref="MissingControllerException">
	/// The exception that is thrown when a class does not have the required <see cref="ArgumentsControllerAttribute"/> attribute.
	/// </exception>
	public ArgumentsController()
	{
		_rootType = typeof(T);
		_controllerAttribute = _rootType.GetCustomAttribute<ArgumentsControllerAttribute>()
			?? throw new MissingControllerException(_rootType);
	}

	private List<PropertyBinding> RootBindings
		=> _rootBindings ??= [.. ExtractProperties(_rootType)];

	/// <summary>
	/// Parses the given command-line arguments into a new instance of <typeparamref name="T"/>.
	/// </summary>
	/// <param name="args">The raw command-line arguments.</param>
	/// <returns>A new instance of <typeparamref name="T"/> with properties populated from <paramref name="args"/>.</returns>
	public T Parse(string[] args)
	{
		var instance = new T();

		int i = 0;
		InitializeBooleans(instance, RootBindings);
		_ = ParseObject(instance, RootBindings, args, ref i);

		return instance;
	}
	
	private static void InitializeBooleans(object target, List<PropertyBinding> propertyBindings) 
	{
		foreach (var binding in propertyBindings)
		{
			var propType = binding.Property.PropertyType;
			
			if (propType.IsBoolean())
			{
				// If InvertBoolean is true, default to true; otherwise false.
				var defaultValue = binding.ArgumentAttr.InvertBoolean;

				if (propType == typeof(bool?))
					binding.Property.SetValue(target, (bool?)defaultValue);
				else
					binding.Property.SetValue(target, defaultValue);
			}
			
			if (binding.Children.Count > 0) 
			{
				var subInstance = Activator.CreateInstance(binding.Property.PropertyType)!;
				binding.Property.SetValue(target, subInstance);
				InitializeBooleans(subInstance, binding.Children);
			}
		}
	}

	/// <summary>
	/// Recursively parses command-line arguments for the current "level" into <paramref name="target"/>.
	/// It advances <paramref name="index"/> as it consumes arguments.
	/// </summary>
	private bool ParseObject(object target, List<PropertyBinding> propertyBindings, string[] args, ref int index)
	{
		while (index < args.Length)
		{
			var currentArg = args[index];

			// Check if user wants help at this level
			if (currentArg == "-h" || currentArg == "--help")
			{
				Console.WriteLine(GetUsage(propertyBindings));
				return true;
			}

			// Find a matching property in this level
			var binding = propertyBindings.FirstOrDefault(pb => pb.Matches(currentArg))
				?? throw new ArgumentException($"Unknown argument '{currentArg}'.{(_controllerAttribute.AutoHelpArgument ? " Please use -h or --help for usage information." : "")}");

			var isBool = binding.Property.PropertyType.IsBoolean();

			// If the arg includes an '=' for inline values, e.g. --url=http://...
			string? valuePart = null;
			var eqIdx = currentArg.IndexOf(_controllerAttribute.Separator);

			if (eqIdx >= 0)
				valuePart = currentArg[(eqIdx + 1)..];

			// Move index forward since we recognized this argument
			index++;

			if (binding.Children.Count != 0)
			{
				// Create the sub-instance
				var subInstance = Activator.CreateInstance(binding.Property.PropertyType)!;
				binding.Property.SetValue(target, subInstance);

				// Recurse
				bool helpPrinted = ParseObject(subInstance, binding.Children, args, ref index);

				if (helpPrinted)
					return true;

				continue;
			}

			if (!isBool && valuePart == null && index < args.Length)
			{
				var nextArg = args[index];
				if (!nextArg.StartsWith('-'))
				{
					valuePart = nextArg;
					index++;
				}
			}

			// Convert and set
			object? convertedVal = ConvertValue(binding, valuePart);
			binding.Property.SetValue(target, convertedVal);
		}

		return false;
	}

	/// <summary>
	/// Builds a usage string for the current object.
	/// </summary>
	public string GetUsageText() => GetUsage(RootBindings);

	private string GetUsage(List<PropertyBinding> propertyBindings)
	{
		var sb = new StringBuilder();
		sb.AppendLine("Usage: \n");

		// For each property at this level
		foreach (var pb in propertyBindings)
			sb.AppendLine(pb.Usage());

		if (_controllerAttribute.AutoHelpArgument)
			sb.AppendLine($"{"-h, --help".PadRight(PropertyBinding.PAD_SIZE)}Show this help message.\n");

		return sb.ToString();
	}

	private static IEnumerable<PropertyBinding> ExtractProperties(Type targetType, PropertyBinding? parent = null)
	{
		foreach (var prop in targetType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
		{
			var argAttr = prop.GetCustomAttribute<ArgumentAttribute>();

			if (argAttr == null)
				continue;

			// If both ShortName and LongName are null, default to lowercase property name with "--" prefix.
			if (string.IsNullOrWhiteSpace(argAttr.ShortName) && string.IsNullOrWhiteSpace(argAttr.LongName))
				argAttr.LongName = "--" + prop.Name.ToLowerInvariant();

			var propBind = new PropertyBinding(prop, argAttr, parent);

			// If this is a class and not string, we consider it as nested arguments
			if (prop.PropertyType.IsClass && prop.PropertyType != typeof(string))
				propBind.Children.AddRange(ExtractProperties(propBind.Property.PropertyType, propBind));

			yield return propBind;
		}
	}

	/// <summary>
	/// Converts the given <paramref name="valuePart"/> into the correct type for <paramref name="binding"/>.
	/// </summary>
	private static object? ConvertValue(PropertyBinding binding, string? valuePart)
	{
		var propType = binding.Property.PropertyType;
		var argAttr = binding.ArgumentAttr;

		// If property is bool or bool?, no value means just set it to "true" or "false" if InvertBoolean is set.
		if (propType.IsBoolean())
		{
			bool boolValue;
			if (string.IsNullOrWhiteSpace(valuePart))
			{
				// No explicit value means we toggle based on InvertBoolean
				boolValue = !argAttr.InvertBoolean;
			}
			else
			{
				// parse the explicit boolean string (true/false/1/0/yes/no, etc.)
				boolValue = valuePart.ToBoolean();
				if (argAttr.InvertBoolean)
				{
					boolValue = !boolValue;
				}
			}

			// If property is bool? (nullable), box the bool into bool? 
			if (propType == typeof(bool?))
				return (bool?)boolValue;
			else
				return boolValue;
		}

		// If it’s a string? property, just set it directly:
		if (propType == typeof(string))
		{
			return valuePart;
		}

		// If no value was provided but it's not a boolean -> might be an error
		if (string.IsNullOrEmpty(valuePart))
		{
			return null; // Or throw an exception if required
		}

		// Attempt to convert to other known types (int, float, enum, etc.) 
		// For simplicity, we'll just let Convert.ChangeType handle primitives.
		try
		{
			return Convert.ChangeType(valuePart, Nullable.GetUnderlyingType(propType) ?? propType);
		}
		catch (Exception ex)
		{
			throw new ArgumentException($"Failed to convert value '{valuePart}' to type {propType.Name}: {ex.Message}", ex);
		}
	}
}