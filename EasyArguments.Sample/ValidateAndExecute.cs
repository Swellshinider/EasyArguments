using EasyArguments.Sample.Arguments;

namespace EasyArguments.Sample;

public static class ValidateAndExecute
{
	public static void DisplayVersion()
	{
		Console.WriteLine("Version -> 10.0");
	}
	
	public static void StartArgumentInvoked(StartArgs startCommand)
	{
		Console.WriteLine("Start Command values:");
		Console.WriteLine(startCommand.Url);
		Console.WriteLine(startCommand.Output);
	}
}