using System.Reflection;

namespace EasyArguments.Attributes;

/// <summary>
/// An attribute to specify a static class and method to be executed.
/// </summary>
/// <remarks>
/// This attribute can be applied to properties to specify a static class and method to be executed.
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
public sealed class ExecutorAttributeAttribute : Attribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ExecutorAttributeAttribute"/> class.
	/// </summary>
	/// <param name="staticClass">The static class containing the method.</param>
	/// <param name="methodName">The name of the method to be executed.</param>
	/// <exception cref="ArgumentException">Thrown when the provided type is not a static class or the method does not exist.</exception>
	public ExecutorAttributeAttribute(Type staticClass, string methodName)
	{
		// Check if the provided type is a static class
		if (!staticClass.IsAbstract || !staticClass.IsSealed || staticClass.IsGenericType)
			throw new ArgumentException("The provided type must be a static class.", nameof(staticClass));

		// Check if the provided string exists as a method in the class
		var methodInfos = staticClass.GetMethods(BindingFlags.Static | BindingFlags.Public).Where(m => m.Name == methodName);
		if (!methodInfos.Any())
			throw new ArgumentException($"The method '{methodName}' does not exist in the static class '{staticClass.FullName}'.", nameof(methodName));

		StaticClass = staticClass;
		MethodInfo = methodInfos.First();
	}

	/// <summary>
	/// Gets the static class containing the method.
	/// </summary>
	public Type StaticClass { get; }

	/// <summary>
	/// Gets the <see cref=" System.Reflection.MethodInfo"/> to be executed.
	/// </summary>
	public MethodInfo MethodInfo { get; }

	/// <summary>
	/// Gets or sets a value indicating whether the result of the method execution should be assigned to the property.
	/// </summary>
	public bool AssignResultToProperty { get; set; }
}