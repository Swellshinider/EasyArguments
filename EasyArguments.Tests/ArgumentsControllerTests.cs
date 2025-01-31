using EasyArguments.Exceptions;
using EasyArguments.Tests.TestClasses;

namespace EasyArguments.Tests;

public class ArgumentsControllerTests
{
	[Fact]
	public void Instantiate_ArgumentClassWithoutAttribute_ThrowsException()
	{
		Assert.Throws<MissingControllerException>(() => new ArgumentsController<TestArgumentsNoArgumentController>([]));
	}

	[Fact]
	public void Parse_BooleanFlag_SetsValue()
	{
		var controller = new ArgumentsController<TestArguments>(["-v"]);

		var result = controller.Parse();

		Assert.True(result.Verbose);
	}

	[Fact]
	public void Parse_InlineValue_SetsCorrectValue()
	{
		var controller = new ArgumentsController<TestArguments>(["--user=admin"]);
		
		var result = controller.Parse();

		Assert.Equal("admin", result.User);
	}

	[Fact]
    public void Parse_MissingRequired_ThrowsException()
	{
		var controller = new ArgumentsController<RequiredArguments>([]);

		Assert.Throws<ArgumentException>(() => controller.Parse());
	}

	[Fact]
	public void Parse_HelpCommand_OutputsUsage()
	{
		var controller = new ArgumentsController<TestArguments>(["-h"]);
		
		var ex = Record.Exception(() => controller.Parse());
		Assert.Null(ex); // Should handle help without throwing
	}

	[Fact]
	public void Parse_NestedArguments_HandlesCorrectly()
	{
		var controller = new ArgumentsController<ParentArgs>(["child", "-c" ]);
		
		var result = controller.Parse();

		Assert.True(result.Child!.ChildFlag);
	}
}