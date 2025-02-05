# Extensions

[← Previous: PropertyBinding](./05-PropertyBinding.md) • [[Source: Extensions.cs]](../EasyArguments/Helper/Extensions.cs)

## Table of Contents

- [Extensions](#extensions)
	- [Table of Contents](#table-of-contents)
	- [Overview](#overview)
	- [Key Extension Methods](#key-extension-methods)
			- [ToBoolean](#toboolean)
			- [IsBoolean:](#isboolean)
			- [Tokenize:](#tokenize)
			- [ExtractProperties:](#extractproperties)
	- [Usage Examples](#usage-examples)
			- [Boolean Conversion](#boolean-conversion)
			- [Tokenizing a Command-Line String](#tokenizing-a-command-line-string)
			- [Extracting Argument Properties](#extracting-argument-properties)
			- [Generating Usage Text](#generating-usage-text)
	- [Notes](#notes)

## Overview

The `Extensions` class in EasyArguments provides a suite of extension methods that simplify common tasks such as type conversion, tokenizing command-line input, and extracting metadata from argument-decorated properties. These helpers facilitate robust command-line parsing without relying on regular expressions.

## Key Extension Methods

#### ToBoolean

```csharp
public static bool ToBoolean(this string value)
```

- **Description:**
	Converts a string to a boolean value. Accepts "true" or "false" (case-insensitive, with trimming).
- **Error Handling:**
	Throws an ArgumentException if the string does not represent a valid boolean.

#### IsBoolean:
	
```csharp
public static bool IsBoolean(this Type t)
```

- **Description:**
	Returns true if the provided type is either bool or bool? (nullable boolean).

#### Tokenize:
	
```csharp
public static List<string> Tokenize(this string input, char separator)
```

- **Description:**
	Splits the input string into tokens based on whitespace and a specified separator.

- **Features:**
	- Respects quoted substrings so that spaces or separator characters inside quotes are preserved.
	- Does not use regular expressions.
	
#### ExtractProperties:
	
```csharp
public static IEnumerable<PropertyBinding> ExtractProperties(this Type targetType, PropertyBinding? parent = null)
```

- **Description:**
	Retrieves all public instance properties decorated with `ArgumentAttribute` from the specified type.
- **Features:**
	- Generates a default long name (prefixed with `--`) if both the short and long names are missing.
	- Recursively extracts properties for nested argument classes.

## Usage Examples

#### Boolean Conversion

```csharp
bool flag = "true".ToBoolean(); // returns true
```

#### Tokenizing a Command-Line String

```csharp
string input = "--arg=value -a = \"value with space\"";
char separator = '=';
List<string> tokens = input.Tokenize(separator);
// Expected tokens:
// ["--arg", "=", "value", "-a", "=", "value", "--ba", "=", "value with space"]
```

#### Extracting Argument Properties

```csharp
var bindings = typeof(MyArgs).ExtractProperties();

foreach (var binding in bindings)
		Console.WriteLine(binding.Usage());
```

#### Generating Usage Text

```csharp
var usageBuilder = bindings.GetUsage();
Console.WriteLine(usageBuilder.ToString());
```

## Notes

- **Internal Helpers:**
	Private methods such as HandleQuotes, HandleWhitespace, and HandleSeparator support the Tokenize method. These are not exposed for public use.

- **Partial Class:**
	The Extensions class is defined as partial, allowing additional helper methods to be added in other parts of the project if needed.

- **Non-RegEx Implementation:**
	The tokenization logic is implemented without regular expressions to maintain clarity and performance.

[← Previous: PropertyBinding](./05-PropertyBinding.md) • [[Source: Extensions.cs]](../EasyArguments/Helper/Extensions.cs)