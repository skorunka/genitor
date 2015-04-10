namespace Genitor.Library.Core.Entities.Geo
{
	using System.ComponentModel.DataAnnotations;
	using Genitor.Library.Core.Entities.Localization;

	[MetadataType(typeof(MetaData))]
	public class City : LocalizedEntityBase
	{
		#region ctor

		public City()
		{
			this.Gps = new Gps();
		}

		#endregion

		public string Name { get; set; }

		public int? StateId { get; set; }

		public virtual State State { get; set; }

		public int CountryId { get; set; }

		public virtual Country Country { get; set; }

		public Gps Gps { get; set; }

		public bool InactiveCity { get; set; }

		public class MetaData
		{
			[Localizable]
			[Required, StringLength(50)]
			public string Name { get; set; }
		}
	}
}