# ExecutorAttribute

[← Previous: ArgumentAttribute](./02-ArgumentAttribute.md) • [[Source: ExecutorAttribute.cs]](../EasyArguments/Attributes/ExecutorAttribute.cs) • [Next: ArgumentsController →](./04-ArgumentsController.md)

## Table of Contents

- [ExecutorAttribute](#executorattribute)
  - [Table of Contents](#table-of-contents)
  - [Key Features](#key-features)
  - [Attribute Properties](#attribute-properties)
  - [Constructor](#constructor)
  - [Usage](#usage)
      - [Basic Validation Example:](#basic-validation-example)
      - [Value Transformation Example:](#value-transformation-example)
  - [Method Requirements](#method-requirements)
  - [Execution Flow](#execution-flow)
  - [Error Handling](#error-handling)
  - [Common Use Cases](#common-use-cases)

## Key Features

- **Method Chaining:** Multiple executors per property
- **Result Handling:** Optional result assignment back to property
- **Static Enforcement:** Type-safe execution via static classes
- **Dependency Injection:** Integrate external logic without instance coupling

## Attribute Properties

Property|Type|Description
---|---|---
StaticClass|Type|Class containing static method
MethodInfo|MethodInfo|Method information that will be executed
AssignResultToProperty|bool|Whether to overwrite property value with result

## Constructor

```csharp
public ExecutorAttribute(Type staticClass, string methodName)
```

- **staticClass:** Must be a static class
- **methodName:** Public static method name with matching signature

## Usage 

#### Basic Validation Example:

```csharp
[ArgumentsController(Name = "app_name.exe")]
public class Config 
{
    [Argument("-p", "--port", "Network port")]
    [Executor(typeof(Validators), "ValidatePort")]
    public int Port { get; set; }
}
```

```csharp
public static class Validators 
{
    public static void ValidatePort(int port) 
    {
        if (port < 1024) 
            throw new ArgumentException("Privileged port");
    }
}
```

#### Value Transformation Example:

```csharp
[ArgumentsController(Name = "app_name.exe")]
public class FileArgs 
{
    [Argument("-f", "--file", "Input file")]
    [Executor(typeof(PathHelper), "ResolvePath",  AssignResultToProperty = true)]
    public string? FilePath { get; set; }
}
```

```csharp
public static class PathHelper 
{
    public static string ResolvePath(string input) 
    {
        return Path.GetFullPath(input);
    }
}
```

## Method Requirements

**1.** **Static Modifier:** Must be public static
**2.** **Parameter Matching:** 
  - Single parameter matching property type
  - Return type should match property type if using `AssignResultToProperty`

## Execution Flow

**1.** Argument is parsed.
**2.** Property value assigned.
**3.** Executor methods invoked in declaration order
**4.** If `AssignResultToProperty`:

  ```csharp
  // Original value → method input
  var result = MethodInfo.Invoke(null, [value]);
  // Result → new property value
  ```

## Error Handling

- ArgumentException thrown for:

  - Non-static classes
  - Missing methods
  - Invalid method signatures

- Exceptions in executor methods bubble up to parser

## Common Use Cases

- Input sanitization
- Complex validation
- Cryptographic operations
- Environment resolution
- Configuration file loading

[← Previous: ArgumentAttribute](./02-ArgumentAttribute.md) • [[Source: ExecutorAttribute.cs]](../EasyArguments/Attributes/ExecutorAttribute.cs) • [Next: ArgumentsController →](./04-ArgumentsController.md)