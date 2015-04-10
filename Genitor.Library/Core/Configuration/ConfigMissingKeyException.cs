using System;
using System.Configuration;
using System.Runtime.Serialization;


namespace Genitor.Library.Core.Configuration
{
	[Serializable]
	public class ConfigMissingKeyException : ConfigurationErrorsException
	{
		public ConfigMissingKeyException(string section, string key)
			: base(String.Format("Configuration section '<{0}>' is missing required key '{1}'", section, key))
		{
		}

		protected ConfigMissingKeyException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
