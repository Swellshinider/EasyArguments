using EasyArguments.Attributes;
using System.Reflection;
using System.Linq;
using EasyArguments.Exceptions;
using System;

namespace EasyArguments;

public class ArgumentParser<T> where T : new()
{
    private readonly string[] _args;
    private const double Tolerance = 1e-10;

    public ArgumentParser(string[] args)
    {
        _args = args;
    }

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
}
