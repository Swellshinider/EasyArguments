namespace EasyArguments.Tests.DataTest;

public class ExecutorClass_Test
{
    public static string RemoveFirstCharacter(string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        if (value.Length == 1)
            return string.Empty;

        return value[1..];
    }

    public static string SplitStringInHalf(string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;
        return value[..(value.Length / 2)];
    }
}