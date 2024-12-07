using EasyArguments.Tests.DataTest;

namespace EasyArguments.Tests;

public class ExecutingTests
{
    [Fact]
    public void Execute_NamedArguments()
    {
        var args = new[] { "-n=John", "-d=Description", "-i=Info" };
        var parser = new ArgumentsController<TestExecutionClass_Strings>(args);
        var result = parser.Parse();
        var executableResults = parser.Execute<ExecutorClass_Test>(result).ToList();

        Assert.Equal(3, executableResults.Count);
        Assert.Equal("ohn", executableResults[0]);
        Assert.Equal("escription", executableResults[1]);
        Assert.Equal("nfo", executableResults[2]);
    }

    [Fact]
    public void Execute_EmptyArguments()
    {
        var args = new[] { "-n=", "-d=", "-i=" };
        var parser = new ArgumentsController<TestExecutionClass_Strings>(args);
        var result = parser.Parse();
        var executableResults = parser.Execute<ExecutorClass_Test>(result).ToList();

        Assert.Equal(3, executableResults.Count);
        Assert.Equal(string.Empty, executableResults[0]);
        Assert.Equal(string.Empty, executableResults[1]);
        Assert.Equal(string.Empty, executableResults[2]);
    }

    [Fact]
    public void Execute_SingleCharacterArguments()
    {
        var args = new[] { "-n=J", "-d=D", "-i=I" };
        var parser = new ArgumentsController<TestExecutionClass_Strings>(args);
        var result = parser.Parse();
        var executableResults = parser.Execute<ExecutorClass_Test>(result).ToList();

        Assert.Equal(3, executableResults.Count);
        Assert.Equal(string.Empty, executableResults[0]);
        Assert.Equal(string.Empty, executableResults[1]);
        Assert.Equal(string.Empty, executableResults[2]);
    }
}
