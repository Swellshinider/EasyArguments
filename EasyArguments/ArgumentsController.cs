using EasyArguments.Attributes;
using EasyArguments.Exceptions;
using System.Collections;
using System.Reflection;
using System.Text;

namespace EasyArguments;

public static class ArgumentsController
{
	public static T Parse<T>(IEnumerable<string> args) where T : new()
	{
		ArgumentNullException.ThrowIfNull(args, nameof(args));

		var result = new T();
		var type = typeof(T);

		// Fetch controller-level attributes
		var controllerAttribute = type.GetCustomAttribute<ArgumentsControllerAttribute>()
			?? throw new MissingArgumentsControllerAttributeException(type);

		if (!args.Any())
			return result;

		var respectOrder = controllerAttribute.RespectOrder;
		var autoHelpArgument = controllerAttribute.AutoHelpArgument;
		var separator = controllerAttribute.Separator;
		var arguments = type.GetArguments();

		// args = "-v" "--help" "-o=caminho_path"
		if (respectOrder)
			ParseWithOrder(result, type, arguments, [.. args], separator);
		else
		{

		}

		return result;
	}

	private static void ParseWithOrder<T>(T result, Type type, List<Argument> arguments, List<string> args, char separator) where T : new()
	{
		foreach (var argument in arguments)
		{
			if (args.Count == 0)
			{
				if (argument.Attribute.Required)
					throw new ArgumentException($"Required argument '{argument.Attribute.ShortName ?? argument.Attribute.LongName}' is missing.");

				continue;
			}

			// Current argument string
			var currentArg = args[0];
			string? value = null;

			// TODO: handle separator as '\0' and separator as ' '
			var parts = currentArg.Split(separator, 2);

			if (parts[0].Equals(argument.Attribute.ShortName ?? "", StringComparison.CurrentCultureIgnoreCase) ||
				parts[0].Equals(argument.Attribute.LongName ?? "", StringComparison.CurrentCultureIgnoreCase))
			{
				value = parts.Length > 1 ? parts[1] : null;
				args.RemoveAt(0);
			}
			else
			{
				if (argument.PropertyInfo.PropertyType == typeof(bool) &&
					currentArg.Equals(argument.Attribute.ShortName ?? "", StringComparison.CurrentCultureIgnoreCase) ||
					currentArg.Equals(argument.Attribute.LongName ?? "", StringComparison.CurrentCultureIgnoreCase))
					value = "True";
			}

			if (value != null)
			{
				var propertyType = argument.PropertyInfo.PropertyType;
				var convertedValue = Convert.ChangeType(value, propertyType);
				argument.PropertyInfo.SetValue(result, convertedValue);
			}
			else if (argument.Attribute.Required)
				throw new ArgumentException($"Required argument '{argument.Attribute.ShortName ?? argument.Attribute.LongName}' is missing a value.");
		}
	}

	private static List<Argument> GetArguments(this Type type, Argument? parent = null)
	{
		var arguments = new List<Argument>();
		_ = type.GetCustomAttribute<ArgumentsControllerAttribute>()
			?? throw new MissingArgumentsControllerAttributeException(type);

		var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

		foreach (var prop in properties)
		{
			var argumentAttrib = prop.GetCustomAttribute<ArgumentAttribute>();

			if (argumentAttrib == null)
				continue;

			if (prop.PropertyType.IsClass && prop.PropertyType != typeof(string))
			{
				var commandArgument = new Argument(prop, argumentAttrib);

				if (parent != null)
					parent.Children.Add(commandArgument);
				else
					arguments.Add(commandArgument);

				arguments.AddRange(prop.PropertyType.GetArguments(commandArgument));
				continue;
			}

			var argument = new Argument(prop, argumentAttrib);

			if (parent != null)
				parent.Children.Add(argument);
			else
				arguments.Add(argument);
		}

		return arguments;
	}
}