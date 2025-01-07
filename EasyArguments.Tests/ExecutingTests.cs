using EasyArguments.Tests.DataTest;

namespace EasyArguments.Tests;

public class ExecutingTests
{
    [Fact]
    public void Execute_NamedArguments()
    {
        var args = new[] { "-n=John", "-d=Description", "-i=Info", "-s=splittedTest", "-l=test" };
        var parser = new ArgumentsController<TestExecutionClass_Strings>(args);
        var result = parser.Parse();
        var executableResults = parser.Execute(result).ToList();

        Assert.Equal(5, executableResults.Count);
        Assert.Equal("ohn", executableResults[0]);
        Assert.Equal("escription", executableResults[1]);
        Assert.Equal("nfo", executableResults[2]);
        Assert.Equal("splittedTest"[..("splittedTest".Length / 2)], executableResults[3]);
        Assert.Equal("test"[..^1], executableResults[4]);
    }

    [Fact]
    public void Execute_EmptyArguments()
    {
        var args = new[] { "-n=", "-d=", "-i=", "-s=splittedTest", "--last=test"};
        var parser = new ArgumentsController<TestExecutionClass_Strings>(args);
        var result = parser.Parse();
        var executableResults = parser.Execute(result).ToList();

        Assert.Equal(5, executableResults.Count);
        Assert.Equal(string.Empty, executableResults[0]);
        Assert.Equal(string.Empty, executableResults[1]);
        Assert.Equal(string.Empty, executableResults[2]);
        Assert.Equal("splittedTest"[..("splittedTest".Length / 2)], executableResults[3]);
        Assert.Equal("test"[..^1], executableResults[4]);
    }

    [Fact]
    public void Execute_SingleCharacterArguments()
    {
        var args = new[] { "-n=J", "-d=D", "-i=I", "-s=splittedTest", "--last=test" };
        var parser = new ArgumentsController<TestExecutionClass_Strings>(args);
        var result = parser.Parse();
        var executableResults = parser.Execute(result).ToList();

        Assert.Equal(5, executableResults.Count);
        Assert.Equal(string.Empty, executableResults[0]);
        Assert.Equal(string.Empty, executableResults[1]);
        Assert.Equal(string.Empty, executableResults[2]);
        Assert.Equal("splittedTest"[..("splittedTest".Length / 2)], executableResults[3]);
        Assert.Equal("test"[..^1], executableResults[4]);
    }
}