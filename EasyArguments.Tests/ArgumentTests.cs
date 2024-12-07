using EasyArguments.Exceptions;

namespace EasyArguments.Tests;

public class ArgumentTests
{
    [Fact]
    public void Parse_StringsArguments_ShouldParseCorrectly()
    {
        var args = new[] { "-n=John", "-d=Description", "-i=Info" };
        var parser = new ArgumentParser<TestArgumentClass_Strings>(args);
        var result = parser.Parse();

        Assert.Equal("John", result.Name);
        Assert.Equal("Description", result.Description);
        Assert.Equal("Info", result.Information);
    }

    [Fact]
    public void Parse_NumbersArguments_ShouldParseCorrectly()
    {
        var args = new[] { "--number1=1.23", "--number2=123", "--number3=123456789", "--number4=12345", "--number5=1.23", "--number6=123.45" };
        var parser = new ArgumentParser<TestArgumentClass_Numbers>(args);
        var result = parser.Parse();

        Assert.Equal(1.23, result.Number1);
        Assert.Equal(123, result.Number2);
        Assert.Equal(123456789, result.Number3);
        Assert.Equal((short)12345, result.Number4);
        Assert.Equal(1.23f, result.Number5);
        Assert.Equal(123.45m, result.Number6);
    }

    [Fact]
    public void Parse_BooleansArguments_ShouldParseCorrectly()
    {
        var args = new[] { "-f", "-v" };
        var parser = new ArgumentParser<TestArgumentClass_Booleans>(args);
        var result = parser.Parse();

        Assert.True(result.Flag);
        Assert.True(result.Verbose);
    }

    [Fact]
    public void Parse_NoAttribute_ShouldThrowException()
    {
        var args = new[] { "--id=1", "-n=John", "-v=1" };
        var parser = new ArgumentParser<TestArgumentClass_NoAttribute>(args);

        Assert.Throws<MissingArgumentsControllerAttributeException>(() => parser.Parse());
    }

    [Fact]
    public void Parse_UnknownArgument_ShouldThrowException()
    {
        var args = new[] { "--unknown=1" };
        var parser = new ArgumentParser<TestArgumentClass_Strings>(args);

        Assert.Throws<UnknownArgumentException>(() => parser.Parse());
    }

    [Fact]
    public void Parse_InvalidArgumentType_ShouldThrowException()
    {
        var args = new[] { "--number2=1.23" };
        var parser = new ArgumentParser<TestArgumentClass_Numbers>(args);

        Assert.Throws<InvalidArgumentTypeException>(() => parser.Parse());
    }
}