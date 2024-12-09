using EasyArguments.Attributes;

namespace EasyArguments.Tests.DataTest;

[ArgumentsController]
internal class TestExecutionClass_Strings
{
    [Argument("-n", "--name", Description = "The name")]
    [ArgumentExecutor(typeof(ExecutorClass_Test), "RemoveFirstCharacter")]
    public string? Name { get; set; }

    [Argument("-d", "--description", Description = "The description")]
    [ArgumentExecutor(typeof(ExecutorClass_Test), "RemoveFirstCharacter")]
    public string? Description { get; set; }

    [Argument("-i", "--info", Description = "The information")]
    [ArgumentExecutor(typeof(ExecutorClass_Test), "RemoveFirstCharacter")]
    public string? Information { get; set; }

    [Argument("-s", "--split", Description = "Split the string in half")]
    [ArgumentExecutor(typeof(ExecutorClass_Test), "SplitStringInHalf")]
    public string? Split { get; set; }
}
