using EasyArguments.Attributes;
using EasyArguments.Exceptions;

namespace EasyArguments.Tests;

public class ArgumentsControllerTests
{
	private class TestArgumentsNoArgumentController
	{
		[Argument("-v", "--verbose", "Verbose output")]
		public bool Verbose { get; set; }

		[Argument(null, null, "Username")]
		public string? User { get; set; }
	}
	
	[ArgumentsController]
	private class TestArguments
	{
		[Argument("-v", "--verbose", "Verbose output")]
		public bool Verbose { get; set; }

		[Argument(null, null, "Username")]
		public string? User { get; set; }
	}

	[ArgumentsController(AutoHelpArgument = false)]
	private class RequiredArguments
	{
		[Argument("-r", "--required", "Required arg", Required = true)]
		public string? Required { get; set; }
	}

	[ArgumentsController]
	private class ParentArgs
	{
		[Argument(null, "child", "Child arguments")]
		public ChildArgs? Child { get; set; }
	}

	[ArgumentsController]
	private class ChildArgs
	{
		[Argument("-c", "--child-flag", "Child flag")]
		public bool ChildFlag { get; set; }
	}
	
	[Fact]
	public void Instantiate_ArgumentClassWithoutAttribute_ThrowsException()
	{
		Assert.Throws<MissingControllerException>(() => new ArgumentsController<TestArgumentsNoArgumentController>());
	}

	[Fact]
	public void Parse_BooleanFlag_SetsValue()
	{
		var controller = new ArgumentsController<TestArguments>();
		var args = new[] { "-v" };

		var result = controller.Parse(args);

		Assert.True(result.Verbose);
	}

	[Fact]
	public void Parse_InlineValue_SetsCorrectValue()
	{
		var controller = new ArgumentsController<TestArguments>();
		var args = new[] { "--user=admin" };

		var result = controller.Parse(args);

		Assert.Equal("admin", result.User);
	}

	[Fact(Skip = "This test is skipped for now because it's not ready")]
	public void Parse_MissingRequired_ThrowsException()
	{
		var controller = new ArgumentsController<RequiredArguments>();

		Assert.Throws<ArgumentException>(() => controller.Parse([]));
	}

	[Fact]
	public void Parse_HelpCommand_OutputsUsage()
	{
		var controller = new ArgumentsController<TestArguments>();
		var args = new[] { "-h" };

		var ex = Record.Exception(() => controller.Parse(args));
		Assert.Null(ex); // Should handle help without throwing
	}

	[Fact]
	public void Parse_NestedArguments_HandlesCorrectly()
	{
		var controller = new ArgumentsController<ParentArgs>();
		var args = new[] { "child", "-c" };

		var result = controller.Parse(args);

		Assert.True(result.Child!.ChildFlag);
	}
}