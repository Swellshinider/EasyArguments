# ArgumentsController

[← Previous: ExecutorAttribute](./03-ExecutorAttribute.md) • [[Source: ArgumentsController.cs]](../EasyArguments/ArgumentsController.cs) • [Next: PropertyBinding →](./05-PropertyBinding.md)

## Table of Contents

- [ArgumentsController](#argumentscontroller)
  - [Table of Contents](#table-of-contents)
  - [Overview](#overview)
  - [Key Features](#key-features)
  - [Constructors](#constructors)
      - [ArgumentsController(string\[\] args)](#argumentscontrollerstring-args)
      - [ArgumentsController(string argLine)](#argumentscontrollerstring-argline)
  - [Properties](#properties)
  - [Core Methods](#core-methods)
      - [Parse()](#parse)
      - [Parse(out bool helpMessageDisplayed)](#parseout-bool-helpmessagedisplayed)
      - [GetUsageText()](#getusagetext)
      - [ExtractBoundProperties()](#extractboundproperties)
  - [Usage Example](#usage-example)
  - [Handling Nested Arguments](#handling-nested-arguments)
  - [Error Handling](#error-handling)

## Overview

The core class that handles command-line argument parsing. Responsible for:

- Mapping values to decorated properties
- Validating required arguments
- Generating colored help screens
- Handling nested argument structures
- Automatic boolean inversion logic
 
## Key Features

- **Dual Input Support:** Accepts both `string[]` and concatenated `string` arguments
- **Color Customization:** Control help screen colors via properties
- **Nested Arguments:** Recursively parse complex object hierarchies
- **Auto-Help:** Built-in `-h, --help` support with `AutoHelpArgument` flag from [ArgumentsControllerAttribute](./01-ArgumentsControllerAttribute.md) 
- **Boolean Handling:** Automatic `true/false` parsing with inversion support

## Constructors

#### ArgumentsController(string[] args)

Initializes with command-line argument array

```csharp
public ArgumentsController(string[] args)
```

#### ArgumentsController(string argLine)

Initializes with single-line argument string

```csharp
public ArgumentsController(string argLine)
```

## Properties

Property|Type|Default|Description
---|---|---|---
ApplicationNameColor|ConsoleColor|Cyan|Color for application name in help output
RequiredArgumentsHighlightColor|ConsoleColor|Magenta|Color for required arguments section
OptionalArgumentsHighlightColor|ConsoleColor|Green|Color for optional arguments section

## Core Methods

#### Parse()

Main parsing method that returns populated arguments object

```csharp
public T Parse()
```

#### Parse(out bool helpMessageDisplayed)

Advanced parse with help display detection

```csharp
public T Parse(out bool helpMessageDisplayed)
```

#### GetUsageText()

Generates formatted help documentation

```csharp
public StringBuilder GetUsageText()
```

#### ExtractBoundProperties()

Retrieves metadata about configured arguments

```csharp
public IEnumerable<PropertyBinding> ExtractBoundProperties()
```

## Usage Example

```csharp
var controller = new ArgumentsController<MyArgs>(args);
MyArgs parsed = controller.Parse();
```

## Handling Nested Arguments

The controller automatically detects and processes nested classes:

```csharp
[ArgumentsController(Name = "app_name.exe")]
public class MyArgs 
{
    [Argument(null, "advanced", "Advanced configuration")]
    public AdvancedOptions? Options { get; set; }
}
```

```csharp
public class AdvancedOptions 
{
    [Argument(null, "--threads", "Worker thread count")]
    public int ThreadCount { get; set; }
}
```

## Error Handling

Throws specific exceptions for common issues:

- **MissingControllerException:** Missing [ArgumentsControllerAttribute]
- **ArgumentException:** Missing required arguments or invalid values
- **FormatException:** Type conversion failures

[← Previous: ExecutorAttribute](./03-ExecutorAttribute.md) • [[Source: ArgumentsController.cs]](../EasyArguments/ArgumentsController.cs) • [Next: PropertyBinding →](./05-PropertyBinding.md)