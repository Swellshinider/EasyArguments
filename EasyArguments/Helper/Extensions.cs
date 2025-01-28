namespace EasyArguments.Helper;

/// <summary>
/// Provides extension methods for common type conversions and checks.
/// </summary>
public static class Extensions
{
	/// <summary>
	/// A simple extension method to parse known boolean strings (true/false/yes/no/1/0).
	/// </summary>
	public static bool ToBoolean(this string value) => value.Trim().ToLower() switch
	{
		"true" or "1" or "yes" or "y" => true,
		"false" or "0" or "no" or "n" => false,
		_ => throw new ArgumentException($"Unable to interpret '{value}' as a valid boolean. Invalid argument type")
	};
	
	/// <summary>Returns true if <paramref name="t"/> is bool or nullable bool.</summary>
	public static bool IsBoolean(this Type t) => t == typeof(bool) || t == typeof(bool?);
}