using EasyArguments.Attributes;

namespace EasyArguments.Tests.TestClasses;

public class TestArgumentsNoArgumentController
{
	[Argument("-v", "--verbose", "Verbose output")]
	public bool Verbose { get; set; }

	[Argument(null, null, "Username")]
	public string? User { get; set; }
}