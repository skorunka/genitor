namespace Genitor.Library.Core.Entities.Localization
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.Entity;
	using System.Data.Entity.Infrastructure;
	using System.Linq;

	using Fasterflect;

	using Genitor.Library.Core.Localization;

	public interface ILocalizationDataContext : IDisposable
	{
		#region Public Properties

		string DefaultLanguageCode { get; }

		IDbSet<Localization> Localizations { get; set; }

		IDbSet<Language> Languages { get; set; }

		#endregion

		#region Public Methods and Operators

		bool IsDefaultLanguage(string languageCode);

		TEntity LocalizeEntity<TEntity>(TEntity entity, string languageCode = null, bool deep = false)
			where TEntity : LocalizedEntityBase, new();

		/// <summary>
		/// Lokalizuje seznam Entity. Kazda lokalizovana entita se vraci jako clone puvodni. Entity v DefaultLanguage ignoruje.
		/// </summary>
		/// <param name="entities"> Seznam entit, ktere maji byt lokalizovane. </param>
		/// <param name="languageCode"> Jazyk do ktereho se Entity lokalizuji. Pokud neni nsatven pouzije se LanguiageCode property u Entity. </param>
		/// <param name="deep"> Pokud je true, lokalizuje i navigacni property. </param>
		/// <returns> Naklonovany seznam lokalizovanych entit. </returns>
		IList<TEntity> Localize<TEntity>(IList<TEntity> entities, string languageCode = null, bool deep = false)
			where TEntity : LocalizedEntityBase, new();

		int SaveChanges();

		#endregion
	}

	public abstract class LocalizationDataContextBase : EntityDataContextBase, ILocalizationDataContext
	{
		protected LocalizationDataContextBase(string v = null) : base(v)
		{
		}

		#region Public Properties

		public string DefaultLanguageCode
		{
			get
			{
				return LocalizationHelper.sDefaultLanguageCode;
			}
		}

		public IDbSet<Localization> Localizations { get; set; }

		public IDbSet<Language> Languages { get; set; }

		#endregion

		#region Public Methods and Operators

		public bool IsDefaultLanguage(string languageCode)
		{
			return this.DefaultLanguageCode.EqualsInsensitive(languageCode);
		}

		public IEnumerable<Localization> GetLocalizations(string entityName, int entityId, string languageCode)
		{
			var localizations =
				this.Localizations.Where(
					l =>
					l.EntityName.Equals(entityName, StringComparison.OrdinalIgnoreCase) && l.EntityPkValue.Equals(entityId)
					&& l.LanguageCode.Equals(languageCode, StringComparison.OrdinalIgnoreCase));
			return localizations;
		}

		public TEntity LocalizeEntity<TEntity>(TEntity entity, string languageCode = null, bool deep = false)
			where TEntity : LocalizedEntityBase, new()
		{
			IList<TEntity> entities = new List<TEntity> { entity };
			return Localize(entities, languageCode ?? entity.LanguageCode, deep).First();
		}

		public IList<TEntity> Localize<TEntity>(IList<TEntity> entities, string languageCode = null, bool deep = false)
			where TEntity : LocalizedEntityBase, new()
		{
			if (this.IsDefaultLanguage(languageCode))
			{
				return entities;
			}

			if (deep)
			{
				throw new NotImplementedException("Deep lokalizace neni zatim implementovana.");
			}

			var clones = new List<TEntity>(entities.Count);
			foreach (var entity in entities)
			{
				if (this.IsDefaultLanguage(languageCode))
				{
					clones.Add(entity);
					continue;
				}

				var entityType = LocalizationHelper.GetEntityType(entity);
				var entityKey = LocalizationHelper.GetEntityKey(entityType);
				var metadata = LocalizationHelper.GetLocalizedEntityMetadata(entityKey);
				if (metadata == null)
				{
					break;
				}

				//// vytvorime novou instanci a nakopirujeme do ni hodnoty z puvodni
				var clone = (TEntity)entityType.CreateInstance();
				entity.MapProperties(clone);
				clone.LanguageCode = languageCode;

				var localizations = this.GetLocalizations(entityKey, clone.Id, clone.LanguageCode).ToList();
				metadata.Properties.ForEach(
					p =>
					{
						var localization = localizations.FirstOrDefault(l => l.EntityPropertyName.Equals(p.First));
						if (localization != null)
						{
							p.Second.SetValue(clone, localization.Text, null);
						}
					});
				clones.Add(clone);
			}

			return clones;
		}

		public override int SaveChanges()
		{
			this.ChangeTracker.Entries().Where(
				x => x.State == EntityState.Added || x.State == EntityState.Modified || x.State == EntityState.Deleted).ForEach(
					this.ProcessEntityOnSaveChanges);

			return base.SaveChanges();
		}

		#endregion

		#region private Methods

		private new void ProcessEntityOnSaveChanges(DbEntityEntry entry)
		{
			base.ProcessEntityOnSaveChanges(entry); //// <- important

			//// zpracujeme jen lokalizovatelne entity
			var localizedEntity = entry.Entity as LocalizedEntityBase;
			if (localizedEntity == null)
			{
				return;
			}

			//// pokud neni nastavenej jazyk, tak jde o defautlni lokalizaci
			if (string.IsNullOrEmpty(localizedEntity.LanguageCode))
			{
				localizedEntity.LanguageCode = this.DefaultLanguageCode;
			}

			var isDefaultLanguage = this.IsDefaultLanguage(localizedEntity.LanguageCode);

			var entityType = LocalizationHelper.GetEntityType(entry.Entity);
			if (entry.State == EntityState.Added && !isDefaultLanguage)
			{
				throw new NotSupportedException("Can not add entity in non-default language. You have to get entity in default language, change its values and then save it in required language.");
			}

			var entityKey = LocalizationHelper.GetEntityKey(entityType);

			if (entry.State == EntityState.Deleted)
			{
				this.Localizations.Where(
					x =>
					x.EntityName.Equals(entityKey) && x.EntityPkValue.Equals(localizedEntity.Id)
					&& (isDefaultLanguage || x.LanguageCode.Equals(localizedEntity.LanguageCode, StringComparison.OrdinalIgnoreCase)))
					.ForEach(x => this.Localizations.Remove(x));
				if (!isDefaultLanguage)
				{
					entry.State = EntityState.Unchanged;
				}
			}
			else if (!isDefaultLanguage)
			{
				var entityMetaData = LocalizationHelper.GetLocalizedEntityMetadata(entityKey);
				if (entityMetaData == null)
				{
					return; //// nemam zadne lokalizovatelne property, takova entita by tu vubec nemela byt! TODO: add log?
				}

				//// pokud jde o update, smazeme vsechny predchozi lokalizace u konkretni entity v danem jazyku
				if (entry.State == EntityState.Modified)
				{
					this.GetLocalizations(entityKey, localizedEntity.Id, localizedEntity.LanguageCode).ForEach(
						x => this.Localizations.Remove(x));
				}

				foreach (var prop in entityMetaData.Properties)
				{
					var value = prop.Second.GetValue(localizedEntity, null) as string;
					if (!string.IsNullOrEmpty(value))
					{
						this.Localizations.Add(
							new Localization
							{
								EntityName = entityKey,
								EntityPkValue = localizedEntity.Id,
								EntityPropertyName = prop.First,
								LanguageCode = localizedEntity.LanguageCode,
								Text = value
							});
					}

					if (entry.State != EntityState.Modified)
					{
						continue;
					}

					//// pokud jde o update, tak entite nastavim puvodni hodnoty, pac jinak by se po ulozeni prepsali defaultni hodnoty v entite tabulce
					var p = entry.Property(prop.First);
					p.CurrentValue = p.OriginalValue;
				}
			}
		}

		#endregion
	}
}