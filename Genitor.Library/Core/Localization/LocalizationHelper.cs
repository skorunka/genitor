namespace Genitor.Library.Core.Localization
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.Diagnostics;
	using System.Globalization;
	using System.Linq;
	using System.Reflection;

	using Genitor.Library.Core.Entities;
	using Genitor.Library.Core.Entities.Localization;

	public static class LocalizationHelper
	{
		//// <Type.FullName, LocalizedEntityMetadata>
		private static readonly ConcurrentDictionary<string, LocalizedEntityMetadata> Cache;

		/// <summary>
		/// Critical: must be valid CultureInfo code.
		/// </summary>
		private const string DefaultLanguageCodeConst = "en";

		private const string ConfigEnabledLanguagesKeyConst = "Genitor.Library.Localization.EnabledLanguages";
		private const string ConfigDefaultLanguageCodeKeyConst = "Genitor.Library.Localization.DefaultLanguageCode";

		static LocalizationHelper()
		{
			DefaultLanguageCode = (System.Configuration.ConfigurationManager.AppSettings[ConfigDefaultLanguageCodeKeyConst] ?? DefaultLanguageCodeConst).ToLower();

			try
			{
				DefaultCultureInfo = CultureInfo.GetCultureInfo(DefaultLanguageCode);
			}
			catch (Exception e)
			{
				Trace.TraceError(e.ToString());

				DefaultCultureInfo = CultureInfo.DefaultThreadCurrentUICulture;
				DefaultLanguageCode = DefaultCultureInfo.Name.ToLower();
			}

			EnabledCultureInfos = new List<CultureInfo> { DefaultCultureInfo };

			var enabledLanguagesCsv = System.Configuration.ConfigurationManager.AppSettings[ConfigEnabledLanguagesKeyConst];
			if (!string.IsNullOrWhiteSpace(enabledLanguagesCsv))
			{
				foreach (var enabledLanguageCode in enabledLanguagesCsv.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim().ToLower()))
				{
					try
					{
						var cultureInfo = CultureInfo.GetCultureInfo(enabledLanguageCode);
						if (!EnabledCultureInfos.Contains(cultureInfo))
						{
							EnabledCultureInfos.Add(cultureInfo);
						}
					}
					catch (Exception e)
					{
						Trace.TraceError(e.ToString());
					}
				}
			}

			Cache = new ConcurrentDictionary<string, LocalizedEntityMetadata>();
			ScanAssembly(Assembly.GetExecutingAssembly());
		}

		public static string DefaultLanguageCode { get; }

		public static CultureInfo DefaultCultureInfo { get; }

		public static IList<CultureInfo> EnabledCultureInfos { get; }

		public static string GetCultureCountryCode(CultureInfo cultureInfo)
		{
			Guard.IsNotNull(cultureInfo, "cultureInfo");

			return cultureInfo.TextInfo.CultureName.Split('-').Last().ToLower();
		}

		public static void ScanAssembly(Assembly assembly)
		{
			//// pozor, tady nam to nenajde tridu, ktera ma base tridu ktera ma base tridu LocalizedEntityBase!
			var metadatas =
				assembly.GetTypes().Where(t => t.IsClass && t.IsVisible && !t.IsAbstract && t.BaseType == typeof(LocalizedEntityBase))
				.ToDictionary(GetEntityKey, t => new LocalizedEntityMetadata(t))
				.Where(x => x.Value.Properties.Count > 0);

			lock (Cache)
			{
				foreach (var metadata in metadatas)
				{
					Cache.TryAdd(metadata.Key, metadata.Value);
				}
			}
		}

		public static LocalizedEntityMetadata GetLocalizedEntityMetadata<TEntity>()
		{
			return GetLocalizedEntityMetadata(typeof(TEntity));
		}

		public static LocalizedEntityMetadata GetLocalizedEntityMetadata(Type entityType)
		{
			return GetLocalizedEntityMetadata(GetEntityKey(entityType));
		}

		public static LocalizedEntityMetadata GetLocalizedEntityMetadata(string entityName)
		{
			return string.IsNullOrEmpty(entityName) || !Cache.ContainsKey(entityName) ? null : Cache[entityName];
		}

		public static Type GetEntityType(object entity)
		{
			return entity.IsProxy() ? entity.GetType().BaseType : entity.GetType();
		}

		public static string GetEntityKey(Type entityType)
		{
			return entityType.FullName;
		}

		public sealed class LocalizedEntityMetadata
		{
			/// <summary>
			/// Najde lokalizovatelne properties na danem type. Lokalizovana property je bud property na objektu nebo v metadatech s atributem [Localizable].
			/// </summary>
			/// <param name="type">Type localizovatelneho objektu.</param>
			public LocalizedEntityMetadata(Type type)
			{
				this.EntityType = type;

				var fromType = GetLocalizableProperties(type) ?? new List<Pair<string, PropertyInfo>>(0);
				var fromMetadata = GetLocalizablePropertiesFromMetadata(type) ?? new List<Pair<string, PropertyInfo>>(0);
				this.Properties = fromType.Union(fromMetadata).Distinct().ToList();
			}

			public Type EntityType { get; private set; }

			public ICollection<Pair<string, PropertyInfo>> Properties { get; }

			private static ICollection<Pair<string, PropertyInfo>> GetLocalizableProperties(Type type)
			{
				return type.GetProperties()
					.Select(x => new { Attribute = x.GetCustomAttributes(typeof(LocalizableAttribute), true), PropertyInfo = x })
					.Where(x => x.Attribute.Length > 0)
					.Select(x => new Pair<string, PropertyInfo>(((LocalizableAttribute)x.Attribute.First()).StorePropertyName ?? x.PropertyInfo.Name, x.PropertyInfo))
					.ToList();
			}

			private static ICollection<Pair<string, PropertyInfo>> GetLocalizablePropertiesFromMetadata(Type type)
			{
				var metadata = type.GetCustomAttributes(typeof(MetadataTypeAttribute), true).OfType<MetadataTypeAttribute>().FirstOrDefault();
				if (metadata == null)
				{
					return null;
				}

				var prop =
					metadata.MetadataClassType.GetProperties()
							.Select(x => new { Attribute = x.GetCustomAttributes(typeof(LocalizableAttribute), true), PropertyInfo = x })
					.Where(x => x.Attribute.Length > 0)
					.Select(x => new Pair<string, PropertyInfo>(((LocalizableAttribute)x.Attribute.First()).StorePropertyName ?? x.PropertyInfo.Name, type.GetProperty(x.PropertyInfo.Name)))
					.Where(x => x.Second != null)
					.ToList();
				return prop;
			}
		}
	}
}