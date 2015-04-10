namespace Genitor.Library.MVC.Attributes
{
	using System.ComponentModel;
	using System.Web.Mvc;

	using Genitor.Library.Core.Localization;

	public class LocalizedDisplayNameAttribute : DisplayNameAttribute
	{
		private readonly string _text;

		#region ctors

		public LocalizedDisplayNameAttribute(string text)
		{
			this._text = text;
		}

		#endregion

		public override string DisplayName
		{
			get
			{
				var localizationService = DependencyResolver.Current.GetService<IText>();

				return localizationService == null ? this._text : localizationService.Get(this._text).Text;
			}
		}
	}
}