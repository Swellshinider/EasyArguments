using EasyArguments.Attributes;

namespace EasyArguments.Tests;

public static class DataCreator
{
	#region [ Test Helper Types ]
	// A dummy static class for ExecutorAttribute testing.
	public static class TestStaticExecutor
	{
		public static string ValidMethod(string input) => input + " executed";
	}

	// A non-static class to trigger ExecutorAttribute errors.
	public class NonStaticExecutor
	{
#pragma warning disable S2325, CA1822 // Mark members as static
		public string ValidMethod(string input) => input + " executed";
#pragma warning restore S2325, CA1822 // Mark members as static
	}
	
	// Dummy class for testing Matches() in PropertyBinding.
	public class TestMatchesClass
	{
		[Argument("-x", "--execute", "Test argument")]
		public string? Arg { get; set; }
	}

	// Dummy class for testing property value assignments.
	public class DummyAssign
	{
		[Argument("-s", "--string", "String property")]
		public string? StringProp { get; set; }

		[Argument("-i", "--int", "Int property")]
		public int IntProp { get; set; }

		[Argument("-b", "--bool", "Bool property")]
		public bool BoolProp { get; set; }

		[Argument("-n", "--boolinvert", "Inverted bool property", InvertBoolean = true)]
		public bool BoolInvertProp { get; set; }
	}

	// Dummy types to test nested argument extraction.
	public class DummyNested
	{
		[Argument("-s", "--sub", "Sub argument")]
		public DummySub Sub { get; set; } = new DummySub();
	}
	
	public class DummySub
	{
		[Argument("-a", "--subarg", "Sub argument property")]
		public string? SubArg { get; set; }
	}

	// Dummy arguments for testing the main parsing.
	[ArgumentsController(Name = "TestApp")]
	public class DummyArgs
	{
		[Argument("-n", "--name", "Name")]
		public string? Name { get; set; }

		[Argument("-v", "--verbose", "Verbose flag")]
		public bool Verbose { get; set; }

		[Argument("-c", "--count", "Count")]
		public int Count { get; set; }

		// When both short and long names are null, long name should default to "--{propertyname}".
		[Argument(null, null, "Option with default long name")]
		public string? Option { get; set; }

		[Argument("-s", "--sub", "Sub arguments")]
		public DummySub Sub { get; set; } = new DummySub();
	}

	// Dummy type with a required argument.
	[ArgumentsController(Name = "RequiredTest")]
	public class DummyRequired
	{
		[Argument("-r", "--required", "Required argument", Required = true)]
		public string? RequiredArg { get; set; }
	}

	// Dummy type for testing executor execution.
	public static class TestStaticMethods
	{
		public static string AppendHello(string input) => input + " Hello";
	}
	
	[ArgumentsController(Name = "ExecutorTest")]
	public class ExecutorArgs
	{
		[Argument("-m", "--message", "A message")]
		[Executor(typeof(TestStaticMethods), "AppendHello", AssignResultToProperty = true)]
		public string? Message { get; set; }
	}

	#endregion
	
	public static TheoryData<char, bool> ArgumentsControllerSeparatorData => new()
	{
		{ '=', false },
		{ '-', false },
		{ '\0', true }
	};
	
	public static TheoryData<Type, string, bool> ExecutorAttributeData => new()
	{
		{ typeof(TestStaticExecutor), "ValidMethod", false }, // Valid static class with an existing method.
		{ typeof(NonStaticExecutor), "ValidMethod", true }, // Non-static class: should throw.
		{ typeof(TestStaticExecutor), "NonExistentMethod", true } // Static class but method does not exist: should throw.
	};
	
	public static TheoryData<string, bool, bool> ToBooleanData => new()
	{
		{ "true", true, false },
		{ "false", false, false },
		{ " True ", true, false },
		{ " FALSE ", false, false },
		{ "yes", false, true } // invalid input should throw
	};
	
	public static TheoryData<string, string, object> AssignValueData => new()
	{
		// propertyName, input value, expected value
		{ "StringProp", "test", "test" },
		{ "IntProp", "123", 123 },
		{ "BoolProp", "true", true },
		{ "BoolInvertProp", "true", false } // Inverted bool: true becomes false.
	};
	
	// input, expected Name, Verbose, Count, Option, Sub.SubArg, helpDisplayed flag
	public static TheoryData<string, string?, bool, int, string?, string?, bool> ArgumentsControllerParseData => new()
	{
		{"-n John -v -c 42 --option=Hello -s -a SubValue", "John", true, 42, "Hello", "SubValue", false },
		{ "-h", null, false, 0, null, null, true } // When help flag is provided, help should be displayed.
	};
	
	public static TheoryData<string, char, string[]> TokensData()
	{
		var data = new TheoryData<string, char, string[]>();

		foreach (var (input, separator, expected) in BuildTokensValidation())
			data.Add(input, separator, expected);

		return data;
	}

	private static IEnumerable<(string input, char separator, string[] expected)> BuildTokensValidation()
	{
		yield return ("", '=', []);
		yield return ("   ", '=', []);
		yield return ("token", '=', ["token"]);
		yield return ("=", '=', ["="]);
		yield return ("arg = value", '=', ["arg", "=", "value"]);
		yield return ("arg = \"value\"", '=', ["arg", "=", "value"]);
		yield return ("arg==value", '=', ["arg", "=", "=", "value"]);
		yield return ("  token1=token2  ", '=', ["token1", "=", "token2"]);
		yield return ("--arg=\"some=value\"", '=', ["--arg", "=", "some=value"]);
		yield return ("arg= \"multiple   spaces\" ", '=', ["arg", "=", "multiple   spaces"]);
		yield return ("--arg=value -a = value --ba= \"value with space\"", '=', ["--arg", "=", "value", "-a", "=", "value", "--ba", "=", "value with space"]);
	}
}