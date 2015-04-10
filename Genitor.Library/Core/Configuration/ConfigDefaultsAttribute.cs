using System;

namespace Genitor.Library.Core.Configuration
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class ConfigDefaultsAttribute : Attribute
	{
		public ConfigDefaultsAttribute(string name)
		{
			Section = name;
		}

		public string Section { get; set; }
	}
}
