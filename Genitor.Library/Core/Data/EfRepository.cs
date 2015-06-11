namespace Genitor.Library.Core.Data
{
	using System;
	using System.Data.Entity;
	using System.Data.Entity.Validation;
	using System.Diagnostics;
	using System.Linq;

	using Genitor.Library.Core.Entities;

	/// <summary>
	/// Entity Framework repository.
	/// </summary>
	public partial class EfRepository<T> : IRepository<T> where T : EntityBase
	{
		private readonly IDbContext _context;

		private IDbSet<T> _entities;

		#region ctors

		public EfRepository(IDbContext context)
		{
			this._context = context;
		}

		#endregion

		public virtual IQueryable<T> Table
		{
			get
			{
				return this.Entities;
			}
		}

		private IDbSet<T> Entities
		{
			get
			{
				return this._entities ?? (this._entities = this._context.Set<T>());
			}
		}

		public T GetById(object id)
		{
			return this.Entities.Find(id);
		}

		public void Insert(T entity)
		{
			try
			{
				if (entity == null)
				{
					throw new ArgumentNullException(nameof(entity));
				}

				this.Entities.Add(entity);

				this._context.SaveChanges();
			}
			catch (DbEntityValidationException e)
			{
				throw ProcessDbEntityValidationExceptionEntityValidationErrors(e);
			}
		}

		public void Update(T entity)
		{
			try
			{
				if (entity == null)
				{
					throw new ArgumentNullException(nameof(entity));
				}

				this._context.SaveChanges();
			}
			catch (DbEntityValidationException e)
			{
				throw ProcessDbEntityValidationExceptionEntityValidationErrors(e);
			}
		}

		public void Delete(T entity)
		{
			try
			{
				if (entity == null)
				{
					throw new ArgumentNullException(nameof(entity));
				}

				this.Entities.Remove(entity);

				this._context.SaveChanges();
			}
			catch (DbEntityValidationException e)
			{
				throw ProcessDbEntityValidationExceptionEntityValidationErrors(e);
			}
		}

		private static Exception ProcessDbEntityValidationExceptionEntityValidationErrors(DbEntityValidationException e)
		{
			var msg = e.EntityValidationErrors.Aggregate(string.Empty, (current1, validationErrors) => validationErrors.ValidationErrors.Aggregate(current1, (current, validationError) => current + (string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage) + Environment.NewLine)));
			Debug.Print(msg);
			return new Exception(msg, e);
		}
	}
}
