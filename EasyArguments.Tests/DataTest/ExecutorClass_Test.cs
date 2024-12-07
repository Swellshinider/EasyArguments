namespace EasyArguments.Tests.DataTest;

public class ExecutorClass_Test()
{
    public string RemoveFirstCharacter(string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        if (value.Length == 1)
            return string.Empty;

        return value[1..];
    }
}