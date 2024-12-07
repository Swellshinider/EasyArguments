using EasyArguments.Attributes;
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
        _ = typeof(T).GetCustomAttribute<ArgumentsControllerAttribute>()
            ?? throw new MissingArgumentsControllerAttributeException();

        var arguments = typeof(T).GetProperties()
            .Where(p => p.GetCustomAttribute<ArgumentAttribute>() != null)
            .Select(p => new Argument(p, p.GetCustomAttribute<ArgumentAttribute>()!))
            .ToList();

        for (int i = 0; i < _args.Length; i++)
        {
            var inputArgument = _args[i];

            if (inputArgument.Contains('='))
            {
                var split = inputArgument.Split('=');
                var key = split[0];
                var value = split[1];

                var foundArgument = arguments.Find(a => a.Attribute.ArgumentNames.Contains(key));

                if (foundArgument.Equals(default(Argument)))
                    throw new UnknownArgumentException(key);

                // Handle numeric values
                if (double.TryParse(value, out double number))
                {
                    var propertyType = foundArgument.PropertyInfo.PropertyType;

                    // Check for floating types
                    if (propertyType == typeof(float) || propertyType == typeof(double) || propertyType == typeof(decimal))
                    {
                        foundArgument.PropertyInfo.SetValue(result, Convert.ChangeType(number, propertyType));
                        continue;
                    }

                    // Check for integer types
                    if (propertyType == typeof(short) || propertyType == typeof(int) || propertyType == typeof(long))
                    {
                        // Check if the number can fit into the target integer type
                        if (Math.Abs(number % 1) > Tolerance)
                            throw new InvalidArgumentTypeException(key, propertyType);

                        var integerNumber = (long)number;

                        // Bounds checking based on target type
                        if (propertyType == typeof(short) && (integerNumber < short.MinValue || integerNumber > short.MaxValue) ||
                            propertyType == typeof(int) && (integerNumber < int.MinValue || integerNumber > int.MaxValue))
                            throw new InvalidArgumentTypeException(key, propertyType);

                        foundArgument.PropertyInfo.SetValue(result, Convert.ChangeType(integerNumber, propertyType));
                        continue;
                    }

                    throw new InvalidArgumentTypeException(key, propertyType);
                }
                else
                {
                    // Handle non-numeric values
                    try
                    {
                        foundArgument.PropertyInfo.SetValue(result, Convert.ChangeType(value, foundArgument.PropertyInfo.PropertyType));
                    }
                    catch (Exception)
                    {
                        throw new InvalidArgumentTypeException(key, foundArgument.PropertyInfo.PropertyType);
                    }
                }
            }
            else
            {
                var argumentNoSplit = arguments.Find(a => a.Attribute.ArgumentNames.Contains(inputArgument));

                if (argumentNoSplit.Equals(default(Argument)))
                    throw new UnknownArgumentException(inputArgument);

                var propInfo = argumentNoSplit.PropertyInfo;

                if (propInfo.PropertyType == typeof(Nullable<bool>))
                    propInfo.SetValue(result, (Nullable<bool>)true);
                else if (propInfo.PropertyType == typeof(bool))
                    propInfo.SetValue(result, true);
                else
                    throw new InvalidArgumentTypeException(inputArgument, typeof(bool));
            }
        }

        return result;
    }

    public IEnumerable<object?> Execute<T2>(T obj) where T2 : new()
    {
        var executableArguments = obj!.GetType().GetProperties()
            .Where(p => p.GetCustomAttribute<ArgumentExecutorAttribute<T2>>() != null)
            .Select(p => new ExecutableArgument<T2>(p, p.GetValue(obj), p.GetCustomAttribute<ArgumentExecutorAttribute<T2>>()!))
            .ToList();

        foreach (var execArg in executableArguments)
        {
            var executor = new T2();
            var method = executor.GetType().GetMethod(execArg.Attribute.ExecutorName) 
                ?? throw new MissingMethodException(execArg.Attribute.ExecutorName);

            yield return method.Invoke(executor, [execArg.Value]);
        }
    }
}