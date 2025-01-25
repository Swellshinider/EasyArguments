using System.Reflection;

namespace EasyArguments.Sample;

public static class StaticExecutor 
{
	public static void DisplayVersion()	
	{
		Console.Write("Version: ");
		Console.WriteLine(Assembly.GetExecutingAssembly().ImageRuntimeVersion);
	}
	
	public static string SaveOutputFolder(string output) 
	{
		Console.WriteLine($"Oh, i saved the output folder: '{output}'");
		return output.ToUpper();
	}
}