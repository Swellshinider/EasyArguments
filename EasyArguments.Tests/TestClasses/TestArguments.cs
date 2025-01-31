using EasyArguments.Attributes;

namespace EasyArguments.Tests.TestClasses;

[ArgumentsController]
public class TestArguments
{
	[Argument("-v", "--verbose", "Verbose output")]
	public bool Verbose { get; set; }

	[Argument(null, null, "Username")]
	public string? User { get; set; }
}