// ReSharper disable InconsistentNaming
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Genitor.Library.Tests.UnitTests
{
	[TestClass]
	public class EntityTests
	{
		[TestMethod]
		public void Add_Entity_Set_CreatedOn()
		{
			using (var db = new EntityLocalizationTests.TestDataContext())
			{
				db.ReCreate();
				var entity = new NotLocalizedEntity { Text = "Text" };

				db.NotLocalizedEntities.Add(entity);
				var entityLoaded = db.SaveAndLoadEntity(entity);

				Assert.IsTrue(entityLoaded.CreatedOnUtc <= DateTime.UtcNow);
			}
		}

		[TestMethod]
		public void FirstLevel_Entity_Deletion_Marks_It_As_Deleted_But_Preserve_It()
		{
			using (var db = new EntityLocalizationTests.TestDataContext())
			{
				db.ReCreate();
				var entity = db.Currencies.First();
				db.Currencies.Remove(entity);
				db.SaveChanges();
			}

			using (var db = new EntityLocalizationTests.TestDataContext())
			{
				var entity = db.Currencies.First();

				Assert.AreEqual(1, entity.Id);
				Assert.IsNotNull(entity.ArchivedOnUtc);
			}
		}

		[TestMethod]
		public void SecondLevel_Entity_Deletion_Deleted_It()
		{
			using (var db = new EntityLocalizationTests.TestDataContext())
			{
				db.ReCreate();
				var entity = db.Currencies.Find(1);
				db.Currencies.Remove(entity);
				db.SaveChanges();
			}

			using (var db = new EntityLocalizationTests.TestDataContext())
			{
				var entity = db.Currencies.Find(1);
				db.Currencies.Remove(entity);
				db.SaveChanges();
			}

			using (var db = new EntityLocalizationTests.TestDataContext())
			{
				var entity = db.Currencies.Find(1);

				Assert.IsNull(entity);
			}
		}

		[TestMethod]
		public void Direct_Entity_Deletion_By_Manually_Setting_DeletedOn_Deleted_It()
		{
			using (var db = new EntityLocalizationTests.TestDataContext())
			{
				db.ReCreate();
				var entity = db.Currencies.Find(1);
				entity.ArchivedOnUtc = DateTime.UtcNow;
				db.Currencies.Remove(entity);
				db.SaveChanges();
			}

			using (var db = new EntityLocalizationTests.TestDataContext())
			{
				var entity = db.Currencies.Find(1);

				Assert.IsNull(entity);
			}
		}
	}
}
