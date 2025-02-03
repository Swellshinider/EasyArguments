using EasyArguments.Sample.Arguments;

namespace EasyArguments.Sample;

public static class Program
{
	public static void Main()
	{
		while (true)
		{
			try
			{
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.Write("> ");
				var input = Console.ReadLine() ?? "";
				Console.ResetColor();
				
				if (input == "exit")
					break;

				var controller = new ArgumentsController<MyArgs>(input);
				var result = controller.Parse(out var helpMessageDisplayed);
				
				if (helpMessageDisplayed)
					continue;
				
				Console.WriteLine("Parsing result:\n");
				Console.WriteLine(result);
				Console.WriteLine();
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
			}
		}
	}
}