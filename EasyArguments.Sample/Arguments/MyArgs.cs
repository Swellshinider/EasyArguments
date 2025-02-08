using EasyArguments.Attributes;

namespace EasyArguments.Sample.Arguments;

[ArgumentsController(Name = "EasyArguments.exe")]
public class MyArgs
{
	[Argument("-n", "--name", "Specifies the user name", Required = true)]
	public string? Name { get; set; }

	[Argument("-v", "--version", "Display application version", Required = false)]
	[Executor(typeof(ValidateAndExecute), "DisplayVersion")]
	public bool Version { get; set; }

	[Argument(null, "--no-gui", "Disable the GUI", InvertBoolean = true)]
	public bool GuiEnabled { get; set; }

	[Argument(null, "start", "Start command options")]
	[Executor(typeof(ValidateAndExecute), "StartArgumentInvoked")]
	public StartArgs? Start { get; set; }

	public override string ToString()
	{
		return $"Name: {Name}\nVersion: {Version}\nGuiEnabled: {GuiEnabled} {(Start != null ? $"\n\nStart: [{Start}\n]" : "")}";
	}
}