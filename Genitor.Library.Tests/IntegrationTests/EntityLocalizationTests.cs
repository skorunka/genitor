﻿// ReSharper disable InconsistentNaming
namespace Genitor.Library.Tests.IntegrationTests
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.Linq;
	using System.Reflection;

	using Genitor.Library.Core.Entities.Localization;
	using Genitor.Library.Core.Localization;

	using Microsoft.VisualStudio.TestTools.UnitTesting;

	#region Localizable test classes

	public class LocalizableClass_ByProperties : LocalizedEntityBase
	{
		[Localizable]
		public string Name { get; set; }

		public string NotLocalizable { get; set; }
	}

	[MetadataType(typeof(MetaData))]
	public class LocalizableClass_ByMetaData : LocalizedEntityBase
	{
		public class MetaData
		{
			[Localizable]
			public string Name { get; set; }
		}

		public string Name { get; set; }
	}

	[MetadataType(typeof(MetaData))]
	public class LocalizableClass_Complex : LocalizedEntityBase
	{
		public class MetaData
		{
			[Localizable]
			public string Name { get; set; }

			public string NotLocalizable1 { get; set; }
		}

		public string Name { get; set; }

		[Localizable]
		public string NoMetaData { get; set; }

		public string NotLocalizable1 { get; set; }

		public string NotLocalizable2 { get; set; }
	}

	#endregion

	[TestClass]
	public class EntityLocalizationIntegrationTests
	{
		[TestMethod]
		public void Scan_Assembly_And_Get_Localizable_Entities()
		{
			LocalizationHelper.ScanAssembly(Assembly.GetExecutingAssembly());

			this.CheckCommonLocalizationRulesOnType(typeof(LocalizableClass_ByProperties));
			this.CheckCommonLocalizationRulesOnType(typeof(LocalizableClass_ByMetaData));

			var localizableClass_Complex = LocalizationHelper.GetLocalizedEntityMetadata(typeof(LocalizableClass_Complex).FullName);

			Assert.AreEqual(2, localizableClass_Complex.Properties.Count);
			Assert.AreEqual(2, localizableClass_Complex.Properties.Count(x => x.Second.Name == "Name" || x.Second.Name == "NoMetaData")); // misto contains
		}

		private void CheckCommonLocalizationRulesOnType(Type type)
		{
			var test = LocalizationHelper.GetLocalizedEntityMetadata(type.FullName);

			Assert.IsNotNull(test);
			Assert.IsNotNull(test.Properties);
			Assert.AreEqual(1, test.Properties.Count);
			Assert.IsNotNull(test.Properties.SingleOrDefault(x => x.Second.Name.Equals("Name")));
		}
	}
}
