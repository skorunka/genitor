namespace Genitor.Library.Core.Localization
{
	using System;
	using System.Collections.Concurrent;
	using System.Globalization;
	using System.IO;

	using JetBrains.Annotations;

	using SecondLanguage;

	/// <summary>
	/// Lifetime should be Singleton(StructureMap's ".Singleton()"). 
	/// </summary>
	[UsedImplicitly]
	public class GetTextLocalizedStringManager : ILocalizedStringManager
	{
		private readonly string _localizationFilesRootFolderPattern;
		private readonly ConcurrentDictionary<CultureInfo, Translator> _translators;

		#region ctors

		public GetTextLocalizedStringManager(string localizationFilesRootFolderPattern)
		{
			this._localizationFilesRootFolderPattern = localizationFilesRootFolderPattern;
			this._translators = new ConcurrentDictionary<CultureInfo, Translator>();
		}

		#endregion

		public string GetLocalizedString(string scope, string text, CultureInfo cultureInfo)
		{
			var translator = this._translators.GetOrAdd(
					cultureInfo,
					x =>
					{
						var t = new Translator();
						t.RegisterTranslationsByCulture("{0}.po", cultureInfo, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this._localizationFilesRootFolderPattern.TrimStart('/').Replace("/", "\\")));
						return t;
					});

			var translatedText = translator.Translate(text);

			return !string.IsNullOrEmpty(translatedText) ? translatedText : text;
		}

		public void ReloadLocalizations()
		{
			this._translators.Clear();
		}
	}
}