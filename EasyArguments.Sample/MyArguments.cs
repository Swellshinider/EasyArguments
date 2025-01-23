using EasyArguments.Attributes;

namespace EasyArguments.Sample;

[ArgumentsController]
public class ArgumentsSample
{
	[Argument("-v", "-version", Description = "Display version", Required = false)]
	public bool DisplayVersion { get; set; }

	[Argument("-o", "--out", Description = "Defines the output folder", Required = false)]
	public string? OutputFolder { get; set; }
}