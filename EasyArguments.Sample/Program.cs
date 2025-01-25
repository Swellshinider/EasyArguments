namespace EasyArguments.Sample;

public static class Program
{
	public static void Main(string[] args)
	{
		ArgumentsController.RedirectErrorToConsole = true;
		var arguments = ArgumentsController.Parse<ArgumentsSample>(["-v"]);
	}
}