namespace Genitor.Library.Core.Localization
{
	using System;
	using System.Web;

	public class LocalizedString : MarshalByRefObject, IHtmlString
	{
		private readonly string _localized;
		private readonly string _scope;
		private readonly string _textHint;
		private readonly object[] _args;

		#region ctors
		
		public LocalizedString(string languageNeutral)
		{
			this._localized = languageNeutral;
			this._textHint = languageNeutral;
		}

		public LocalizedString(string localized, string scope, string textHint, object[] args)
		{
			this._localized = localized;
			this._scope = scope;
			this._textHint = textHint;
			this._args = args;
		}

		#endregion

		public string Scope
		{
			get { return this._scope; }
		}

		public string TextHint
		{
			get { return this._textHint; }
		}

		public object[] Args
		{
			get { return this._args; }
		}

		public string Text
		{
			get { return this._localized; }
		}

		public static LocalizedString TextOrDefault(string text, LocalizedString defaultValue)
		{
			return string.IsNullOrEmpty(text) ? defaultValue : new LocalizedString(text);
		}

		public override string ToString()
		{
			return this._localized;
		}

		string IHtmlString.ToHtmlString()
		{
			return this._localized;
		}

		public override int GetHashCode()
		{
			var hashCode = 0;
			if (this._localized != null)
			{
				hashCode ^= this._localized.GetHashCode();
			}

			return hashCode;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != this.GetType())
			{
				return false;
			}

			var that = (LocalizedString)obj;
			return string.Equals(this._localized, that._localized);
		}
	}
}