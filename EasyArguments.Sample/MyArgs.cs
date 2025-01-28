using EasyArguments.Attributes;

namespace EasyArguments.Sample;

[ArgumentsController]
public class MyArgs
{
    [Argument("-n", "--name", "Specifies the user name", Required = true)]
    public string? Name { get; set; }

    [Argument("-v", "--verbose", "Enable verbose output", Required = false)]
    public bool? Verbose { get; set; }

    [Argument(null, "--no-gui", "Disable the GUI", InvertBoolean = true)]
    public bool GuiEnabled { get; set; }

    [Argument(null, "start", "Start command options")]
    public StartArgs? Start { get; set; }
}