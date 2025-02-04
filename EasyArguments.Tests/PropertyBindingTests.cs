using System.Reflection;
using EasyArguments.Attributes;
using EasyArguments.Helper;
using static EasyArguments.Tests.DataCreator;

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
		
		[Argument(null, "--bool", "Test argument")]
		public bool NormalBool { get; set; }
	}
	
	[Theory]
	[InlineData("-x", true)]
	[InlineData("--execute", true)]
	[InlineData("-x=value", true)]
	[InlineData("--execute=value", true)]
	[InlineData("--other", false)]
	public void PropertyBinding_Matches_Test(string testArg, bool expected)
	{
		var prop = typeof(TestMatchesClass).GetProperty("Arg");
		Assert.NotNull(prop);
		var attr = prop.GetCustomAttribute<ArgumentAttribute>();
		Assert.NotNull(attr);
		var binding = new PropertyBinding(prop, attr);
		Assert.Equal(expected, binding.Matches(testArg, '='));
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
	public void TestAssignValueBool()
	{
		var testObj = new TestClass();
		var prop = typeof(TestClass).GetProperty(nameof(TestClass.NormalBool))!;
		var attr = prop.GetCustomAttribute<ArgumentAttribute>()!;
		var binding = new PropertyBinding(prop, attr);

		binding.AssignValue(testObj, null);
		Assert.True(testObj.NormalBool);
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
}