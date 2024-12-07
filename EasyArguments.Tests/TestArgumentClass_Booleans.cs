using EasyArguments.Attributes;

namespace EasyArguments.Tests;

[ArgumentsController]
internal class TestArgumentClass_Booleans
{
    [Argument("-f", "--flag", Description = "The flag")]
    public bool? Flag { get; set; }

    [Argument("-v", "--verbose", Description = "Verbose mode")]
    public bool Verbose { get; set; }
}