using EasyArguments.Attributes;

namespace EasyArguments.Tests.DataTest;

[ArgumentsController]
internal class TestExecutionClass_Strings
{
    [Argument("-n", "--name", Description = "The name")]
    [ArgumentExecutor<ExecutorClass_Test>("RemoveFirstCharacter")]
    public string? Name { get; set; }

    [Argument("-d", "--description", Description = "The description")]
    [ArgumentExecutor<ExecutorClass_Test>("RemoveFirstCharacter")]
    public string? Description { get; set; }

    [Argument("-i", "--info", Description = "The information")]
    [ArgumentExecutor<ExecutorClass_Test>("RemoveFirstCharacter")]
    public string? Information { get; set; }
}
