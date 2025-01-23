using EasyArguments.Attributes;
using EasyArguments.Enums;

namespace EasyArguments.Tests.DataTest;

[ArgumentsController]
internal class TestArgumentClass_Strings
{
    [Argument("-n", "--name", Description = "The name", Required = true)]
    public string? Name { get; set; }

    [Argument("-d", "--description", Description = "The description", Required = true)]
    public string? Description { get; set; }

    [Argument("-i", "--info", Description = "The information", Required = true)]
    public string? Information { get; set; }
}