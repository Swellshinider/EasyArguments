# EasyArguments

**EasyArguments** is a lightweight library for managing and parsing input arguments in .NET projects. It provides flexible argument parsing, support for strict argument order, and the ability to execute methods based on parsed arguments.

---

## Features

- Flexible mapping of command-line arguments to properties.
- Attribute-based configuration for argument definitions.
- Support for strict argument order (`RespectOrder`).
- Method execution via attributes (`ArgumentExecutorAttribute`).
- Built-in type conversion and validation.
- Clear and user-friendly exception handling for invalid or missing arguments.

---

## Installation

Add the library to your project via NuGet (coming soon) or include the source code directly.

---

## Usage

### Defining Your Arguments Controller
Decorate a class with `[ArgumentsController]` and define properties annotated with `[Argument]` to specify the accepted arguments.

```csharp
[ArgumentsController]
public class MyArguments
{
    [Argument("-n", "--name", Description = "The user's name")]
    public string? Name { get; set; }

    [Argument("-a", "--age", Description = "The user's age")]
    public int? Age { get; set; }

    [Argument("-v", "--verbose", Description = "Enable verbose mode")]
    public bool Verbose { get; set; }
}
```

### Parsing Arguments
Create an instance of `ArgumentsController` and parse arguments.

```csharp
public static class Program 
{
	// args -> -n=John -a=30 -v
	public static void Main(string[] args) 
	{
		var parser = new ArgumentsController<MyArguments>(args);
		var result = parser.Parse();
		
		Console.WriteLine($"Name: {result.Name}");
		Console.WriteLine($"Age: {result.Age}");
		Console.WriteLine($"Verbose: {result.Verbose}");
	}
}

```

Output:
```
Name: John
Age: 30
Verbose: True
```

### Using Method Execution
Add `[ArgumentExecutor]` to enable method execution based on parsed arguments.

```csharp
public static class ExecutorClass
{
    public static string FormatName(string name)
    {
        return name.ToUpper();
    }

    public static int DoubleAge(int age)
    {
        return age * 2;
    }
}

[ArgumentsController]
public class ExecutableArguments
{
    [Argument("-n", "--name")]
    [ArgumentExecutor(typeof(ExecutorClass), "FormatName")]
    public string? Name { get; set; }

    [Argument("-a", "--age")]
    [ArgumentExecutor(typeof(ExecutorClass), "DoubleAge")]
    public int? Age { get; set; }
}
```

```csharp
public static class Program 
{
	// args -> -n=John -a=30
	public static void Main(string[] args) 
	{
		var argumentsController = new ArgumentsController<MyArguments>(args);
		var result = argumentsController.Parse();
		var output = argumentsController.Execute(result);
		
		foreach (var output in outputs)
		{
            Console.WriteLine(output);
		}
	}
}
```

Output:
```
JOHN
60
```

### Strict Argument Order (`RespectOrder`)
Enable strict argument order by setting `RespectOrder = true` in the `ArgumentsController` attribute.

```csharp
[ArgumentsController(RespectOrder = true)]
public class OrderedArguments
{
    [Argument("-x", "--first")]
    public string? First { get; set; }

    [Argument("-y", "--second")]
    public string? Second { get; set; }
}

var args = new[] { "-x=Hello", "-y=World" };
var argumentsController = new ArgumentsController<OrderedArguments>(args);
var result = argumentsController.Parse();

Console.WriteLine($"First: {result.First}, Second: {result.Second}");
```

Invalid order will throw an `IncorrectArgumentOrderException`.

---

## Exception Handling

### Common Exceptions

- `MissingArgumentsControllerAttributeException`: Thrown if the target class is not decorated with `[ArgumentsController]`.
- `UnknownArgumentException`: Thrown if an unrecognized argument is provided.
- `InvalidArgumentTypeException`: Thrown if an argument cannot be converted to the required type.
- `MissingRequiredArgumentException`: Thrown if a required argument is missing.
- `IncorrectArgumentOrderException`: Thrown if `RespectOrder` is `true` and arguments are provided in the wrong order.

---

## TODO List

- [x] - Execute methods based on parsed arguments
- [ ] - Automatic help message to display usage of arguments
- [ ] - Property be able to receive enum types
- [ ] - Better executor (be able to insert more than one parameter to the target method)

## Contribution

We welcome contributions to the ArgumentsController Library! If you have an idea for a new feature, find a bug, or want to improve the documentation, feel free to create a pull request or open an issue.

#### Steps to Contribute

- Fork the repository.

- Create a new branch for your feature or bugfix: git checkout -b my-feature-branch

- Make your changes and add tests if applicable.

- Commit your changes: git commit -m "Add feature X"

- Push the branch to your fork: git push origin my-feature-branch

- Open a pull request and describe your changes.

We will review your PR and work with you to merge it into the main branch!

## License

This project follows GPL-3.0 License. See the `LICENSE` file for more details.