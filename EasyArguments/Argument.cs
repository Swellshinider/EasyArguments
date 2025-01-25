using EasyArguments.Attributes;
using System.Reflection;
using System.Text;

namespace EasyArguments;

internal class Argument
{
	internal Argument(PropertyInfo propertyInfo, ArgumentAttribute attribute)
	{
		PropertyInfo = propertyInfo;
		Attribute = attribute;
		Children = [];
		
		if (Attribute.ShortName == null && Attribute.LongName == null)
			Attribute.LongName = $"--{PropertyInfo.Name.ToLowerInvariant()}";
	}

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
			builder.Append(child.ToString($"{Attribute.LongName!} " ,indentation));
		
		return builder.ToString();
	}

	public override string ToString() => ToString("", "");
}