using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Genitor.Library.Core.Entities.Geo;
using Genitor.Library.Core.Entities.Localization;

namespace Genitor.Library.Tests.Db
{
	public class TestDataContextInitializer : DropCreateDatabaseAlways<TestDataContext>
	{
		protected override void Seed(TestDataContext context)
		{
			#region Languages

			var defaultLanguage = new Language { Code = "en", NativeName = "English", Name = "English" };
			var languages = new List<Language>
			                	{
			                		defaultLanguage,
			                		new Language {Code = "cs", NativeName = "Czech", Name = "Česky"},
			                		new Language {Code = "de", NativeName = "German", Name = "Deutsch"},
			                		new Language {Code = "ru", NativeName = "Russia", Name = "По Русски"},
			                	};
			languages.ForEach(x => context.Languages.Add(x));

			#endregion

			#region Currencies - en

			var currencies = new List<Currency>
			                 	{
			                 		new Currency {Code = "CZK", Num = 203, NativeName = "Czech Koruna", Name = "Czech Koruna"},
			                 		new Currency {Code = "EUR", Num = 978, NativeName = "Euro", Name = "Euro"},
			                 		new Currency {Code = "USD", Num = 203, NativeName = "United States Dollar", Name = "Czech United States Dollar"},
			                 	};
			currencies.ForEach(x => context.Currencies.Add(x));

			#endregion

			#region Countries - en

			var cr = new Country { Code = "cz", Name = "Czech Republic", Currency = currencies.First(x => x.Code == "CZK") };
			var usa = new Country { Code = "us", Name = "United States of America", Currency = currencies.First(x => x.Code == "USD") };
			var countries = new List<Country>
			                	{
			                		cr,
			                		usa,
			                	};
			countries.ForEach(x => context.Countries.Add(x));

			#endregion

			#region States - en

			var states = new List<State>
			             	{
			             		new State {Code = "CA", Name = "California", Country = usa},
			             		new State {Code = "TX", Name = "Texas", Country = usa},
			             	};
			states.ForEach(x => context.States.Add(x));

			#endregion

			#region Cities - en

			var cities = new List<City>
			             	{
			             		new City { Name = "Praha", Country = cr},
			             		new City { Name = "Brno", Country = cr},
			                		
			             		new City { Name = "Sacramento", Country = usa, State = states.First(x=>x.Code == "CA")},
			             		new City { Name = "Austin", Country = usa, State = states.First(x=>x.Code == "TX")},
			             	};
			cities.ForEach(x => context.Cities.Add(x));

			#endregion

			context.SaveChanges();
		}
	}
}