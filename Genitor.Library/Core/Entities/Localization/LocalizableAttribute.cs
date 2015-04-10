namespace Genitor.Library.Core.Entities.Localization
{
	using System;

	using Genitor.Library.Core.Localization;

	/// <summary>
	/// Urcuje, zda je property localizovatelna.
	/// Muze nastavit i hodnotu "EntityPropertyName" slozeneho klice <see cref="Localization"/>.
	/// S timto atributem pracuje <see cref="LocalizationHelper"/>
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public sealed class LocalizableAttribute : Attribute
	{
		#region ctor

		public LocalizableAttribute(string storePropertyNamer = null)
		{
			this.StorePropertyName = storePropertyNamer;
		}

		#endregion

		/// <summary>
		/// The name of Property under which is value stored in Localizations table(column "EntityPropertyName").
		/// </summary>
		public string StorePropertyName { get; set; }
	}
}