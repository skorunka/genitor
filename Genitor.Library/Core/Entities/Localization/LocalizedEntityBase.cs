namespace Genitor.Library.Core.Entities.Localization
{
	using System.ComponentModel.DataAnnotations.Schema;

	/// <summary>
	/// Base class for localizable entity.
	/// </summary>
	public abstract class LocalizedEntityBase : EntityBase
	{
		/// <summary>
		/// In which language is entity loaded.
		/// </summary>
		[NotMapped]
		public string LanguageCode { get; set; }
	}
}