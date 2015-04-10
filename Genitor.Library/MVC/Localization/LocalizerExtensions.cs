namespace Genitor.Library.MVC.Localization
{
	using System.Linq;

	using Genitor.Library.Core.Localization;

	public static class LocalizerExtensions
	{
		public static LocalizedString Plural(this Localizer localizer, string textSingular, string textPlural, int count, params object[] args)
		{
			return localizer(count == 1 ? textSingular : textPlural, new object[] { count }.Concat(args).ToArray());
		}
	}
}