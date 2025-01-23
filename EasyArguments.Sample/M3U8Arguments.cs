using EasyArguments.Attributes;

namespace EasyArguments.Sample;

[ArgumentsController(AutoHelpArgument = true, RespectOrder = true)]
public class M3U8Arguments
{
	[Argument("-u", "--url", Description = "Url of m3u8 target")]
	public string? Url { get; set; }

	[Argument("-o", "--output", Description = "Output folder")]
	public string? Output { get; set; }
}