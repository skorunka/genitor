namespace Genitor.Library.Core.Localization
{
	using System.Globalization;

	public interface ILocalizedStringManager
	{
		string GetLocalizedString(string scope, string text, CultureInfo cultureInfo);

		void ReloadLocalizations();
	}
}