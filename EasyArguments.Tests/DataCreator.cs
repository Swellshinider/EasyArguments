namespace EasyArguments.Tests;

public static class DataCreator 
{
	public static TheoryData<string, string[]> TokensData() 
	{
		var data = new TheoryData<string, string[]>();
		
		foreach (var (input, expected) in BuildTokensValidation())
			data.Add(input, expected);
		
		return data;
	}
	
	private static IEnumerable<(string input, string[] expected)> BuildTokensValidation() 
	{
		yield return ("--arg=\"text\"", new string[] { "--arg=\"text\"" });
		yield return ("--arg =\"text\"", new string[] { "--arg", "=\"text\"" });
		yield return ("--arg= \"text\"", new string[] { "--arg=", "text" });
		yield return ("--arg = \"text\"", new string[] { "--arg", "=", "text" });
		yield return ("--arg \"value with spaces\"", new string[] { "--arg", "value with spaces" });
	}
}