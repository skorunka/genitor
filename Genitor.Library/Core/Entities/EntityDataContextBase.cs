namespace Genitor.Library.Core.Entities
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.Common;
	using System.Data.Entity;
	using System.Data.Entity.Core.Objects;
	using System.Data.Entity.Infrastructure;
	using System.Linq;

	using Data;

	public abstract class EntityDataContextBase : DbContext, IDbContext
	{
		#region ctors

		protected EntityDataContextBase()
		{
		}

		protected EntityDataContextBase(string nameOrConnectionString)
			: base(nameOrConnectionString)
		{
		}

		#endregion

		public ObjectContext ObjectContext => ((IObjectContextAdapter)this).ObjectContext;

		public new IDbSet<TEntity> Set<TEntity>() where TEntity : EntityBase
		{
			return base.Set<TEntity>();
		}

		/// <summary>
		/// Execute stores procedure and load a list of entities at the end.
		/// </summary>
		public IList<TEntity> ExecuteStoredProcedureList<TEntity>(string commandText, params object[] parameters) where TEntity : EntityBase, new()
		{
			//// HACK: Entity Framework Code First doesn't support doesn't support output parameters
			//// That's why we have to manually create command and execute it.
			//// Just wait until EF Code First starts support them
			//// More info: http://weblogs.asp.net/dwahlin/archive/2011/09/23/using-entity-framework-code-first-with-stored-procedures-that-have-output-parameters.aspx

			var hasOutputParameters = false;
			if (parameters != null)
			{
				foreach (var outputP in parameters.OfType<DbParameter>().Where(outputP => outputP.Direction == ParameterDirection.InputOutput || outputP.Direction == ParameterDirection.Output))
				{
					hasOutputParameters = true;
				}
			}

			if (!hasOutputParameters)
			{
				//// no output parameters
				var result = this.Database.SqlQuery<TEntity>(commandText, parameters).ToList();
				for (var i = 0; i < result.Count; i++)
				{
					result[i] = this.AttachEntityToContext(result[i]);
				}

				return result;

				////var result = context.ExecuteStoreQuery<TEntity>(commandText, parameters).ToList();
				////foreach (var entity in result)
				////    Set<TEntity>().Attach(entity);
				////return result;
			}

			var connection = this.Database.Connection;

			//// open the connection for use
			if (connection.State == ConnectionState.Closed)
			{
				connection.Open();
			}

			//// don`t close Connection!

			using (var cmd = connection.CreateCommand())
			{
				cmd.CommandText = commandText;
				cmd.CommandType = CommandType.StoredProcedure;

				foreach (var p in parameters)
				{
					cmd.Parameters.Add(p);
				}

				using (var reader = cmd.ExecuteReader())
				{
					//// return reader.DataReaderToObjectList<TEntity>();

					var result = this.ObjectContext.Translate<TEntity>(reader).ToList();
					for (var i = 0; i < result.Count; i++)
					{
						result[i] = this.AttachEntityToContext(result[i]);
					}

					reader.Close();
					return result;
				}
			}
		}

		/// <summary>
		/// Creates a raw SQL query that will return elements of the given generic type.  The type can be any type that has properties that match the names of the columns returned from the query, or can be a simple primitive type. The type does not have to be an entity type. The results of this query are never tracked by the context even if the type of object returned is an entity type.
		/// </summary>
		/// <typeparam name="TElement">The type of object returned by the query.</typeparam>
		/// <param name="sql">The SQL query string.</param>
		/// <param name="parameters">The parameters to apply to the SQL query string.</param>
		/// <returns>Result</returns>
		public IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters)
		{
			return this.Database.SqlQuery<TElement>(sql, parameters);
		}

		/// <summary>
		/// Executes the given DDL/DML command against the database.
		/// </summary>
		/// <param name="sql">The command string.</param>
		/// <param name="timeout">Timeout value, in seconds. A null value indicates that the default value of the underlying provider will be used.</param>
		/// <param name="parameters">The parameters to apply to the command string.</param>
		/// <returns>The result returned by the database after executing the command.</returns>
		public int ExecuteSqlCommand(string sql, int? timeout = null, params object[] parameters)
		{
			int? previousTimeout = null;
			if (timeout.HasValue)
			{
				//// store previous timeout
				previousTimeout = ((IObjectContextAdapter)this).ObjectContext.CommandTimeout;
				((IObjectContextAdapter)this).ObjectContext.CommandTimeout = timeout;
			}

			var result = this.Database.ExecuteSqlCommand(sql, parameters);

			if (timeout.HasValue)
			{
				//// Set previous timeout back
				((IObjectContextAdapter)this).ObjectContext.CommandTimeout = previousTimeout;
			}

			return result;
		}

		/// <summary>
		/// Tato metoda se musi volat v zdedene tride pro kazdy Entry v SaveChanges.
		/// Viz <see cref="LocalizationDataContextBase"/>
		/// </summary>
		protected virtual void ProcessEntityOnSaveChanges(DbEntityEntry entry)
		{
			var entityBase = entry.Entity as EntityBase;
			if (entityBase == null)
			{
				return;
			}

			switch (entry.State)
			{
				case EntityState.Added:
					entityBase.CreatedOnUtc = DateTime.Now;
					entityBase.UpdatedOnUtc = null;
					entityBase.ArchivedOnUtc = null;
					break;

				case EntityState.Modified:
					entityBase.UpdatedOnUtc = DateTime.Now;
					entityBase.ArchivedOnUtc = null;
					break;

				case EntityState.Deleted:
					if (entityBase.ArchivedOnUtc.HasValue)
					{
						return;
					}

					entityBase.ArchivedOnUtc = DateTime.Now;
					entry.State = EntityState.Modified;
					break;
			}
		}

		/// <summary>
		/// Attach an entity to the context or return an already attached entity (if it was already attached).
		/// </summary>
		/// <typeparam name="TEntity">TEntity</typeparam>
		/// <param name="entity">Entity</param>
		/// <returns>Attached entity</returns>
		protected virtual TEntity AttachEntityToContext<TEntity>(TEntity entity) where TEntity : EntityBase, new()
		{
			//// little hack here until Entity Framework really supports stored procedures
			//// otherwise, navigation properties of loaded entities are not loaded until an entity is attached to the context
			var alreadyAttached = this.Set<TEntity>().Local.FirstOrDefault(x => x.Id == entity.Id);

			if (alreadyAttached != null)
			{
				return alreadyAttached;
			}

			this.Set<TEntity>().Attach(entity);
			return entity;
		}
	}
}