using System.Reflection;
using EasyArguments.Attributes;
using EasyArguments.Helper;
using static EasyArguments.Tests.DataCreator;

namespace EasyArguments.Tests.ArgumentsController;

public class ArgumentsTests
{
	[Theory]
	[MemberData(nameof(ArgumentsControllerSeparatorData), MemberType = typeof(DataCreator))]
	public void ArgumentsControllerAttribute_Separator_Setter_Test(char separator, bool shouldThrow)
	{
		var attr = new ArgumentsControllerAttribute { Name = "TestApp" };
		if (shouldThrow)
		{
			Assert.Throws<ArgumentException>(() => attr.Separator = separator);
		}
		else
		{
			attr.Separator = separator;
			Assert.Equal(separator, attr.Separator);
		}
	}

	[Theory]
	[MemberData(nameof(ExecutorAttributeData), MemberType = typeof(DataCreator))]
	public void ExecutorAttribute_Constructor_Test(Type type, string methodName, bool shouldThrow)
	{
		if (shouldThrow)
		{
			Assert.Throws<ArgumentException>(() => new ExecutorAttribute(type, methodName));
		}
		else
		{
			var attr = new ExecutorAttribute(type, methodName);
			Assert.NotNull(attr.MethodInfo);
			Assert.Equal(methodName, attr.MethodInfo.Name);
		}
	}

	[Theory]
	[MemberData(nameof(AssignValueData), MemberType = typeof(DataCreator))]
	public void PropertyBinding_AssignValue_Test(string propertyName, object input, object expected)
	{
		var instance = new DummyAssign();
		var prop = typeof(DummyAssign).GetProperty(propertyName);
		Assert.NotNull(prop);
		var attr = prop.GetCustomAttribute<ArgumentAttribute>();
		Assert.NotNull(attr);
		var binding = new PropertyBinding(prop, attr);
		binding.AssignValue(instance, input);
		var actual = prop.GetValue(instance);
		Assert.Equal(expected, actual);
	}

	[Fact]
	public void PropertyBinding_AssignValue_InvalidConversion_Test()
	{
		var instance = new DummyAssign();
		var prop = typeof(DummyAssign).GetProperty("IntProp");
		Assert.NotNull(prop);
		var attr = prop.GetCustomAttribute<ArgumentAttribute>()!;
		var binding = new PropertyBinding(prop, attr);
		// Passing a non-numeric string to an int property should throw.
		Assert.Throws<ArgumentException>(() => binding.AssignValue(instance, "NaN"));
	}

	[Fact]
	public void ExtractProperties_NestedArgument_Test()
	{
		var bindings = typeof(DummyNested).ExtractProperties();
		// Expect one binding for property 'Sub'
		var subBinding = bindings.FirstOrDefault();
		Assert.NotNull(subBinding);
		Assert.True(subBinding.IsParent);
		Assert.Single(subBinding.Children);
	}

	[Theory]
	[MemberData(nameof(ArgumentsControllerParseData), MemberType = typeof(DataCreator))]
	public void ArgumentsController_Parse_Test(string input,
												string? expectedName,
												bool expectedVerbose,
												int expectedCount,
												string? expectedOption,
												string? expectedSubArg,
												bool helpDisplayedExpected)
	{
		var controller = new ArgumentsController<DummyArgs>(input);
		var args = controller.Parse(out bool helpDisplayed);
		
		Assert.Equal(helpDisplayedExpected, helpDisplayed);
		
		if (!helpDisplayed)
		{
			Assert.Equal(expectedName, args.Name);
			Assert.Equal(expectedVerbose, args.Verbose);
			Assert.Equal(expectedCount, args.Count);
			Assert.Equal(expectedOption, args.Option);
			Assert.NotNull(args.Sub);
			Assert.Equal(expectedSubArg, args.Sub.SubArg);
		}
	}

	[Fact]
	public void ArgumentsController_MissingRequired_Test()
	{
		var controller = new ArgumentsController<DummyRequired>([]);
		Assert.Throws<ArgumentException>(() => controller.Parse());
	}

	[Fact]
	public void ArgumentsController_GetUsageText_Test()
	{
		var controller = new ArgumentsController<DummyArgs>("-n John");
		var usage = controller.GetUsageText();
		var usageText = usage.ToString();
		
		Assert.Contains("Available options:", usageText);
	}

	[Fact]
	public void ExecutorAttribute_Execution_Test()
	{
		var controller = new ArgumentsController<ExecutorArgs>("-m World");
		var args = controller.Parse();
		Assert.Equal("World Hello", args.Message);
	}
}