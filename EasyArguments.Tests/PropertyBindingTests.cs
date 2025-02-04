using System.Reflection;
using EasyArguments.Attributes;
using EasyArguments.Helper;

namespace EasyArguments.Tests;

public class PropertyBindingTests
{
	private class TestClass
	{
		[Argument("-a", "--alpha", "Test argument")]
		public string? TestProp { get; set; }
		
		[Argument("-r", null, "Test argument", Required = true)]
		public string? Requirement { get; set; }
		
		[Argument(null, "--ivb", "Test argument", InvertBoolean = true)]
		public bool InvertedBool { get; set; }
	}

	[Fact]
	public void TestMatchesName()
	{
		var prop = typeof(TestClass).GetProperty(nameof(TestClass.TestProp))!;
		var attr = prop.GetCustomAttribute<ArgumentAttribute>()!;
		var binding = new PropertyBinding(prop, attr);

		Assert.True(binding.Matches("-a", '='));
		Assert.True(binding.Matches("--alpha", '='));
	}

	[Fact]
	public void TestAssignValue()
	{
		var testObj = new TestClass();
		var prop = typeof(TestClass).GetProperty(nameof(TestClass.TestProp))!;
		var attr = prop.GetCustomAttribute<ArgumentAttribute>()!;
		var binding = new PropertyBinding(prop, attr);

		binding.AssignValue(testObj, "testValue");
		Assert.Equal("testValue", testObj.TestProp);
	}

	[Fact]
	public void TestAssignValueRequired()
	{
		var testObj = new TestClass();
		var prop = typeof(TestClass).GetProperty(nameof(TestClass.Requirement))!;
		var attr = prop.GetCustomAttribute<ArgumentAttribute>()!;
		var binding = new PropertyBinding(prop, attr);
		
		Assert.True(binding.ArgumentAttr.Required);
		binding.AssignValue(testObj, "requiredValue");
		Assert.Equal("requiredValue", testObj.Requirement);
	}

	[Fact]
	public void TestAssignValueInvertedBool()
	{
		var testObj = new TestClass();
		var prop = typeof(TestClass).GetProperty(nameof(TestClass.InvertedBool))!;
		var attr = prop.GetCustomAttribute<ArgumentAttribute>()!;
		var binding = new PropertyBinding(prop, attr);

		binding.AssignValue(testObj, null);
		Assert.False(testObj.InvertedBool);
	}

	[Fact]
	public void TestUsage()
	{
		var prop = typeof(TestClass).GetProperty(nameof(TestClass.TestProp))!;
		var attr = prop.GetCustomAttribute<ArgumentAttribute>()!;
		var binding = new PropertyBinding(prop, attr);

		var usage = binding.Usage();
		Assert.Contains("--alpha", usage);
		Assert.Contains("Test argument", usage);
	}

	[Fact]
	public void TestGetName()
	{
		var prop = typeof(TestClass).GetProperty(nameof(TestClass.TestProp))!;
		var attr = prop.GetCustomAttribute<ArgumentAttribute>()!;
		var binding = new PropertyBinding(prop, attr);

		Assert.Equal("-a, --alpha", binding.ToString());
	}

	[Fact]
	public void TestGetAvailableName()
	{
		var prop = typeof(TestClass).GetProperty(nameof(TestClass.TestProp))!;
		var attr = prop.GetCustomAttribute<ArgumentAttribute>()!;
		var binding = new PropertyBinding(prop, attr);
		
		Assert.Equal("-a", binding.ToString());
	}
}