using EasyArguments.Attributes;

namespace EasyArguments.Sample;

[ArgumentsController]
public class ArgumentsSample
{
	[Argument("-v", "--version", "Display version", Required = false)]
	public bool? DisplayVersion { get; set; }
	
	[Argument(null, "stop", "Stop application", Required = false)]
	public bool? Stop { get; set; }
	
	[Argument(null, "start", "Start the application command", Required = false)]
	public StartArguments? StartCommand { get; set; }
}