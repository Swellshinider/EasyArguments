using EasyArguments.Attributes;
using System.Reflection;
using System.Text;

namespace EasyArguments;

internal class Argument
{
	internal Argument(PropertyInfo propertyInfo, ArgumentAttribute attribute)
		: this(null, propertyInfo, attribute) { }

	internal Argument(Argument? parent, PropertyInfo propertyInfo, ArgumentAttribute attribute)
	{
		Parent = parent;
		PropertyInfo = propertyInfo;
		Attribute = attribute;
		Children = [];

		if (Attribute.ShortName == null && Attribute.LongName == null)
			Attribute.LongName = $"--{PropertyInfo.Name.ToLowerInvariant()}";
	}

	internal Argument? Parent { get; }
	internal PropertyInfo PropertyInfo { get; }
	internal ArgumentAttribute Attribute { get; }
	internal List<Argument> Children { get; }

	private string ToString(string parentArgument, string indentation)
	{
		var builder = new StringBuilder();
		var names = string.IsNullOrEmpty(parentArgument) ? Attribute.LongName : $"{Attribute.ShortName}|{Attribute.LongName}";
		var prefix = $"{(string.IsNullOrEmpty(parentArgument) ? "\n" : "")}{indentation}{parentArgument}{names}";
		var suffix = $"{Attribute.HelpMessage} {(Attribute.Required ? "(Required)" : "")}";
		builder.AppendLine($"{prefix.PadRight(35)}{suffix}");

		indentation += "    ";
		foreach (var child in Children)
			builder.Append(child.ToString($"{Attribute.LongName!} ", indentation));

		return builder.ToString();
	}

	public string GetName()
	{
		var builder = new StringBuilder();
		if (Parent != null)
			builder.Append($"{Parent.GetName()} ");
		
		if (Attribute.ShortName != null)
			builder.Append($"{Attribute.ShortName}|");

		if (Attribute.LongName != null)
			builder.Append($"{Attribute.LongName}");

		return builder.ToString();
	}

	public override string ToString() => ToString("", "");
}