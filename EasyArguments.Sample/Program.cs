namespace EasyArguments.Sample;

public static class Program
{
	public static bool IsRunning { get; set; } = true;
	
	public static void Main()
	{
		ArgumentsController.RedirectErrorToConsole = true;

		while (IsRunning)
		{
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.Write("> ");
			var input = Console.ReadLine() ?? "";
			Console.ResetColor();

			var args = input.Split(' ', StringSplitOptions.TrimEntries);
			var arguments = ArgumentsController.Parse<ArgumentsSample>(args);
			_ = ArgumentsController.Execute(arguments).ToList();
		}
	}
}