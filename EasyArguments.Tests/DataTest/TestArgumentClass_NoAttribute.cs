using EasyArguments.Attributes;

namespace EasyArguments.Tests.DataTest;

internal class TestArgumentClass_NoAttribute
{
    [Argument("--id", Description = "The id")]
    public int Id { get; set; }

    [Argument("-n", "--name", Description = "The name")]
    public string? Name { get; set; }

    [Argument("-v", "--verbose", Description = "Verbose mode")]
    public int Verbose { get; set; }
}