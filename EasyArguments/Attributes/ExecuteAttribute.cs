namespace EasyArguments.Attributes;

/// <summary>
/// An attribute used to specify that a static method from a specified class should be executed in association with a property.
/// This attribute targets properties and optionally allows the result of the method's execution to be assigned to the property.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
public sealed class ExecuteAttribute : Attribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ExecuteAttribute"/> class with the specified class type and method name.
	/// </summary>
	/// <param name="classType">
	/// The <see cref="Type"/> of the class containing the static method to be executed.
	/// </param>
	/// <param name="methodName">
	/// The name of the static method in the specified class to be executed.
	/// </param>
	public ExecuteAttribute(Type classType, string methodName)
	{
		ClassType = classType;
		MethodName = methodName;
	}
	
	/// <summary>
	/// Gets the <see cref="Type"/> of the class containing the static method to be executed.
	/// </summary>
	public Type ClassType { get; }

	/// <summary>
    /// Gets the name of the static method to be executed.
    /// The method must be defined in the class specified by <see cref="ClassType"/>.
    /// </summary>
	public string MethodName { get; }

	/// <summary>
	/// Gets or sets a value indicating whether the result of the method execution
	/// should be assigned to the property. This assignment will only occur if the
	/// result is compatible with the property's type.
	/// </summary>
	public bool AssignResultToProperty { get; set; } = false;
}