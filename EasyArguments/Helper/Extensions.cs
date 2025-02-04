using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using EasyArguments.Attributes;

namespace EasyArguments.Helper;

/// <summary>
/// Provides extension methods for common type conversions and checks.
/// </summary>
public static partial class Extensions
{
	/// <summary>
	/// A simple extension method to parse known boolean strings (true/false).
	/// </summary>
	public static bool ToBoolean(this string value) => value.Trim().ToLower() switch
	{
		"true" => true,
		"false" => false,
		_ => throw new ArgumentException($"Unable to interpret '{value}' as a valid boolean. Invalid argument type")
	};

	/// <summary>Returns true if <paramref name="t"/> is bool or nullable bool.</summary>
	public static bool IsBoolean(this Type t) => t == typeof(bool) || t == typeof(bool?);

	/// <summary>
	/// Tokenizes the input string based on the default regular expression.
	/// </summary>
	/// <param name="input">The input string to tokenize.</param>
	/// <returns>A list of tokens extracted from the input string.</returns>
	public static List<string> Tokenize(this string input) => Tokenize(input, SeparateValuesRegex());

	/// <summary>
	/// Tokenizes the input string based on the provided regular expression.
	/// </summary>
	/// <param name="input">The input string to tokenize.</param>
	/// <param name="matchingRegex">The regular expression used to match tokens.</param>
	/// <returns>A list of tokens extracted from the input string.</returns>
	public static List<string> Tokenize(this string input, Regex matchingRegex)
	{
		var tokens = new List<string>();
		var matches = matchingRegex.Matches(input);

		for (int i = 0; i < matches.Count; i++)
		{
			var match = matches[i];
			var token = match.Value;

			if (token.StartsWith('\"') && token.EndsWith('\"'))
				token = token[1..(token.Length - 1)];

			if (!string.IsNullOrWhiteSpace(token))
				tokens.Add(token);
		}

		return tokens;
	}
	
	internal static bool IsNestedArgument(this Type type) => type.IsClass && type != typeof(string);
	
	internal static IEnumerable<PropertyBinding> ExtractProperties(this Type targetType, PropertyBinding? parent = null)
	{
		foreach (var prop in targetType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
		{
			// Get the attribute from the property
			var argAttr = prop.GetCustomAttribute<ArgumentAttribute>();

			// Ignore if does not contain the attribute
			if (argAttr == null)
				continue;

			// If both ShortName and LongName are null, set LongName to property name with "--" prefix.
			if (string.IsNullOrWhiteSpace(argAttr.ShortName) && string.IsNullOrWhiteSpace(argAttr.LongName))
				argAttr.LongName = "--" + prop.Name.ToLowerInvariant();

			var boundProperty = new PropertyBinding(prop, argAttr, parent);

			// If this is a class and not string, we consider it as nested arguments
			if (prop.PropertyType.IsNestedArgument())
				boundProperty.Children = ExtractProperties(boundProperty.Property.PropertyType, boundProperty);

			yield return boundProperty;
		}
	}
	
	internal static StringBuilder GetUsage(this IEnumerable<PropertyBinding> propertyBindings)
	{
		var builder = new StringBuilder();
		var requiredArguments = propertyBindings.Where(p => p.ArgumentAttr.Required);
		var optionalArguments = propertyBindings.Where(p => !p.ArgumentAttr.Required);

		builder.AppendLine();

		WriteRequiredUsage(builder, requiredArguments);
		WriteOptionalUsage(builder, optionalArguments);

		builder.AppendLine();
		
		return builder;
	}

	private static void WriteRequiredUsage(StringBuilder builder, IEnumerable<PropertyBinding> required)
	{
		if (!required.Any())
			return;

		builder.AppendLine("  Required arguments:");
		Console.ResetColor();
		
		foreach (var req in required)
			builder.AppendLine($"    {req.Usage()}");
			
		builder.AppendLine();
	}
	
	private static void WriteOptionalUsage(StringBuilder builder, IEnumerable<PropertyBinding> options)
	{
		if (!options.Any())
			return;
		
		builder.AppendLine("  Available options:");
		Console.ResetColor();
		
		foreach (var opt in options)
			builder.AppendLine($"    {opt.Usage()}");
		
		builder.AppendLine();
	}

	/// <summary>
	/// Reference: https://regex101.com/r/eUTGpf/2
	/// </summary>
	[GeneratedRegex(@"(?:[/-]+?:[/-]+(?<key>\S+?))|(?:""[/-]+(?<key>.+?)"")|(?:'[/-]+(?<key>.+?)')|(?:""(?<value>.+?)"")|(?:'(?<value>.+?)')|(?<value>\S+)", RegexOptions.Compiled)]
	private static partial Regex SeparateValuesRegex();
}