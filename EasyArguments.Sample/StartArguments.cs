using EasyArguments.Attributes;

namespace EasyArguments.Sample;

[ArgumentsController]
public class StartArguments
{
	[Argument("-u", "--url", "Url", Required = true)]
	public string? Url { get; set; }

	[Argument("-o", "--output", "Output folder", Required = true)]
	public string? Output { get; set; }
}