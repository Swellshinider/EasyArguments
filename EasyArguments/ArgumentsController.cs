using EasyArguments.Attributes;
using EasyArguments.Exceptions;
using System.Reflection;

namespace EasyArguments;

/// <summary>
/// Provides utilities for parsing command-line arguments into strongly-typed objects.
/// </summary>
public static class ArgumentsController
{
	/// <summary>
	/// Gets or sets a value indicating whether errors should be redirected to the console.
	/// </summary>
	public static bool RedirectErrorToConsole { get; set; } = false;

	/// <summary>
	/// Parses the specified command-line arguments into an instance of the given type.
	/// </summary>
	/// <typeparam name="T">The type into which arguments should be parsed. Must have a parameterless constructor.</typeparam>
	/// <param name="args">The collection of command-line arguments.</param>
	/// <returns>An instance of <typeparamref name="T"/> populated with the parsed arguments.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="args"/> is null.</exception>
	/// <exception cref="MissingArgumentsControllerAttributeException">Thrown when the target type does not have the required <see cref="ArgumentsControllerAttribute"/>.</exception>
	/// <exception cref="ArgumentException">Thrown when parsing fails or required arguments are missing.</exception>
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
		var hasHelp = args.Any(a => a.Trim().Equals("-h") || a.Trim().Equals("--help"));
		var separator = controllerAttribute.Separator;
		var arguments = type.GetArguments();

		if (respectOrder)
		{
			var argHelper = ParseWithOrder(result, arguments, [.. args], separator);

			if (!autoHelpArgument || !hasHelp)
				return result;

			if (argHelper != null)
				Console.WriteLine(argHelper.ToString());
			else
				arguments.ForEach(Console.WriteLine);
		}
		else
			throw new ArgumentException($"Parsing arguments without order is not implemented yet.");

		return result;
	}

	/// <summary>
	/// Executes static methods specified by the <see cref="ExecuteAttribute"/> applied to properties of the given object.
	/// The method receives the current value of the property, processes it, and optionally assigns the result back to the property.
	/// They are executed returned as an enumerable.
	/// </summary>
	/// <typeparam name="T">The type of the object containing the decorated properties.</typeparam>
	/// <param name="obj">The object whose properties are processed.</param>
	/// <returns>An enumerable of results from the executed methods.</returns>
	public static IEnumerable<object?> Execute<T>(T obj)
	{
		var type = typeof(T);
		var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

		foreach (var prop in properties)
		{
			var executorAttrib = prop.GetCustomAttribute<ExecuteAttribute>();

			if (executorAttrib == null)
				continue;

			var classType = executorAttrib.ClassType;
			var methodName = executorAttrib.MethodName;

			// Retrieve the static method info
			var method = classType.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);

			if (method == null)
			{
				LaunchError($"The method '{methodName}' was not found in the class '{classType.FullName}'.", true);
				yield break;
			}

			var currentValue = prop.GetValue(obj);
			var methodParameters = method.GetParameters();
			object? executionResult;

			// Validate the method signature
			if (methodParameters.Length == 0)
			{
				// Invoke method with no parameters
				executionResult = method.Invoke(null, null);
			}
			else if (methodParameters.Length == 1 && methodParameters[0].ParameterType.IsAssignableFrom(prop.PropertyType))
			{
				// Invoke method with the current property value as the parameter
				executionResult = method.Invoke(null, [currentValue]);
			}
			else
			{
				// Throw an exception if the method signature is invalid
				LaunchError(@$"The method '{methodName}' in class '{classType.FullName}' must either have no parameters 
					or a single parameter of type '{prop.PropertyType}'.", true);
				yield break;
			}

			// Optionally assign the result back to the property if allowed
			if (executorAttrib.AssignResultToProperty &&
				prop.PropertyType.IsAssignableFrom(method.ReturnType) &&
				prop.CanWrite)
			{
				prop.SetValue(obj, executionResult);
			}

			yield return executionResult;
		}
	}

	private static Argument? ParseWithOrder<T>(T result, List<Argument> arguments, List<string> args, char separator, Argument? lastArgument = null) where T : new()
	{
		Argument? helper = null;

		foreach (var arg in args)
		{
			if (!arguments.Where(a => arg.ArgumentNameEquals(a.Attribute.ShortName, a.Attribute.LongName)).Any())
			{
				LaunchError($"Provided argument '{arg}' does not exist.");
				return null;
			}
		}

		foreach (var argument in arguments)
		{
			if (args.Count == 0)
			{
				if (argument.Attribute.Required)
					LaunchError($"Required argument '{argument.GetName()}' is missing.");

				continue;
			}

			if (args[0] == "-h" || args[0] == "--help")
			{
				if (argument.Parent == null)
					helper = lastArgument;
				else
					helper = argument.Parent;
				args.RemoveAt(0);
				continue;
			}

			var currentArg = args[0];
			var shortName = argument.Attribute.ShortName;
			var longName = argument.Attribute.LongName;

			if (argument.PropertyInfo.PropertyType.IsClass &&
				argument.PropertyInfo.PropertyType != typeof(string))
			{
				if (!currentArg.ArgumentNameEquals(shortName, longName))
				{
					if (argument.Attribute.Required)
						LaunchError($"Required argument '{argument.GetName()}' is missing.");

					continue;
				}

				object? innerResult;
				try
				{
					innerResult = argument.PropertyInfo.PropertyType.Assembly.CreateInstance(argument.PropertyInfo.PropertyType.FullName!);
				}
				catch (MissingMethodException)
				{
					LaunchError($"Public constructor on type '{argument.PropertyInfo.PropertyType.FullName}' not found!");
					return helper;
				}

				args.RemoveAt(0);
				argument.PropertyInfo.SetValue(result, innerResult);
				var subParseResult = ParseWithOrder(innerResult, argument.Children, args, separator, argument);

				if (subParseResult != null)
					helper = subParseResult;

				continue;
			}

			// The currentArg fits perfectly with the name, so it's a boolean
			// Otherwise no value was found to the provided argument
			if (currentArg.ArgumentNameEquals(shortName, longName))
			{
				if (argument.PropertyInfo.PropertyType == typeof(bool) ||
					argument.PropertyInfo.PropertyType == typeof(bool?))
				{
					argument.PropertyInfo.SetValue(result, !argument.Attribute.InvertBoolean);
					args.RemoveAt(0);
					continue;
				}
				else
				{
					LaunchError($"No value found for the provided argument '{currentArg}'.");
					return helper;
				}
			}

			var parts = currentArg.Split(separator, 2, StringSplitOptions.TrimEntries);

			if (parts.Length != 2 && argument.Attribute.Required)
			{
				LaunchError($"Argument '{argument.GetName()}' is required.");
				continue;
			}

			// Ex: parts = ["-o", "test"]
			// If "-o" does not match the current argument, but the argument is required, throw error
			if (!parts[0].ArgumentNameEquals(shortName, longName))
			{
				if (argument.Attribute.Required)
					LaunchError($"Provided argument '{currentArg}' does not match the required order.");

				continue;
			}

			var value = parts[1];
			object? convertedValue;

			try
			{
				convertedValue = Convert.ChangeType(value, argument.PropertyInfo.PropertyType);
			}
			catch (FormatException)
			{
				if (!argument.Attribute.Required)
					continue;

				var errorMessage = $"Argument {argument.GetName()} requires type '{argument.PropertyInfo.PropertyType.Name}', received: '{value}'.";
				LaunchError(errorMessage);
				return helper;
			}

			argument.PropertyInfo.SetValue(result, convertedValue);
			args.RemoveAt(0);
		}
		return helper;
	}

	private static bool ArgumentNameEquals(this string argv, string? shortName, string? longName)
	{
		return argv.Equals(shortName ?? "", StringComparison.CurrentCultureIgnoreCase) ||
			   argv.Equals(longName ?? "", StringComparison.CurrentCultureIgnoreCase);
	}

	private static void LaunchError(string message, bool operationEx = false)
	{
		if (RedirectErrorToConsole)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.Error.WriteLine(message);
			Console.ResetColor();
			return;
		}

		if (operationEx)
			throw new InvalidOperationException(message);
		else
			throw new ArgumentException(message);
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
				var commandArgument = new Argument(parent, prop, argumentAttrib);

				if (parent != null)
					parent.Children.Add(commandArgument);
				else
					arguments.Add(commandArgument);

				arguments.AddRange(prop.PropertyType.GetArguments(commandArgument));
				continue;
			}

			var argument = new Argument(parent, prop, argumentAttrib);

			if (parent != null)
				parent.Children.Add(argument);
			else
				arguments.Add(argument);
		}

		return arguments;
	}
}