namespace Genitor.Library.Core.Configuration
{
	using System;

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class ConfigDefaultsAttribute : Attribute
	{
		public ConfigDefaultsAttribute(string name)
		{
			this.Section = name;
		}

		public string Section { get; set; }
	}
}
