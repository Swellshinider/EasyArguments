using EasyArguments.Attributes;
using EasyArguments.Enums;
using EasyArguments.Exceptions;
using System.Reflection;

namespace EasyArguments;

/// <summary>
/// Parses command-line arguments and maps them to the properties of a specified class.
/// </summary>
public class ArgumentsController
{
	private readonly string[] _args;

	public ArgumentsController(string[] argv)
	{
		_args = argv;
	}

	public T Parse<T>() where T : new()
	{
		var result = new T();

		// Fetch controller-level attributes
		var controllerAttribute = typeof(T).GetCustomAttribute<ArgumentsControllerAttribute>()
			?? throw new MissingArgumentsControllerAttributeException();

		var respectOrder = controllerAttribute.RespectOrder;
		var autoHelpArgument = controllerAttribute.AutoHelpArgument;
		var separators = controllerAttribute.Separators;

		// Fetch properties with ArgumentAttribute
		var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
			.Where(p => p.GetCustomAttribute<ArgumentAttribute>() != null)
			.Select(p => new Argument(p, p.GetCustomAttribute<ArgumentAttribute>()!))
			.ToList();

		if (autoHelpArgument && _args.Any(arg => arg == "-h" || arg == "--help"))
		{
			PrintHelp(properties);
			Environment.Exit(0);
		}

		foreach (var arg in _args)
		{
			// Split the argument into key-value pairs based on separators
			var separator = GetSeparator(arg, separators);
			if (separator == null) continue;

			var parts = arg.Split([separator], 2, StringSplitOptions.RemoveEmptyEntries);
			if (parts.Length < 2) continue;

			var key = parts[0].TrimStart('-');
			var value = parts[1];

			// Find the matching property
			var property = properties.FirstOrDefault(p => p.Attribute.ArgumentNames.Contains(key));
			if (property.Equals(default)) continue;

			// Convert and assign the value to the property
			var propertyType = property.PropertyInfo.PropertyType;
			var convertedValue = Convert.ChangeType(value, propertyType);
			property.PropertyInfo.SetValue(result, convertedValue);
		}

		foreach (var property in properties)
		{
			if (property.Attribute.Required && property.PropertyInfo.GetValue(result) == null)
			{
				throw new ArgumentException($"Missing required argument: {string.Join(", ", property.Attribute.ArgumentNames)}");
			}
		}

		return result;
	}

	private static string? GetSeparator(string arg, SeparatorTypes separators)
	{
		if (separators.HasFlag(SeparatorTypes.Equals) && arg.Contains('=')) return "=";
		if (separators.HasFlag(SeparatorTypes.Space) && arg.Contains(' ')) return " ";
		if (separators.HasFlag(SeparatorTypes.Arrow) && arg.Contains("->")) return "->";
		if (separators.HasFlag(SeparatorTypes.Dot) && arg.Contains('.')) return ".";
		return null;
	}

	private static void PrintHelp(IEnumerable<Argument> properties)
	{
		Console.WriteLine("Available arguments:");
		foreach (var property in properties)
		{
			var names = string.Join(", ", property.Attribute.ArgumentNames);
			Console.WriteLine($"{names}: {property.Attribute.Description} (Required: {property.Attribute.Required})");
		}
	}
}