using EasyArguments.Tests.TestClasses;

namespace EasyArguments.Tests;

public class ExecutorTests 
{
	[Fact]
	public void Parse_And_Execute_Success()
	{
		var controller = new ArgumentsController<TestArgumentWithExecution>(["-v"]);
		_ = controller.Parse();
		
		Assert.True(ExecuteClass.VersionWasDisplayed);
	}	
}