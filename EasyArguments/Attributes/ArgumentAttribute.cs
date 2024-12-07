namespace EasyArguments.Attributes;

/// <summary>
/// An attribute used to define arguments for properties.
/// </summary>
/// <param name="argumentNames">An array of argument names associated with the property.</param>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class ArgumentAttribute(params string[] argumentNames) : ArgumentBaseAttribute(argumentNames) { }