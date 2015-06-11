namespace Genitor.Library.Core.Entities
{
	using System;
	using System.ComponentModel.DataAnnotations;

	[Serializable]
	public abstract class EntityBase
	{
		#region ctor

		protected EntityBase()
		{
			this.CreatedOnUtc = DateTime.Now;
		}

		#endregion

		[Key, Required]
		public int Id { get; }

		public DateTime CreatedOnUtc { get; set; }

		public DateTime? UpdatedOnUtc { get; set; }

		public DateTime? ArchivedOnUtc { get; set; }

		public static bool operator ==(EntityBase x, EntityBase y)
		{
			return Equals(x, y);
		}

		public static bool operator !=(EntityBase x, EntityBase y)
		{
			return !(x == y);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as EntityBase);
		}

		public virtual bool Equals(EntityBase other)
		{
			if (other == null)
			{
				return false;
			}

			if (ReferenceEquals(this, other))
			{
				return true;
			}

			if (!IsTransient(this) &&
				!IsTransient(other) &&
				Equals(this.Id, other.Id))
			{
				var otherType = other.GetUnproxiedType();
				var thisType = this.GetUnproxiedType();
				return thisType.IsAssignableFrom(otherType) || otherType.IsAssignableFrom(thisType);
			}

			return false;
		}

		public override int GetHashCode()
		{
			return Equals(this.Id, default(int)) ? base.GetHashCode() : this.Id.GetHashCode();
		}

		public bool IsNew()
		{
			return IsTransient(this);
		}

		public bool IsArchived()
		{
			return this.ArchivedOnUtc.HasValue;
		}

		private static bool IsTransient(EntityBase obj)
		{
			return obj != null && Equals(obj.Id, default(int));
		}

		private Type GetUnproxiedType()
		{
			return this.GetType();
		}
	}
}