using EasyArguments.Attributes;

namespace EasyArguments.Sample;

[ArgumentsController(AutoHelpArgument = true, RespectOrder = true)]
public class StartArguments
{
	[Argument("-u", "--url", "Url")]
	public string? Url { get; set; }

	[Argument("-o", "--output", "Output folder")]
	public string? Output { get; set; }
}