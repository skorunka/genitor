namespace Genitor.Library.Core.Configuration
{
	using System;
	using System.Configuration;
	using System.Runtime.Serialization;
	using System.Security.Permissions;

	[Serializable]
	public class ConfigInvalidValueException : ConfigurationErrorsException
	{
		#region Constructors and Destructors

		public ConfigInvalidValueException(string type)
		{
			this.Type = type;
		}

		public ConfigInvalidValueException(string section, string key, string type)
		{
			this.Section = section;
			this.Key = key;
			this.Type = type;
		}

		protected ConfigInvalidValueException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		#endregion

		#region Public Properties

		public string Key { get; }

		public override string Message
		{
			get
			{
				return string.Format(
					"Key '{0}' in configuration section '<{1}>' is not of type '{2}'", this.Key, this.Section, this.Type);
			}
		}

		public string Section { get; }

		public string Type { get; }

		#endregion

		#region Public Methods and Operators

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException(nameof(info));
			}

			info.AddValue("Section", this.Section);
			info.AddValue("Key", this.Key);
			info.AddValue("Type", this.Type);

			base.GetObjectData(info, context);
		}

		#endregion
	}
}