using System;
using System.Text.RegularExpressions;

namespace Genitor.Library.Core.Configuration
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class ConfigAutoFillAttribute : Attribute
	{
		private static Regex sHandlerRegex = new Regex(@"^\s*(\w+)(?:\;\s*(.+))?\s*$");

		private bool? mIsRequired;
		private string mDefaultValue;
		private string mHandler;

		public ConfigAutoFillAttribute()
		{
		}

		public ConfigAutoFillAttribute(string name)
		{
			Name = name;
		}

		public string Name { get; set; }

		public string Section { get; set; }

		public bool IsRequired
		{
			get 
			{ 
				if (mIsRequired.HasValue)
					return mIsRequired.Value;

				return mDefaultValue == null;
			}

			set { mIsRequired = value; }
		}

		public string DefaultValue
		{
			get { return mDefaultValue; }
			set { mDefaultValue = value; }
		}

		public string Handler
		{
			get { return mHandler; }
			set 
			{ 
				mHandler = value;
				string[] parts = mHandler.Split(new char[] { '@' }, 2);
				HandlerMethod = parts[0];
				if (parts.Length > 1 && parts[1].Length > 0)
					HandlerTypeName = parts[1];
			}
		}

		public string HandlerMethod { get; private set; }

		public string HandlerTypeName { get; private set; }
	}
}
