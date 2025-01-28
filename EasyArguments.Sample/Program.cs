namespace EasyArguments.Sample;

public static class Program
{
	public static bool IsRunning { get; set; } = true;
	
	public static void Main()
	{
		var argumentController = new ArgumentsController<ArgumentsSample>();
		Console.WriteLine(argumentController.GetUsageText());
		
		while (IsRunning)
		{
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.Write("> ");
			var input = Console.ReadLine() ?? "";
			Console.ResetColor();

			var args = input.Split(' ', StringSplitOptions.TrimEntries);
			try 
			{
				var arguments = argumentController.Parse(args);
				
				Console.WriteLine(arguments.DisplayVersion);
				Console.WriteLine(arguments.Stop);
				Console.WriteLine(arguments.StartCommand?.Url);
				Console.WriteLine(arguments.StartCommand?.Output);
				
			}
			catch (Exception e)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(e.Message);
				Console.ResetColor();
			}
		}
	}
}