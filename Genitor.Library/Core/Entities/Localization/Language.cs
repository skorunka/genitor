namespace Genitor.Library.Core.Entities.Localization
{
	using System.ComponentModel.DataAnnotations;

	[MetadataType(typeof(MetaData))]
	public class Language
	{
		public string Code { get; set; }

		public string Name { get; set; }

		public string NativeName { get; set; }

		public class MetaData
		{
			[Required, StringLength(10), Key]
			public string Code { get; set; }

			[Required, StringLength(50)]
			public string Name { get; set; }

			[Required, StringLength(50)]
			public string NativeName { get; set; }
		}
	}
}