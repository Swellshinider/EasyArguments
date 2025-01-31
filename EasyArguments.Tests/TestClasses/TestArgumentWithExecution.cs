using EasyArguments.Attributes;

namespace EasyArguments.Tests.TestClasses;

[ArgumentsController(ExecuteWhenParsing = true)]
public class TestArgumentWithExecution
{
	[Argument("-v", "--version", "Display version")]
	[Executor(typeof(ExecuteClass), "DisplayVersion")]
	public bool DisplayVersion { get; set; }
}

public static class ExecuteClass
{
	public static bool VersionWasDisplayed { get; set; }

	public static bool DisplayVersion(bool display)
	{
		VersionWasDisplayed = display;
		
		if (display)
		{
			// Display version here, Console.WriteLine()
			return true;
		}

		return false;
	}
}