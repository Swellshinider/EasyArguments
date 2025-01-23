namespace EasyArguments.Sample;

public static class Program
{
	public static void Main(string[] args)
	{
		var parser = new ArgumentsController(["-v", "-o"]);
		var arguments = parser.Parse<ArgumentsSample>();

		Console.WriteLine(arguments.DisplayVersion);
		Console.WriteLine(arguments.OutputFolder);
	}
}