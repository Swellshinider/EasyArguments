namespace EasyArguments.Sample;

public static class Program
{
	public static void Main(string[] args)
	{
		var arguments = ArgumentsController.Parse<ArgumentsSample>(["-v", "-o=Path", "start", "-h"]);
		
		Console.WriteLine(arguments.DisplayVersion);
		Console.WriteLine(arguments.OutputFolder);
		Console.WriteLine(arguments.StartCommand.Url);
		Console.WriteLine(arguments.StartCommand.Output);
	}
}