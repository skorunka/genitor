namespace Genitor.Library.Core.Configuration
{
	using System;
	using System.Text.RegularExpressions;

	[AttributeUsage(AttributeTargets.Field)]
	public class ConfigAutoFillAttribute : Attribute
	{
		private static Regex sHandlerRegex = new Regex(@"^\s*(\w+)(?:\;\s*(.+))?\s*$");

		private bool? _isRequired;
		private string _defaultValue;
		private string _handler;

		public ConfigAutoFillAttribute()
		{
		}

		public ConfigAutoFillAttribute(string name)
		{
			this.Name = name;
		}

		public string Name { get; set; }

		public string Section { get; set; }

		public bool IsRequired
		{
			get
			{
				if (this._isRequired.HasValue)
				{
					return this._isRequired.Value;
				}

				return this._defaultValue == null;
			}

			set
			{
				this._isRequired = value;
			}
		}

		public string DefaultValue
		{
			get
			{
				return this._defaultValue;
			}

			set
			{
				this._defaultValue = value;
			}
		}

		public string Handler
		{
			get
			{
				return this._handler;
			}

			set
			{
				this._handler = value;
				var parts = this._handler.Split(new[] { '@' }, 2);
				this.HandlerMethod = parts[0];
				if (parts.Length > 1 && parts[1].Length > 0)
				{
					this.HandlerTypeName = parts[1];
				}
			}
		}

		public string HandlerMethod { get; private set; }

		public string HandlerTypeName { get; private set; }
	}
}
