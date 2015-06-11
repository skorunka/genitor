namespace Genitor.Library.Tests.Db
{
	using System.Data.Entity;

	using Genitor.Library.Core.Entities.Geo;
	using Genitor.Library.Core.Entities.Localization;

	public class TestDataContext : LocalizationDataContextBase
	{
		#region DbSets

		public DbSet<Currency> Currencies { get; set; }
		public DbSet<Country> Countries { get; set; }
		public DbSet<State> States { get; set; }
		public DbSet<City> Cities { get; set; }

		#endregion

		public TestDataContext(string v) : base(v)
		{
		}
	}
}