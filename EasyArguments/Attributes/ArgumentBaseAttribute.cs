using EasyArguments.Enums;

namespace EasyArguments.Attributes;

/// <summary>
/// An abstract attribute used to associate one or more argument names with a property.
/// </summary>
/// <remarks>
/// This attribute can be applied to properties to specify argument names
/// and optionally provide a description. It is intended to be inherited by custom attributes.
/// </remarks>
/// <param name="argumentNames">An array of argument names associated with the property.</param>
[AttributeUsage(AttributeTargets.Property)]
public abstract class ArgumentBaseAttribute(params string[] argumentNames) : Attribute
{
    /// <summary>
    /// Gets the argument names associated with the property.
    /// </summary>
    public string[] ArgumentNames { get; } = argumentNames;

    /// <summary>
    /// Gets or sets the description of the argument.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Gets or sets the requirement for the argument.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="ArgumentRequirement.AutoDetect"/> (The requirement is automatically detected).
    /// <para></para>
    /// If the Property has a default value, it is optional; otherwise, it is required.
    /// </remarks>
    public ArgumentRequirement Required { get; set; } = ArgumentRequirement.AutoDetect;
}