namespace Genitor.Library.Core.Entities.Localization
{
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	/// <summary>
	/// Konkretni lokalizovana hodnota property dane entity.
	/// </summary>
	[MetadataType(typeof(MetaData))]
	public class Localization
	{
		public string LanguageCode { get; set; }

		public string EntityName { get; set; }

		public int EntityPkValue { get; set; }

		public string EntityPropertyName { get; set; }

		public string Text { get; set; }

		public class MetaData
		{
			[Key, Column(Order = 1), Required, StringLength(10)]
			public string LanguageCode { get; set; }

			[Key, Column(Order = 2), Required, StringLength(100)]
			public string EntityName { get; set; }

			[Key, Column(Order = 3)]
			public int EntityPkValue { get; set; }

			[Key, Column(Order = 4), Required, StringLength(100)]
			public string EntityPropertyName { get; set; }

			[Required]
			public string Text { get; set; }
		}
	}
}