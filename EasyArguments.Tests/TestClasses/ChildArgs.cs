using EasyArguments.Attributes;

namespace EasyArguments.Tests.TestClasses;

public class ChildArgs
{
	[Argument("-c", "--child-flag", "Child flag")]
	public bool ChildFlag { get; set; }
	
	[Argument("-s", "--second", "Second child flag")]
	public bool SecondFlag { get; set; }
}