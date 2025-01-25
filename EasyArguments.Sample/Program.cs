namespace EasyArguments.Sample;

public static class Program
{
	public static void Main(string[] args)
	{
		ArgumentsController.RedirectErrorToConsole = true;
		var arguments = ArgumentsController.Parse<ArgumentsSample>(["-v", "-o=path_mec"]);
		var results = ArgumentsController.Execute(arguments);
		results.ToList().ForEach(Console.WriteLine);
	}
}