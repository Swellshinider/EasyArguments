# EasyArguments Documentation

- [← Back to README](../README.md)

### Getting started?

- [Basic Usage Example](../README.md#basic-usage-example)
- [Installation Guide](../README.md#installation)

### Advanced examples

- **[Nested Arguments](./Examples/NestedArguments.md)** Nested arguments allow you to structure your command-line options into hierarchical groups.
- **[Executor](./Examples/ArgumentsExecution.md)** The Arguments Execution process in **EasyArguments** refers to the mechanism that automatically invokes executor methods after a property’s value has been assigned. 

### Core Components

1. **[ArgumentsControllerAttribute](01-ArgumentsControllerAttribute.md)** Attribute responsible to configure your class arguments.

2. **[ArgumentAttribute](02-ArgumentAttribute.md)** Attribute for configuring individual command-line parameters

3. **[ExecutorAttribute](03-ExecutorAttribute.md)** Attribute that define command handlers, useful to execute functions automatically

4. **[ArgumentsController](04-ArgumentsController.md)** The main parser class that orchestrates argument processing

5. **[PropertyBinding](05-PropertyBinding.md)** Behind-the-scenes value conversion mechanics

6. **[Extensions](06-Extensions.md)** Additional utilities for advanced scenarios