namespace Genitor.Library.Core.Data
{
	using System.Linq;

	using Genitor.Library.Core.Entities;

	/// <summary>
	/// Repository pattern.
	/// </summary>
	public partial interface IRepository<T> where T : EntityBase
	{
		IQueryable<T> Table { get; }

		T GetById(object id);

		void Insert(T entity);

		void Update(T entity);

		void Delete(T entity);
	}
}
