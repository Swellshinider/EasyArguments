using EasyArguments.Attributes;

namespace EasyArguments.Tests;

[ArgumentsController]
internal class TestArgumentClass_Strings
{
    [Argument("-n", "--name", Description = "The name")]
    public string? Name { get; set; }

    [Argument("-d", "--description", Description = "The description")]
    public string? Description { get; set; }

    [Argument("-i", "--info", Description = "The information")]
    public string? Information { get; set; }
}