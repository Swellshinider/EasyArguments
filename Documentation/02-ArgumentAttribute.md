# ArgumentAttribute

[← Previous: ArgumentsControllerAttribute](./01-ArgumentsControllerAttribute.md) • [[Source: ArgumentAttribute.cs]](../EasyArguments/Attributes/ArgumentAttribute.cs) • [Next: ExecutorAttribute →](./03-ExecutorAttribute.md)

## Table of Contents

- [ArgumentAttribute](#argumentattribute)
  - [Table of Contents](#table-of-contents)
  - [Attribute Properties](#attribute-properties)
  - [Usage](#usage)
      - [Simple String Argument](#simple-string-argument)
      - [Required Number Argument](#required-number-argument)
      - [Boolean Flag with Inversion](#boolean-flag-with-inversion)
  - [Validation Rules](#validation-rules)
  - [Positional Arguments](#positional-arguments)
        - [Example:](#example)
  - [Error Conditions](#error-conditions)

## Attribute Properties

Property|Type|Default|Description
---|---|---|---
ShortName|string|null|Single-dash prefix (e.g. `-v`)
LongName|string|null|Double-dash prefix (e.g. `--verbose`)
HelpMessage|string|null|Description shown in help output
Required|bool|false|Enforce mandatory argument
InvertBoolean|bool|false|If set to `true`, this property inverts the default boolean value of the argument, making it `true` instead of `false`.

## Usage

#### Simple String Argument


```csharp
[ArgumentsController(Name = "app_name.exe")]
public class MyArguments
{
    [Argument("-n", "--name", "User's full name")]
    public string? Name { get; set; }
}
```

#### Required Number Argument

```csharp
[ArgumentsController(Name = "app_name.exe")]
public class MyArguments
{
    [Argument("-p", "--port", "Listening port", Required = true)]
    public int Port { get; set; }
}
```

#### Boolean Flag with Inversion

```csharp
[ArgumentsController(Name = "app_name.exe")]
public class MyArguments
{
    [Argument(null, "--no-log", "Disable logging", InvertBoolean = true)]
    public bool EnableLogging { get; set; } // --no-log → EnableLogging = false
}
```

## Validation Rules

1. **Required Arguments:** Throws `ArgumentException` if missing
2. **Name Generation:** Auto-generates long name if both names null
    ```csharp
    // Property 'Timeout' becomes "--timeout"
    [Argument(null, null, "Operation timeout")]
    public int Timeout { get; set; }
    ```
3. **Type Conversion:** Automatic for primitives (`int`, `bool`, etc.)
4. **Boolean Handling:**
    - No value → Sets to true (--verbose)
    - With InvertBoolean → Sets to false (--no-log)
    Only works for `bool` types, nullable booleans like `bool?` will not be considered

## Positional Arguments

Arguments are positional, which means that if you do not specify the argument name and only provide the value, it should still work.

```csharp
[ArgumentsController(Name = "app_name.exe")]
public class MyArguments
{
    [Argument(null, "--file-name", "Input file path", Required = true)]
    public string? FilePath { get; set; }
}
```

##### Example: 

  - Works with:
    ```bash
    app_name.exe "/data/file.txt"
    ```
  - Also works:
    ```bash
    app_name.exe --file-name="/data/file.txt"
    ```

## Error Conditions

- **MissingControllerException:** Property missing attribute
- **ArgumentException:** Required argument not provided
- **FormatException:** Invalid value type conversion

[← Previous: ArgumentsControllerAttribute](./01-ArgumentsControllerAttribute.md) • [[Source: ArgumentAttribute.cs]](../EasyArguments/Attributes/ArgumentAttribute.cs) • [Next: ExecutorAttribute →](./03-ExecutorAttribute.md)