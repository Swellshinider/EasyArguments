namespace EasyArguments.Tests;

public static class DataCreator
{
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