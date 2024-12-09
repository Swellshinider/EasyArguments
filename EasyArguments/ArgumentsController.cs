using EasyArguments.Attributes;
using EasyArguments.Enums;
using EasyArguments.Exceptions;
using System.Linq;
using System.Reflection;

namespace EasyArguments;

/// <summary>
/// Parses command-line arguments and maps them to the properties of a specified class.
/// </summary>
/// <typeparam name="T">
/// The type of the class to which arguments will be mapped. 
/// Must have a parameterless constructor and be annotated with <see cref="ArgumentsControllerAttribute"/>.
/// </typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="ArgumentsController{T}"/> class with the specified arguments.
/// </remarks>
/// <param name="args">An array of command-line arguments to parse.</param>
public class ArgumentsController<T>(string[] args) where T : new()
{
    private readonly string[] _args = args;
    private const double Tolerance = 1e-10;

    /// <summary>
    /// Parses the arguments and maps them to the properties of the type <typeparamref name="T"/>.
    /// </summary>
    /// <returns>
    /// An instance of <typeparamref name="T"/> with its properties set according to the parsed arguments.
    /// </returns>
    /// <exception cref="MissingArgumentsControllerAttributeException">
    /// Thrown when the type <typeparamref name="T"/> is not decorated with <see cref="ArgumentsControllerAttribute"/>.
    /// </exception>
    /// <exception cref="UnknownArgumentException">
    /// Thrown when an argument is provided that does not match any property in <typeparamref name="T"/>.
    /// </exception>
    /// <exception cref="InvalidArgumentTypeException">
    /// Thrown when an argument cannot be converted to the required type for a property in <typeparamref name="T"/>.
    /// </exception>
    public T Parse()
    {
        var result = new T();
        var controllerAttribute = typeof(T).GetCustomAttribute<ArgumentsControllerAttribute>()
            ?? throw new MissingArgumentsControllerAttributeException();

        var respectOrder = controllerAttribute.RespectOrder;

        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        var arguments = properties
            .Where(p => p.GetCustomAttribute<ArgumentAttribute>() != null)
            .Select(p => new Argument(p, p.GetCustomAttribute<ArgumentAttribute>()!))
            .ToList();

        if (respectOrder)
        {
            // If we are respecting order, we assume the arguments come in the same order as the properties.
            // Here we check if the user provided more arguments than we have properties or fewer.
            if (_args.Length > arguments.Count)
            {
                // More arguments than expected:
                // Throw UnknownArgumentException for the extra arguments.
                // This behavior can be adjusted as needed.
                throw new UnknownArgumentException(_args[arguments.Count]);
            }

            for (int i = 0; i < arguments.Count; i++)
            {
                var foundArgument = arguments[i];
                // If the user provided fewer arguments than the number of properties:
                // If 'Required' is true and argument not provided, throw an exception.
                // If optional, you can skip. For simplicity, let's assume everything is required unless proven otherwise.
                if (i >= _args.Length)
                {
                    // Check if required
                    bool isRequired = IsArgumentRequired(foundArgument);
                    if (isRequired)
                        throw new MissingRequiredArgumentException(string.Join(", ", foundArgument.Attribute.ArgumentNames));
                    // If not required and no argument given, continue
                    continue;
                }

                var inputArgument = _args[i];

                if (inputArgument.Contains('='))
                {
                    var split = inputArgument.Split('=', 2);
                    // Use a limit of 2 so that 'value' can contain '=' if needed
                    var key = split[0];
                    var value = split.Length > 1 ? split[1] : string.Empty;

                    // Check if the key matches one of the allowed argument names
                    if (!foundArgument.Attribute.ArgumentNames.Contains(key))
                        throw new IncorrectArgumentOrderException(key, string.Join(", ", foundArgument.Attribute.ArgumentNames));

                    SetPropertyValue(result, foundArgument, key, value);
                }
                else
                {
                    // No '=' means it's likely a boolean flag
                    if (!foundArgument.Attribute.ArgumentNames.Contains(inputArgument))
                        throw new UnknownArgumentException(inputArgument);

                    var propInfo = foundArgument.PropertyInfo;
                    if (propInfo.PropertyType == typeof(bool) || propInfo.PropertyType == typeof(bool?))
                    {
                        propInfo.SetValue(result, true);
                    }
                    else
                    {
                        // If no '=' and not boolean, then it's invalid format for ordered mode
                        throw new InvalidArgumentTypeException(inputArgument, propInfo.PropertyType);
                    }
                }
            }
        }
        else
        {
            // Original logic for non-ordered parsing
            for (int i = 0; i < _args.Length; i++)
            {
                var inputArgument = _args[i];

                if (inputArgument.Contains('='))
                {
                    var split = inputArgument.Split('=');
                    var key = split[0];
                    var value = split.Length > 1 ? split[1] : string.Empty;

                    var foundArgument = arguments.Find(a => a.Attribute.ArgumentNames.Contains(key));

                    if (foundArgument.Equals(default(Argument)))
                        throw new UnknownArgumentException(key);

                    SetPropertyValue(result, foundArgument, key, value);
                }
                else
                {
                    var argumentNoSplit = arguments.Find(a => a.Attribute.ArgumentNames.Contains(inputArgument));

                    if (argumentNoSplit.Equals(default(Argument)))
                        throw new UnknownArgumentException(inputArgument);

                    var propInfo = argumentNoSplit.PropertyInfo;

                    if (propInfo.PropertyType == typeof(Nullable<bool>))
                        propInfo.SetValue(result, (bool?)true);
                    else if (propInfo.PropertyType == typeof(bool))
                        propInfo.SetValue(result, true);
                    else
                        throw new InvalidArgumentTypeException(inputArgument, typeof(bool));
                }
            }
        }

        // Validate required arguments if needed (if you rely on auto detect or have a required property)
        foreach (var arg in arguments)
        {
            if (IsArgumentRequired(arg) && arg.PropertyInfo.GetValue(result) == null)
                throw new MissingRequiredArgumentException(string.Join(", ", arg.Attribute.ArgumentNames));
        }

        return result;
    }

    private static bool IsArgumentRequired(Argument argument)
    {
        // Determine if an argument is required based on:
        // 1. The `argument.Attribute.Required` property.
        // 2. The presence of a default value on the property (if Required == AutoDetect).
        // Adjust logic as necessary.
        if (argument.Attribute.Required == ArgumentRequirement.Required)
            return true;

        if (argument.Attribute.Required == ArgumentRequirement.Optional)
            return false;

        // AutoDetect logic:
        // If property has a default value (for example, it's a nullable or has a default), consider it optional.
        // Otherwise, required.
        var property = argument.PropertyInfo;
        if ((property.PropertyType.IsValueType && Nullable.GetUnderlyingType(property.PropertyType) == null) && property.GetValue(Activator.CreateInstance(property.DeclaringType!)) == null)
        {
            // Non-nullable value type with no default => required
            return true;
        }

        // For reference types or nullable value types without a set default, treat as required if no default.
        // This logic can be adjusted based on your needs.
        // To truly detect a default, you might rely on `DefaultValueAttribute` or similar.

        return false;
    }

    private static void SetPropertyValue(T result, Argument foundArgument, string key, string value)
    {
        var propertyType = foundArgument.PropertyInfo.PropertyType;

        // Handle numeric values first
        if (double.TryParse(value, out double number))
        {
            // Floating types
            if (propertyType == typeof(float) || propertyType == typeof(double) || propertyType == typeof(decimal))
            {
                foundArgument.PropertyInfo.SetValue(result, Convert.ChangeType(number, propertyType));
                return;
            }

            // Integer types
            if (propertyType == typeof(short) || propertyType == typeof(int) || propertyType == typeof(long))
            {
                if (Math.Abs(number % 1) > Tolerance)
                    throw new InvalidArgumentTypeException(key, propertyType);

                var integerNumber = (long)number;

                // Bounds checking
                if ((propertyType == typeof(short) && (integerNumber < short.MinValue || integerNumber > short.MaxValue)) ||
                    (propertyType == typeof(int) && (integerNumber < int.MinValue || integerNumber > int.MaxValue)))
                    throw new InvalidArgumentTypeException(key, propertyType);

                foundArgument.PropertyInfo.SetValue(result, Convert.ChangeType(integerNumber, propertyType));
                return;
            }

            throw new InvalidArgumentTypeException(key, propertyType);
        }
        else
        {
            // Non-numeric values
            try
            {
                foundArgument.PropertyInfo.SetValue(result, Convert.ChangeType(value, propertyType));
            }
            catch (Exception)
            {
                throw new InvalidArgumentTypeException(key, propertyType);
            }
        }
    }

    /// <summary>
    /// Executes the methods annotated with <see cref="ArgumentExecutorAttribute{T2}"/> on the provided object.
    /// </summary>
    /// <typeparam name="T2">
    /// The type of the class containing the methods to execute.
    /// </typeparam>
    /// <param name="obj">
    /// Object with properties annotated with <see cref="ArgumentExecutorAttribute{T2}"/>.
    /// </param>
    /// <returns>
    /// Returns the results of the executed methods.
    /// </returns>
    /// <exception cref="MissingMethodException">
    /// Thrown when a method annotated with <see cref="ArgumentExecutorAttribute{T2}"/> is not found.
    /// </exception>
    public IEnumerable<object?> Execute<T2>(T obj) where T2 : new()
    {
        var executableArguments = obj!.GetType().GetProperties()
            .Where(p => p.GetCustomAttribute<ArgumentExecutorAttribute<T2>>() != null)
            .Select(p => new ExecutableArgument<T2>(p, p.GetValue(obj), p.GetCustomAttribute<ArgumentExecutorAttribute<T2>>()!))
            .ToList();

        foreach (var execArg in executableArguments)
        {
            var method = execArg.Attribute.Instance!.GetType().GetMethod(execArg.Attribute.ExecutorName) 
                ?? throw new MissingMethodException(execArg.Attribute.ExecutorName);
            
            yield return method.Invoke(execArg.Attribute.Instance, [execArg.Value]);
        }
    }
}