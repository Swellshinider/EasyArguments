using EasyArguments.Attributes;

namespace EasyArguments.Sample;

[ArgumentsController(AutoHelpArgument = true, RespectOrder = true)]
public class StartArguments
{
	[Argument("-u", "--url", "Url", Required = false)]
	public string? Url { get; set; }

	[Argument("-o", "--output", "Output folder", Required = false)]
	public string? Output { get; set; }
}