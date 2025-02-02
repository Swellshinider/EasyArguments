using EasyArguments.Attributes;

namespace EasyArguments.Tests.TestClasses;

[ArgumentsController(Name = "tests.exe", AutoHelpArgument = false)]
public class RequiredArguments
{
	[Argument("-r", "--required", "Required arg", Required = true)]
	public string? Required { get; set; }
}