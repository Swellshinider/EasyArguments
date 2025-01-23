using EasyArguments.Attributes;

namespace EasyArguments.Tests.DataTest;

[ArgumentsController]
internal class TestArgumentClass_Booleans
{
    [Argument("-f", "--flag", Description = "The flag", Required = false)]
    public bool? Flag { get; set; }

    [Argument("-v", "--verbose", Description = "Verbose mode")]
    public bool Verbose { get; set; }
}