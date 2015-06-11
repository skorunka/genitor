namespace Genitor.Library.Core.Configuration
{
	using System;
	using System.Configuration;
	using System.Runtime.Serialization;

	[Serializable]
	public class ConfigMissingKeyException : ConfigurationErrorsException
	{
		public ConfigMissingKeyException(string section, string key)
			: base($"Configuration section '<{section}>' is missing required key '{key}'")
		{
		}

		protected ConfigMissingKeyException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
