using EasyArguments.Attributes;

namespace EasyArguments.Sample;

public class StartArgs
{
    [Argument("-u", "--url", "URL of the service")]
    public string? Url { get; set; }

    [Argument("-o", "--output", "Output directory")]
    public string? Output { get; set; }
}