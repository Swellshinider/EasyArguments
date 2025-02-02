# ArgumentsControllerAttribute

[← Back to Overview](Overview.md) • [[Source File]](../EasyArguments/Attributes/ArgumentsControllerAttribute.cs) • [Next: ArgumentAttribute →](./02-ArgumentAttribute.md)

## Table of Contents

- [ArgumentsControllerAttribute](#argumentscontrollerattribute)
  - [Table of Contents](#table-of-contents)
  - [Attribute Properties](#attribute-properties)
  - [Usage](#usage)


## Attribute Properties

Property|Type|Default|Description
---|---|---|---
Name|string|True|Represents the name that will be displayed when user requests help usage.
AutoHelpArgument|bool|False|Indicates whether an automatic help argument should be included.
ExecuteWhenParsing|bool|False|Indicates whether the command should be executed when parsing.
Separator|bool|=|Indicates the character used as a separator for parsing arguments.

## Usage

Simply declare the attribute above a `public class`. The *Name* parameter specifies the application's name, which will be displayed when the user invokes the `--help` option (only if `AutoHelpArgument` is set to true).

```csharp
[ArgumentsController(Name = "app_name.exe")]
public class MyArguments
{

}
```

Example using all the properties with their default values:

```csharp
[ArgumentsController(
    Name = "app_name.exe", 
    AutoHelpArgument = True, 
    ExecuteWhenParsing = True, 
    Separator = '=')]
public class MyArguments
{

}
```

[← Back to Overview](Overview.md) • [[Source File]](../EasyArguments/Attributes/ArgumentsControllerAttribute.cs) • [Next: ArgumentAttribute →](./02-ArgumentAttribute.md)