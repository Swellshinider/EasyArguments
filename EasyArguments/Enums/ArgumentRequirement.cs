namespace EasyArguments.Enums;

/// <summary>
/// Specifies the requirement of an argument.
/// </summary>
public enum ArgumentRequirement
{
    /// <summary>
    /// The requirement is automatically detected.
    /// If the field/property has a default value, it is optional; otherwise, it is required.
    /// </summary>
    AutoDetect,
    /// <summary>
    /// The argument is required.
    /// </summary>
    Required,
    /// <summary>
    /// The argument is optional.
    /// </summary>
    Optional
}