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

	private string ToString(string indentation)
	{
		var builder = new StringBuilder();
		var prefix = $"{indentation}{GetName()}";
		var suffix = $"{Attribute.HelpMessage} {(Attribute.Required ? "(Required)" : "")}";
		builder.Append($"{prefix.PadRight(35)}{suffix}\n");

		indentation += "    ";
		foreach (var child in Children)
			builder.Append(child.ToString(indentation));

		return builder.ToString();
	}

	public string GetName(bool ignoreReq = false)
	{
		var builder = new StringBuilder();
		var required = Attribute.Required;
		
		if (Parent != null)
			builder.Append($"{Parent.GetName(true)} ");
		
		if (!required && !ignoreReq)
			builder.Append('[');
		
		if (Attribute.ShortName != null)
			builder.Append($"{Attribute.ShortName}|");

		if (Attribute.LongName != null)
			builder.Append($"{Attribute.LongName}");

		if (!required && !ignoreReq)
			builder.Append(']');
			
		return builder.ToString();
	}

	public override string ToString() => ToString("");
}