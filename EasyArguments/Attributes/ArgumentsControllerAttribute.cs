namespace EasyArguments.Attributes;

/// <summary>
/// Specifies that a class is an arguments controller, which is responsible for handling and managing arguments.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class ArgumentsControllerAttribute : Attribute
{
    /// <summary>
    /// Gets or sets a value indicating whether the order of arguments should be respected.
    /// Defaults to <c>false</c>.
    /// </summary>
    public bool RespectOrder { get; set; } = false;
}