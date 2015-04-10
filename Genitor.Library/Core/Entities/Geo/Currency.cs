namespace Genitor.Library.Core.Entities.Geo
{
	using System.ComponentModel.DataAnnotations;

	using Genitor.Library.Core.Entities.Localization;

	[MetadataType(typeof(MetaData))]
	public class Currency : LocalizedEntityBase
	{
		/// <summary>
		/// ISO 4217 Code(http://en.wikipedia.org/wiki/ISO_4217)
		/// </summary>
		public string Code { get; set; }

		/// <summary>
		/// ISO 4217 Num(http://en.wikipedia.org/wiki/ISO_4217)
		/// </summary>
		public int Num { get; set; }

		public string NativeName { get; set; }

		public string Name { get; set; }

		/// <summary>
		/// Actual exchange rate against default Currency.
		/// One default currency is this value in this Currency.
		/// Denormalized colum of ExchangeRate.RateValue.
		/// </summary>
		public decimal ExchangeRate { get; set; }

		public bool IsUsedForDeals { get; set; }

		public class MetaData
		{
			[Required, StringLength(3)]
			public string Code { get; set; }

			[Required, StringLength(50)]
			public string NativeName { get; set; }

			[Localizable]
			[Required, StringLength(50)]
			public string Name { get; set; }
		}
	}
}