# ArgumentsExecution

- [← Back to Overview](../Overview.md)

## Table of Contents

- [ArgumentsExecution](#argumentsexecution)
	- [Table of Contents](#table-of-contents)
	- [Overview](#overview)
	- [Execution Flow](#execution-flow)
	- [Usage Examples](#usage-examples)
		- [Example 1: Basic Executor Usage](#example-1-basic-executor-usage)
		- [Example 2: Multiple Executors on a Single Property](#example-2-multiple-executors-on-a-single-property)
		- [Example 3: Executor on a Nested Argument](#example-3-executor-on-a-nested-argument)

## Overview

The Arguments Execution process in **EasyArguments** refers to the mechanism that automatically invokes executor methods after a property’s value has been assigned. 
Executors allow you to perform additional processing, validation, or transformation on the parsed argument values. They are defined using the [ExecutorAttribute](../03-ExecutorAttribute.md) on a property and are executed immediately after the property is assigned during parsing.

## Execution Flow

1. **Value Assignment:**  
   When a command-line argument is parsed and a value is assigned to a property, the framework checks if any executor methods are attached to that property.

2. **Executor Invocation:**  
   The `Execute(object target)` method in the `PropertyBinding` class iterates over all `ExecutorAttribute` instances associated with the property. For each executor:
   - It verifies that the static method exists in the provided static class.
   - It determines the expected number of parameters:
	 - **Zero parameters:** The executor method is invoked without any input.
	 - **One parameter:** The current property value is passed as an argument.
	 - **More than one parameter:** A `TargetParameterCountException` is thrown.

3. **Result Assignment:**  
   If an executor attribute has `AssignResultToProperty` set to `true`, the return value of the executor method replaces the current value of the property. This is useful for transforming or sanitizing input values.

4. **Synchronous Execution:**  
   All executor methods are executed synchronously as part of the parsing process. This ensures that any validations or transformations occur immediately.

## Usage Examples

### Example 1: Basic Executor Usage

Consider a scenario where you want to append a string to a user-provided message.

```csharp
public static class MessageTransformers
{
	public static string AppendGreeting(string input)
	{
		return input + " - Welcome!";
	}
}

[ArgumentsController(Name = "app.exe")]
public class MyArguments
{
	[Argument("-m", "--message", "User message")]
	[Executor(typeof(MessageTransformers), "AppendGreeting", AssignResultToProperty = true)]
	public string Message { get; set; }
}
```

**Run:**

```bash
> -m "My Message"
```

**Output:**
```
My Message - Welcome!
```

**Execution Flow:**

**1.** When `-m "My Message"` is parsed, Message is initially set to **"My Message"**.
**2.** The executor method AppendGreeting is then invoked with **"My Message"** as its parameter.
**3.** The returned value **"My Message - Welcome!"** is assigned back to the Message property.

### Example 2: Multiple Executors on a Single Property

This example demonstrates how multiple executor attributes can be applied to a single property to perform sequential processing. In this scenario, the first executor logs the value, the second executor transforms the value to uppercase, and the third logs the value again

```csharp
public static class Logger
{
	public static void LogValue(string input)
	{
		Console.WriteLine("Logging value: " + input);
	}
}

public static class Transformer
{
	public static string ToUpperCase(string input)
	{
		return input.ToUpperInvariant();
	}
}
```

```csharp
[ArgumentsController(Name = "app.exe")]
public class MyArguments
{
	[Argument("-v", "--value", "Some value")]
	[Executor(typeof(Logger), "LogValue")] // Logs the value
	[Executor(typeof(Transformer), "ToUpperCase", AssignResultToProperty = true)] // Transforms the value to uppercase.
	[Executor(typeof(Logger), "LogValue")] // Logs again
	public string? Value { get; set; }
}
```

**Run:**

```bash
> -v "hello"
```

**Output:**
```
Logging value: Hello
Logging value: HELLO
```

**Execution Flow:**

1. When --value "hello" is provided, the property's Value is first set to "hello".
2. The first executor (LogValue) is invoked, logging the original value.
3. Next, the second executor (ToUpperCase) is called with the value "hello", and returns "HELLO".
4. Since AssignResultToProperty is set to true for the second executor, the property's value is updated to "HELLO".
5. The third executor (LogValue) is invoked, logging the new value.

### Example 3: Executor on a Nested Argument

Executors can also be applied to properties within nested argument classes. This allows you to perform specialized processing on values that belong to a subgroup of arguments.

```csharp
public static class PathResolver
{
	public static void CheckAdvancedOptions(AdvancedOptions advancedOptions)
	{
		for (int i = 0; i < advancedOptions.RepeatCount; i++)
		{
			Console.WriteLine(advancedOptions.LogPath);
		}
	}
}

[ArgumentsController(Name = "app.exe")]
public class MyArguments
{
	[Argument("-a", "--advanced", "Advanced configuration options")]
	[Executor(typeof(PathResolver), "CheckAdvancedOptions")]
	public AdvancedOptions? Advanced { get; set; }
}

public class AdvancedOptions
{
	[Argument("-p", "--path", "Log file path")]
	public string? LogPath { get; set; }
	
	[Argument("-i", null, "Repeat logs", Required = true)]
	public int RepeatCount { get; set; }
}
```

**Run:**

```bash
> --advanced -p "path to heaven" -i 5
```

**Output:**
```
path to heaven
path to heaven
path to heaven
path to heaven
path to heaven
```

**Note:**

Executors for nested arguments are executed lazily. This means that the nested object is first fully generated and all its properties are populated. Only at the end of the parsing process are the executor methods for the nested arguments invoked. This ensures that all nested values are available before any transformations or validations occur.

- [← Back to Overview](../Overview.md)