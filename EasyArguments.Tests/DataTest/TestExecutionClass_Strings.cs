using EasyArguments.Attributes;

namespace EasyArguments.Tests.DataTest;

[ArgumentsController]
internal class TestExecutionClass_Strings
{
    [Argument("-n", "--name", Description = "The name")]
    public string? Name { get; set; }

    [Argument("-d", "--description", Description = "The description")]
    public string? Description { get; set; }

    [Argument("-i", "--info", Description = "The information")]
    public string? Information { get; set; }

    [Argument("-s", "--split", Description = "Split the string in half")]
    public string? Split { get; set; }

    [Argument("-l", "--last", Description = "Remove the last character")]
    public string? Last { get; set; }
    
}
