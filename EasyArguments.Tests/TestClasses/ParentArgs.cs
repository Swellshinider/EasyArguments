using EasyArguments.Attributes;

namespace EasyArguments.Tests.TestClasses;

[ArgumentsController]
public class ParentArgs
{
	[Argument(null, "--lang", "Display language")]
	public bool? Lang { get; set; }
	
	[Argument(null, "child", "Child arguments")]
	public ChildArgs? Child { get; set; }
}