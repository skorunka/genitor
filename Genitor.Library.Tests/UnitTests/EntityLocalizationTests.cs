// ReSharper disable InconsistentNaming
using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using Genitor.Library.Core.Entities;
using Genitor.Library.Core.Entities.Geo;
using Genitor.Library.Core.Entities.Localization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Genitor.Library.Tests.UnitTests
{
	using Genitor.Library.Core.Localization;

	#region Test classes

	public class NotLocalizedEntity : EntityBase
	{
		public string Text { get; set; }
	}

	[MetadataType(typeof(MetaData))]
	public class LocalizedEntity : LocalizedEntityBase
	{
		public class MetaData
		{
			[Localizable]
			public string LocalizedText { get; set; }

			public string NotLocalizedText { get; set; }
		}

		public string LocalizedText { get; set; }

		public string NotLocalizedText { get; set; }
	}

	#endregion

	[TestClass]
	public class EntityLocalizationTests
	{
		public class TestDataContext : LocalizationDataContextBase
		{
			#region DbSets

			public DbSet<Currency> Currencies { get; set; }

			public DbSet<NotLocalizedEntity> NotLocalizedEntities { get; set; }

			public DbSet<LocalizedEntity> LocalizedEntities { get; set; }

			#endregion

			public T SaveAndLoadEntity<T>(T entity) where T : EntityBase
			{
				Set<T>().Add(entity);

				SaveChanges();

				return Set<T>().Find(entity.Id);
			}

			public T SaveAndLoadLocalizedEntity<T>(T entity) where T : LocalizedEntityBase
			{
				Set<T>().Add(entity);

				SaveChanges();

				return Set<T>().Find(entity.Id);
			}

			public void ReCreate()
			{
				this.Database.Delete();
				this.Database.CreateIfNotExists();

				Languages.Add(new Language { Code = "en", NativeName = "English", Name = "English" });
				Languages.Add(new Language { Code = "cs", NativeName = "Čeština", Name = "Czech" });

				Currencies.Add(new Currency { Code = "CZK", Name = "Koruna", ExchangeRate = 1, Num = 1, NativeName = "Koruna" });

				SaveChanges();
			}
		}

		[ClassInitialize]
		public static void ClassInit(TestContext context)
		{
			LocalizationHelper.ScanAssembly(Assembly.GetExecutingAssembly());
		}

		[TestMethod]
		public void Add_NonLocalizable_Entity_Does_Not_Add_Localizations()
		{
			using (var db = new TestDataContext())
			{
				db.ReCreate();
				var entity = new NotLocalizedEntity { Text = "Text" };

				db.NotLocalizedEntities.Add(entity);
				var entityLoaded = db.SaveAndLoadEntity(entity);

				Assert.AreSame(entity, entityLoaded);
				Assert.IsTrue(!db.Localizations.Any());
			}
		}

		[TestMethod]
		public void Add_Localizable_Entity_In_Default_Language_Does_Not_Add_Localizations()
		{
			using (var db = new TestDataContext())
			{
				db.ReCreate();
				var entity = new LocalizedEntity { NotLocalizedText = "NotLocalizedText", LocalizedText = "LocalizedText" };

				db.LocalizedEntities.Add(entity);
				var entityLoaded = db.SaveAndLoadLocalizedEntity(entity);

				Assert.AreSame(entity, entityLoaded);
				Assert.IsTrue(!db.Localizations.Any());
			}
		}

		[TestMethod]
		[ExpectedException(typeof(NotSupportedException))]
		public void Add_Localizable_Entity_In_NonDefault_Language_Throws_NotSupportedException()
		{
			using (var db = new TestDataContext())
			{
				db.ReCreate();
				var entity = new LocalizedEntity { NotLocalizedText = "NotLocalizedText", LocalizedText = "LocalizedText", LanguageCode = "cs" };

				db.LocalizedEntities.Add(entity);
				db.SaveChanges(); // -> shoud throw exception...
			}
		}

		[TestMethod]
		[ExpectedException(typeof(NotSupportedException))]
		public void Add_Localizable_Entities_In_Default_And_NonDefault_Language_In_One_UnitOfWork_Throws_NotSupportedException()
		{
			using (var db = new TestDataContext())
			{
				db.ReCreate();
				var entity1 = new LocalizedEntity { NotLocalizedText = "NotLocalizedText", LocalizedText = "LocalizedText" };
				var entity2 = new LocalizedEntity { NotLocalizedText = "NotLocalizedText", LocalizedText = "LocalizedText", LanguageCode = "cs" };

				db.LocalizedEntities.Add(entity1);
				db.LocalizedEntities.Add(entity2);
				db.SaveChanges(); // -> shoud throw exception...
			}
		}

		[TestMethod]
		public void Add_Localizable_Entity_Does_Add_Localizations()
		{
			using (var db = new TestDataContext())
			{
				db.ReCreate();
				var entity = new LocalizedEntity { NotLocalizedText = "NotLocalizedText", LocalizedText = "LocalizedText" };

				db.LocalizedEntities.Add(entity);
				db.SaveChanges();
			}

			using (var db = new TestDataContext())
			{
				var entity = db.LocalizedEntities.First();
				entity.LanguageCode = "cs";
				entity.LocalizedText = "new localized text";
				db.SaveChanges();
			}

			using (var db = new TestDataContext())
			{
				var entity = db.LocalizedEntities.First();

				Assert.IsTrue(db.LocalizedEntities.Count() == 1);
				Assert.IsTrue(db.Localizations.Count() == 1);
				Assert.IsTrue(db.Localizations.Count(x => x.EntityName == typeof(LocalizedEntity).FullName && x.EntityPkValue == entity.Id) == 1);
				Assert.IsTrue(db.Localizations.Single(x => x.EntityName == typeof(LocalizedEntity).FullName && x.EntityPkValue == entity.Id && x.EntityPropertyName == "LocalizedText").Text == "new localized text");
				Assert.AreEqual("NotLocalizedText", entity.NotLocalizedText);
				Assert.AreEqual("LocalizedText", entity.LocalizedText);
				Assert.AreEqual(1, entity.Id);
			}
		}

		[TestMethod]
		public void Add_Localizable_Entity_Preserve_Original_And_Loads_Localized()
		{
			using (var db = new TestDataContext())
			{
				db.ReCreate();
				var entity = new LocalizedEntity { NotLocalizedText = "NotLocalizedText", LocalizedText = "LocalizedText" };

				db.LocalizedEntities.Add(entity);
				db.SaveChanges();
			}

			using (var db = new TestDataContext())
			{
				var entity = db.LocalizedEntities.First();
				entity.LanguageCode = "cs";
				entity.LocalizedText = "new localized text";
				db.SaveChanges();
			}

			using (var db = new TestDataContext())
			{
				var entity = db.LocalizeEntity(db.LocalizedEntities.First(), "cs");

				Assert.AreEqual(1, entity.Id);
				Assert.AreEqual("NotLocalizedText", entity.NotLocalizedText);
				Assert.AreEqual("new localized text", entity.LocalizedText);
			}
		}

		[TestMethod]
		public void Delete_Localizable_Entity_In_NonDefault_Language_Delete_Only_Specific_Localizations()
		{
			using (var db = new TestDataContext())
			{
				db.ReCreate();
				var entity = new LocalizedEntity { NotLocalizedText = "NotLocalizedText", LocalizedText = "LocalizedText" };
				db.LocalizedEntities.Add(entity);
				db.SaveChanges();
				entity = db.LocalizedEntities.First();
				entity.LanguageCode = "cs";
				entity.LocalizedText = "new localized text";
				db.SaveChanges();
			}
			using (var db = new TestDataContext())
			{
				var entity = db.LocalizedEntities.First();
				entity.LanguageCode = "cs";
				entity.ArchivedOnUtc = DateTime.Now;
				db.LocalizedEntities.Remove(entity);
				db.SaveChanges();
			}

			using (var db = new TestDataContext())
			{
				var entity = db.LocalizedEntities.First();

				Assert.AreEqual(1, entity.Id);
				Assert.AreEqual("NotLocalizedText", entity.NotLocalizedText);
				Assert.AreEqual("LocalizedText", entity.LocalizedText);
				Assert.IsTrue(!db.Localizations.Any());
			}
		}

		[TestMethod]
		public void Delete_Localizable_Entity_In_Default_Language_Delete_AllLocalizations_And_Entity()
		{
			using (var db = new TestDataContext())
			{
				db.ReCreate();
				var entity = new LocalizedEntity { NotLocalizedText = "NotLocalizedText", LocalizedText = "LocalizedText" };
				db.LocalizedEntities.Add(entity);
				db.SaveChanges();
				entity = db.LocalizedEntities.First();
				entity.LanguageCode = "cs";
				entity.LocalizedText = "new localized text";
				db.SaveChanges();
			}
			using (var db = new TestDataContext())
			{
				var entity = db.LocalizedEntities.First();
				entity.ArchivedOnUtc = DateTime.Now; // force delete
				db.LocalizedEntities.Remove(entity);
				db.SaveChanges();
			}

			using (var db = new TestDataContext())
			{
				var entity = db.LocalizedEntities.FirstOrDefault();

				Assert.IsNull(entity);
				Assert.IsTrue(!db.Localizations.Any());
			}
		}
	}
}