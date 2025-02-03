using EasyArguments.Attributes;

namespace EasyArguments.Sample.Arguments;

public class StartArgs
{
	[Argument("-u", "--url", "URL of the service")]
	public string? Url { get; set; }

	[Argument("-o", "--output", "Output directory")]
	public string? Output { get; set; }

    public override string ToString()
    {
		return $"\n    Url: {Url}\n    Output: {Output}";
    }
}