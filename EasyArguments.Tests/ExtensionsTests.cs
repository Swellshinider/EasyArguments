using System.Diagnostics;
using EasyArguments.Helper;

namespace EasyArguments.Tests;

public class ExtensionsTests
{
	[Theory]
	[MemberData(nameof(DataCreator.ToBooleanData), MemberType = typeof(DataCreator))]
	public void ToBoolean_Extension_Test(string input, bool expected, bool shouldThrow)
	{
		if (shouldThrow)
		{
			Assert.Throws<ArgumentException>(() => input.ToBoolean());
		}
		else
		{
			Assert.Equal(expected, input.ToBoolean());
		}
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
	
	[Theory]
	[MemberData(nameof(DataCreator.TokensData), MemberType = typeof(DataCreator))]
	public void Tokenize_ValidInput_ReturnsExpectedTokens(string input, char separator, string[] expected)
	{
		var result = input.Tokenize(separator);
		Debug.WriteLine(input);
		Assert.Equal(expected, result);
	}
}