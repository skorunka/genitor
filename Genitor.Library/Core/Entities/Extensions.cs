namespace Genitor.Library.Core.Entities
{
	using System.Data.Entity.Core.Objects;
	using System.Linq;

	public static class Extensions
	{
		/// <summary>
		/// Indicates if the Type is EF proxy class.
		/// </summary>
		public static bool IsProxy(this object type)
		{
			return type != null && ObjectContext.GetObjectType(type.GetType()) != type.GetType();
		}

		public static IQueryable<T> IncludeArchived<T>(this IQueryable<T> query, bool include) where T : EntityBase
		{
			return !include ? query.Where(x => x.ArchivedOnUtc == null) : query;
		}
	}
}
