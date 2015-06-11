namespace Genitor.Library.MVC
{
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using System.Web.SessionState;

	using Genitor.Library.Core;
	using Genitor.Library.Core.Localization;

	public abstract class WorkContextBase : IWorkContextCore
	{
		private const string FlashMessagesContainerKey = "__WorkContextBase.FlashMessages";

		private readonly HttpSessionState _session;

		#region ctors

		protected WorkContextBase(HttpSessionState session, IText text)
		{
			this._session = session;
			this.T = text.Get;
		}

		#endregion

		public IEnumerable<CultureInfo> EnabledCultureInfos
		{
			get
			{
				return LocalizationHelper.EnabledCultureInfos;
			}
		}

		public CultureInfo UiCultureInfo
		{
			get
			{
				return CultureInfo.CurrentUICulture;
			}
		}

		public CultureInfo DataCultureInfo
		{
			get
			{
				return CultureInfo.CurrentCulture;
			}
		}

		public Localizer T { get; }

		public void AddFlashMessage(MessageTypes messageType, LocalizedString localizedString)
		{
			this.AddFlashMessage(messageType, localizedString.Text);
		}

		public void AddFlashMessage(MessageTypes messageType, string messageText)
		{
			if (string.IsNullOrWhiteSpace(messageText))
			{
				return;
			}

			var flashMessages = this._session[FlashMessagesContainerKey] as IDictionary<MessageTypes, IList<string>>;

			if (flashMessages == null)
			{
				flashMessages = new Dictionary<MessageTypes, IList<string>>();

				this._session.Add(FlashMessagesContainerKey, flashMessages);
			}

			if (!flashMessages.ContainsKey(messageType))
			{
				flashMessages.Add(messageType, new List<string>());
			}

			flashMessages[messageType].Add(messageText);
		}

		public IReadOnlyDictionary<MessageTypes, IList<string>> GetFlashMessages(bool clearMessages = true)
		{
			var flashMessages = this._session[FlashMessagesContainerKey] as IDictionary<MessageTypes, IList<string>>;

			if (flashMessages == null)
			{
				return null;
			}

			if (clearMessages)
			{
				this.ClearFlashMessages();
			}

			return new ReadOnlyDictionary<MessageTypes, IList<string>>(flashMessages);
		}

		public void ClearFlashMessages()
		{
			this._session.Remove(FlashMessagesContainerKey);
		}
	}
}