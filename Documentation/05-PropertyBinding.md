# PropertyBinding

[← Previous: ArgumentsController](./04-ArgumentsController.md) • [[Source: PropertyBinding.cs]](../EasyArguments/Helper/PropertyBinding.cs) • [Next: Extensions →](./06-Extensions.md)

## Table of Contents

- [PropertyBinding](#propertybinding)
	- [Table of Contents](#table-of-contents)
	- [Overview](#overview)
	- [Key Features](#key-features)
	- [Constructors](#constructors)
		- [`PropertyBinding(PropertyInfo property, ArgumentAttribute argumentAttr, PropertyBinding? parent = null)`](#propertybindingpropertyinfo-property-argumentattribute-argumentattr-propertybinding-parent--null)
	- [Properties](#properties)
	- [Methods](#methods)
	- [Usage Example](#usage-example)

## Overview

`PropertyBinding` is a helper class in EasyArguments that ties together a target property and its associated `ArgumentAttribute`. It encapsulates metadata and functionality needed for value conversion, usage string formatting, nested argument handling, and executor invocation.

## Key Features

- **Property Association:** Binds a property to its corresponding `ArgumentAttribute` metadata.
- **Usage String Generation:** Provides methods to generate well-formatted usage strings for help screens.
- **Value Assignment:** Converts and assigns command-line argument values to properties.
- **Nested Argument Support:** Manages child bindings for nested argument classes.
- **Executor Support:** Invokes static executor methods defined via `ExecutorAttribute` after property assignment.

## Constructors

### `PropertyBinding(PropertyInfo property, ArgumentAttribute argumentAttr, PropertyBinding? parent = null)`

Creates a new instance of `PropertyBinding` by associating a given property with its argument metadata. The optional `parent` parameter is used when the property is part of a nested argument structure.

## Properties

Property|Type|Description
---|---|---
Property|PropertyInfo|The target property being bound.
ArgumentAttr|ArgumentAttribute|The argument attribute providing metadata for the property.
Parent|PropertyBinding?|The parent binding, if the property is part of a nested argument.
Children|IEnumerable of PropertyBinding| Collection of child bindings for nested argument types.
WaitingForLazyExecution|bool|Indicates if the property binding is pending lazy executor invocation.
IsParent|bool| Returns `true` if the binding has child bindings (i.e., represents a nested argument group).

## Methods

Method|Return Type| Description
---|---|---
Matches(string arg, char separator)|bool| Determines if a given command-line argument matches the binding’s short or long name (ignoring any attached values after the separator).
Usage()|string| Builds a formatted usage string that combines the argument names with the help message.
GetName()|string|Returns a nicely formatted string that includes both the short and long names (if available) for the argument.
GetAvailableName()|string|Retrieves the primary available name for the argument (prefers short name when available).
AssignValue(object target, object? value)|void|Converts the provided value to the property's type and assigns it to the target object. Supports primitives and boolean inversion logic.
AssignNested(object target, object? value)|void|Assigns a nested argument value to the target property and marks the binding for lazy execution of associated executor methods.
Execute(object target)|void|Invokes any executor methods attached via `ExecutorAttribute` on the property and assigns the result if required.
ToString()|string|Returns the formatted argument name (delegates to `GetName(`).                                                                        

## Usage Example

Below is a simplified example demonstrating how `PropertyBinding` might be used internally:

```csharp
// Assume MyArgs has a property decorated with ArgumentAttribute.
PropertyInfo propInfo = typeof(MyArgs).GetProperty("Timeout");
var argAttr = propInfo.GetCustomAttribute<ArgumentAttribute>();

// Create a PropertyBinding instance.
var binding = new PropertyBinding(propInfo, argAttr);

// Check if a given command-line argument matches the property.
bool isMatch = binding.Matches("--timeout", '=');

// Generate a usage string for help output.
string usageInfo = binding.Usage();

// Create an instance of MyArgs and assign a value.
var argsInstance = new MyArgs();
binding.AssignValue(argsInstance, "30");

// If executors are attached, execute them.
binding.Execute(argsInstance);
```

[← Previous: ArgumentsController](./04-ArgumentsController.md) • [[Source: PropertyBinding.cs]](../EasyArguments/Helper/PropertyBinding.cs) • [Next: Extensions →](./06-Extensions.md)