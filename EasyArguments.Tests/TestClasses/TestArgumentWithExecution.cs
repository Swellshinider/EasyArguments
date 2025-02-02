using EasyArguments.Attributes;

namespace EasyArguments.Tests.TestClasses;

[ArgumentsController(Name = "tests.exe", ExecuteWhenParsing = true)]
public class TestArgumentWithExecution
{
	[Argument("-v", "--version", "Display version")]
	[Executor(typeof(ExecuteClass), "DisplayVersion")]
	public bool DisplayVersion { get; set; }
}

public static class ExecuteClass
{
	public static bool VersionWasDisplayed { get; set; }

	public static void DisplayVersion(bool display)
	{
		VersionWasDisplayed = display;
		
		if (display)
		{
			// Console.WriteLine()
		}
	}
}