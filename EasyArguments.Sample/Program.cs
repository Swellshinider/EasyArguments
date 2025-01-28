namespace EasyArguments.Sample;

public static class Program
{
	public static void Main(string[] args)
	{
		// Instantiate a controller for your argument class
		var controller = new ArgumentsController<MyArgs>();

		// Parse the given args
		var parsed = controller.Parse(args);

		// Now you can use the strongly-typed properties:
		Console.WriteLine($"Name: {parsed.Name}");
		Console.WriteLine($"Verbose: {parsed.Verbose}");
		Console.WriteLine($"GUI enabled? {parsed.GuiEnabled}");

		// If the user included "start" on the CLI, 
		// then parsed.Start != null and has its own parsed values:
		if (parsed.Start != null)
		{
			Console.WriteLine($"Starting with URL={parsed.Start.Url}, output={parsed.Start.Output}");
		}
	}
}