using EasyArguments.Attributes;

namespace EasyArguments.Tests.TestClasses;

[ArgumentsController(Name = "tests.exe")]
public class ParentArgs
{
	[Argument(null, "--lang", "Display language")]
	public bool? Lang { get; set; }
	
	[Argument(null, "child", "Child arguments")]
	public ChildArgs? Child { get; set; }
}