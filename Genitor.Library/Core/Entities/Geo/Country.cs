namespace Genitor.Library.Core.Entities.Geo
{
	using System.ComponentModel.DataAnnotations;
	using Genitor.Library.Core.Entities.Localization;

	[MetadataType(typeof(MetaData))]
	public class Country : LocalizedEntityBase
	{
		public string Code { get; set; }

		public string Name { get; set; }

		public bool AllowAddCountry { get; set; }

		public int CurrencyId { get; set; }

		public virtual Currency Currency { get; set; }

		public class MetaData
		{
			[Required, StringLength(10)]
			public string Code { get; set; }

			[Localizable]
			[Required, StringLength(50)]
			public string Name { get; set; }

			public bool AllowAddCountry { get; set; }
		}
	}
}