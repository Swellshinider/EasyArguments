using EasyArguments.Attributes;

namespace EasyArguments.Sample;

[ArgumentsController]
public class ArgumentsSample
{
	[Argument("-v", "--version", "Display version", Required = false)]
	[Execute(typeof(StaticExecutor), "DisplayVersion")]
	public bool? DisplayVersion { get; set; }

	[Argument("-o", "--out", "Defines the output folder", Required = true)]
	[Execute(typeof(StaticExecutor), "SaveOutputFolder", AssignResultToProperty = true)]
	public string? OutputFolder { get; set; }
	
	[Argument(null, "start", "Start the application command", Required = false)]
	public StartArguments? StartCommand { get; set; }
}