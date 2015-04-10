namespace Genitor.Library.Core.Localization
{
	using System.Globalization;

	public interface IText
	{
		LocalizedString Get(string text, params object[] args);

		LocalizedString Get(string text, string scope, params object[] args);

		LocalizedString Get(string text, CultureInfo cultureInfo, params object[] args);

		LocalizedString Get(string text, string scope, CultureInfo cultureInfo, params object[] args);
	}
}