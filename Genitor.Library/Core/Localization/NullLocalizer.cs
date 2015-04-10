namespace Genitor.Library.Core.Localization
{
	/// <summary>
	/// Does not localize anytning at all. It just returns original string.
	/// </summary>
	public static class NullLocalizer
	{
		private static readonly Localizer _instance;

		static NullLocalizer()
		{
			_instance = (format, args) => new LocalizedString((args == null || args.Length == 0) ? format : string.Format(format, args));
		}

		public static Localizer Instance
		{
			get
			{
				return _instance;
			}
		}
	}
}