namespace Genitor.Library.Core.Localization
{
	using System.Globalization;

	using JetBrains.Annotations;

	[UsedImplicitly]
	public class Text : IText
	{
		private readonly ILocalizedStringManager _localizedStringManager;

		#region ctors

		public Text(ILocalizedStringManager localizedStringManager)
		{
			Guard.IsNotNull(localizedStringManager, "localizedStringManager");

			this._localizedStringManager = localizedStringManager;
		}

		#endregion

		public LocalizedString Get(string text, params object[] args)
		{
			return this.Get(text, (string)null, args);
		}

		public LocalizedString Get(string text, string scope, params object[] args)
		{
			return this.Get(text, scope, null, args);
		}

		public LocalizedString Get(string text, CultureInfo cultureInfo, params object[] args)
		{
			return this.Get(text, null, cultureInfo, args);
		}

		public LocalizedString Get(string text, string scope, CultureInfo cultureInfo, params object[] args)
		{
			var localizedFormat = this._localizedStringManager.GetLocalizedString(scope, text, cultureInfo ?? CultureInfo.CurrentUICulture);

			return new LocalizedString(args.Length == 0 ? localizedFormat : string.Format(cultureInfo ?? CultureInfo.CurrentUICulture, localizedFormat, args), scope, text, args);
		}
	}
}