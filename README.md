<div align="center">

# EasyArguments

[![NuGet](https://img.shields.io/nuget/v/EasyArguments.svg)](https://www.nuget.org/packages/EasyArguments/)


EasyArguments is a lightweight .NET library that simplifies the process of parsing command-line arguments into strongly-typed objects. It provides attributes to define metadata for arguments and a controller to handle the parsing logic.

</div>

### Table of Contents

- [EasyArguments](#easyarguments)
    - [Table of Contents](#table-of-contents)
  - [Installation](#installation)
  - [Usage](#usage)
    - [Configuring the Controller](#configuring-the-controller)
    - [Defining Arguments](#defining-arguments)
    - [Parsing Arguments](#parsing-arguments)
    - [Handling Errors](#handling-errors)
  - [Full Example](#full-example)
  - [Contribution](#contribution)
    - [How to Contribute](#how-to-contribute)
  - [License](#license)

## Installation

Nuget package coming...

## Usage


### Configuring the Controller

Use the `ArgumentsControllerAttribute` to configure the behavior of the class that defines the arguments. You can specify whether the order of arguments should be respected, whether an automatic help argument should be included, and the character used as a separator.

```csharp
using EasyArguments.Attributes;

[ArgumentsController(RespectOrder = true, AutoHelpArgument = true, Separator = '=')]
public class MyArguments
{
    // Argument definitions
}
```

### Defining Arguments

To define command-line arguments, use the `ArgumentAttribute` on properties within a class. You can specify short and long names, help messages, and whether the argument is required.

```csharp
using EasyArguments.Attributes;

[ArgumentsController(RespectOrder = true, AutoHelpArgument = true, Separator = '=')]
public class MyArguments
{
    [Argument("-n", "--name", "Specifies the name.", Required = true)]
    public string Name { get; set; }

    [Argument("-a", "--age", "Specifies the age.", Required = true)]
    public int Age { get; set; }

    [Argument(null, "--verbose", "Enables verbose mode.")]
    public bool Verbose { get; set; }

    [Argument("-v", "--version", "Displays application version.")]
    public bool Version { get; set; }
}
```


### Parsing Arguments

Use the `ArgumentsController` class to parse the command-line arguments into an instance of your class.

```csharp
using EasyArguments;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            MyArguments parsedArgs = ArgumentsController.Parse<MyArguments>(args);
            // Use parsedArgs
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
```

### Handling Errors

The `ArgumentsController` provides a mechanism to handle errors. By default, errors are thrown as exceptions. You can redirect errors to the console by setting `ArgumentsController.RedirectErrorToConsole` to `true`.

```csharp
using EasyArguments;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            
            ArgumentsController.RedirectErrorToConsole = true; // Errors will be shown in the console
            MyArguments parsedArgs = ArgumentsController.Parse<MyArguments>(args);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
```

## Full Example

Here is a complete example:

```csharp
using EasyArguments;
using EasyArguments.Attributes;
using System;

[ArgumentsController(RespectOrder = true, AutoHelpArgument = true, Separator = '=')]
public class MyArguments
{    
    [Argument("-n", "--name", "Specifies the name.", Required = true)]
    public string Name { get; set; }

    [Argument("-a", "--age", "Specifies the age.", Required = true)]
    public int Age { get; set; }

    [Argument(null, "--verbose", "Enables verbose mode.")]
    public bool Verbose { get; set; }

    [Argument("-v", "--version", "Displays application version.")]
    public bool Version { get; set; }
}

class Program
{
    // args = ["-n=EasyArguments", "-age=1", "--verbose"]
    static void Main(string[] args)
    {
        ArgumentsController.RedirectErrorToConsole = true;

        try
        {
            var parsedArgs = ArgumentsController.Parse<MyArguments>(args);
            Console.WriteLine($"Name: {parsedArgs.Name}");
            Console.WriteLine($"Age: {parsedArgs.Age}");
            Console.WriteLine($"Verbose: {parsedArgs.Verbose}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected Error: {ex.Message}");
        }
    }
}
```

## Contribution

Contributions are welcome! If you have ideas to improve EasyArguments feel free to open an issue.

### How to Contribute

- Fork the repository.
- Create a issue
- Create a new branch for your issue.
- Submit a pull request with a detailed explanation of your changes.
- :)

## License

This project is licensed under the GPLv3 License. See the [LICENSE](./LICENSE) file for details.