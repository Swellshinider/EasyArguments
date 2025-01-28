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
	}

	[Fact]
	public void Matches_ShortName_ReturnsTrue()
	{
		var prop = typeof(TestClass).GetProperty(nameof(TestClass.TestProp))!;
		var attr = prop.GetCustomAttribute<ArgumentAttribute>()!;
		var binding = new PropertyBinding(prop, attr);

		Assert.True(binding.Matches("-a"));
		Assert.True(binding.Matches("-a=value"));
	}

	[Fact]
	public void GetName_FormatsCorrectly()
	{
		var prop = typeof(TestClass).GetProperty(nameof(TestClass.TestProp))!;
		var attr = prop.GetCustomAttribute<ArgumentAttribute>()!;
		var binding = new PropertyBinding(prop, attr);

		Assert.Equal("[-a, --alpha]", binding.GetName());
	}
}