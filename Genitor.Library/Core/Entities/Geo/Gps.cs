namespace Genitor.Library.Core.Entities.Geo
{
	using System.ComponentModel.DataAnnotations.Schema;

	[ComplexType]
	public class Gps
	{
		public float Longitude { get; set; }

		public float Latitude { get; set; }

		public bool LocationConfirmed { get; set; }
	}
}