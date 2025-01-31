<div align="center">

# EasyArguments

[![GitHub](https://img.shields.io/github/license/swellshinider/EasyArguments)](https://github.com/swellshinider/EasyArguments/blob/main/LICENSE) [![NuGet download count](https://img.shields.io/nuget/dt/EasyArguments)](https://www.nuget.org/packages/EasyArguments) [![NuGet](https://img.shields.io/nuget/v/EasyArguments.svg)](https://www.nuget.org/packages/EasyArguments/) [![Build](https://github.com/Swellshinider/EasyArguments/actions/workflows/dotnet-desktop.yml/badge.svg?branch=main&event=push)](https://github.com/Swellshinider/EasyArguments/actions/workflows/dotnet-desktop.yml)

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
  - [Plans for the future](#plans-for-the-future)
  - [Contribution](#contribution)
    - [How to Contribute](#how-to-contribute)
  - [License](#license)

## Installation

```bash
dotnet add package EasyArguments
```

## Usage


### Configuring the Controller

Use the `ArgumentsControllerAttribute` to configure the behavior of the class that defines the arguments. You can specify whether an automatic help argument should be included, and the character used as a separator.

```csharp
using EasyArguments.Attributes;

[ArgumentsController(AutoHelpArgument = true, Separator = '=')]
public class MyArgs
{
    // Argument definitions
}
```

### Defining Arguments

To define command-line arguments, use the `ArgumentAttribute` on properties within a class. You can specify short and long names, help messages, and whether the argument is required.

```csharp
using EasyArguments.Attributes;

[ArgumentsController(AutoHelpArgument = true, Separator = '=')]
public class MyArguments
{
    // A string argument with both short and long forms:
    [Argument("-n", "--name", "Specifies the user name", Required = true)]
    public string? Name { get; set; }

    // A boolean argument that defaults to false unless called:
    [Argument("-v", "--verbose", "Enable verbose output", Required = false)]
    public bool? Verbose { get; set; }

    // A boolean argument that is "inverted," meaning it defaults to true
    // and becomes false if specified:
    [Argument(null, "--no-gui", "Disable the GUI", InvertBoolean = true)]
    public bool GuiEnabled { get; set; }

    // A sub-command (nested class) holding its own arguments:
    [Argument(null, "start", "Start command options")]
    public StartArgs? Start { get; set; }
}

public class StartArgs
{
    // Arguments that only apply when "start" is used:
    [Argument("-u", "--url", "URL of the service")]
    public string? Url { get; set; }

    [Argument("-o", "--output", "Output directory")]
    public string? Output { get; set; }
}
```


### Parsing Arguments

Create an `ArgumentsController` instance and call the method `Parse(string[] args)` to parse the command-line arguments into an instance of your class.

```csharp
using EasyArguments;

static void Main(string[] args)
{
    // Instantiate a controller for your argument class
    var controller = new ArgumentsController<MyArgs>(args);

    // Parse the given args
    var parsed = controller.Parse();

    // Now you can use the strongly-typed properties:
    Console.WriteLine($"Name: {parsed.Name}");
    Console.WriteLine($"Verbose: {parsed.Verbose}");
    Console.WriteLine($"GUI enabled? {parsed.GuiEnabled}");
    
    // If the user included "start" on the CLI, 
    // then parsed.Start != null and has its own parsed values:
    if (parsed.Start != null)
    {
        Console.WriteLine($"Starting with URL={parsed.Start.Url}, output={parsed.Start.Output}");
    }
}
```

### Handling Errors

The `ArgumentsController` provides a mechanism to handle errors that are thrown as exceptions. You can simply print the error message to the console to get a nice result.

```csharp
using EasyArguments;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            // Instantiate a controller for your argument class
            var controller = new ArgumentsController<MyArgs>(args);

            // Parse the given args
            MyArguments parsed = controller.Parse();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
```
OUTPUT:

```bash
> -a
Unknown argument '-a'. Please use -h or --help for usage information.
```

## Full Example

Here is a complete example:

```csharp
using EasyArguments;
using EasyArguments.Attributes;
using System;

[ArgumentsController]
public class MyArgs
{
    [Argument("-n", "--name", "Specifies the user name", Required = true)]
    public string? Name { get; set; }

    [Argument("-v", "--verbose", "Enable verbose output", Required = false)]
    public bool? Verbose { get; set; }

    [Argument(null, "--no-gui", "Disable the GUI", InvertBoolean = true)]
    public bool GuiEnabled { get; set; }

    [Argument(null, "start", "Start command options")]
    public StartArgs? Start { get; set; }
}

public class StartArgs
{
    // Arguments that only apply when "start" is used:
    [Argument("-u", "--url", "URL of the service")]
    public string? Url { get; set; }

    [Argument("-o", "--output", "Output directory")]
    public string? Output { get; set; }
}


class Program
{
    static void Main(string[] args)
    {
        try
        {
            // Instantiate a controller for your argument class
            var controller = new ArgumentsController<MyArgs>(args);

            // Parse the given args
            var parsed = controller.Parse();

            // Now you can use the strongly-typed properties:
            Console.WriteLine($"Name: {parsed.Name}");
            Console.WriteLine($"Verbose: {parsed.Verbose}");
            Console.WriteLine($"GUI enabled? {parsed.GuiEnabled}");
            
            // If the user included "start" on the CLI, 
            // then parsed.Start != null and has its own parsed values:
            if (parsed.Start != null)
            {
                Console.WriteLine($"Starting with URL={parsed.Start.Url}, output={parsed.Start.Output}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
```

## Plans for the future

- **Enhanced Help Customization** (Issue #9)
    While `EasyArguments` already generate automatic usage information, I want to give developers more control over the help text’s layout and content. Plans include support for: 
    - Customizing headings, sections, and formatting in the auto-generated help.
    - Displaying examples inline with each argument’s description.
    - Condition-based or role-based help, where certain arguments appear only when relevant.

## Contribution

Contributions are welcome! If you have ideas to improve EasyArguments feel free to open an issue.

### How to Contribute

- Fork the repository.
- Create a issue or get an existing one
- Create a new branch for your issue.
- Submit a pull request with a detailed explanation of your changes.
- :)

## License

This project is licensed under the GPLv3 License. See the [LICENSE](./LICENSE) file for details.
