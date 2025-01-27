using System.Reflection;

namespace EasyArguments.Sample;

public static class StaticExecutor 
{
	public static void DisplayVersion(bool? version)
	{
		if (!version.HasValue || !version.Value) // Argument --version was not called
			return;
			
		Console.Write("Version: ");
		Console.WriteLine(Assembly.GetAssembly(typeof(Program))?.ManifestModule.MDStreamVersion);
	}
	
	public static void StopApplication(bool? stop) 
	{
		if (!stop.HasValue || !stop.Value)
			return;
			
		Program.IsRunning = false;
	}
}