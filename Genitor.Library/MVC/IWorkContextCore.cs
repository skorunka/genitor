namespace Genitor.Library.MVC
{
	using System.Collections.Generic;
	using System.Globalization;

	using Genitor.Library.Core.Localization;

	public interface IWorkContextCore
	{
		IEnumerable<CultureInfo> EnabledCultureInfos { get; }

		/// <summary>
		/// CultureInfo used for localization and globalization of static texts.
		/// </summary>
		CultureInfo UiCultureInfo { get; }

		/// <summary>
		/// CultureInfo used for localization and globalization of entities.
		/// </summary>
		CultureInfo DataCultureInfo { get; }

		Localizer T { get; }

		void AddFlashMessage(MessageTypes messageType, LocalizedString localizedString);

		void AddFlashMessage(MessageTypes messageType, string messageText);

		void ClearFlashMessages();

		IReadOnlyDictionary<MessageTypes, IList<string>> GetFlashMessages(bool clearMessages = true);
	}
}