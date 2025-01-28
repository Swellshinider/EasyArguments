using EasyArguments.Helper;

namespace EasyArguments.Tests;

public class ExtensionsTests
{
	[Theory]
	[InlineData("true", true)]
	[InlineData("TRUE", true)]
	[InlineData("1", true)]
	[InlineData("yes", true)]
	[InlineData("false", false)]
	[InlineData("FALSE", false)]
	[InlineData("0", false)]
	[InlineData("no", false)]
	public void ToBoolean_ValidValues_ReturnsExpected(string input, bool expected)
	{
		Assert.Equal(expected, input.ToBoolean());
	}

	[Theory]
	[InlineData("invalid")]
	[InlineData("2")]
	[InlineData("maybe")]
	public void ToBoolean_InvalidValues_ThrowsException(string input)
	{
		Assert.Throws<ArgumentException>(() => input.ToBoolean());
	}

	[Theory]
	[InlineData(typeof(bool), true)]
	[InlineData(typeof(bool?), true)]
	[InlineData(typeof(string), false)]
	[InlineData(typeof(object), false)]
	public void IsBoolean_TypeCheck_ReturnsExpected(Type type, bool expected)
	{
		Assert.Equal(expected, type.IsBoolean());
	}
}