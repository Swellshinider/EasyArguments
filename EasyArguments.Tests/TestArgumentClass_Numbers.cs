using EasyArguments.Attributes;

namespace EasyArguments.Tests;

[ArgumentsController]
internal class TestArgumentClass_Numbers
{
    [Argument("--number1", Description = "Number 1")]
    public double Number1 { get; set; }

    [Argument("--number2", Description = "Number 2")]
    public int Number2 { get; set; }

    [Argument("--number3", Description = "Number 3")]
    public long Number3 { get; set; }

    [Argument("--number4", Description = "Number 4")]
    public short Number4 { get; set; }

    [Argument("--number5", Description = "Number 5")]
    public float Number5 { get; set; }

    [Argument("--number6", Description = "Number 6")]
    public decimal Number6 { get; set; }
}