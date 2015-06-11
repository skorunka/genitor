namespace Genitor.Library.Core
{
	using System;

	/// <summary>
	/// Obecna struktura pro ulozeni dvou hodnot, resp. usporadane dvojice (inspirace v C++ stl::pair)
	/// .NET ma take Pair class ale je v System.Web.UI.
	/// </summary>
	/// <typeparam name="TFirst">Typ prvni hodnoty</typeparam>
	/// <typeparam name="TSecond">Typ druhe hodnoty</typeparam>
	[Serializable]
	public struct Pair<TFirst, TSecond>
		where TFirst : class
		where TSecond : class
	{
		#region ctors

		public Pair(TFirst first, TSecond second)
			: this()
		{
			this.First = first;
			this.Second = second;
		}

		#endregion

		public TFirst First { get; }

		public TSecond Second { get; }

		public static bool operator ==(Pair<TFirst, TSecond> p1, Pair<TFirst, TSecond> p2)
		{
			return p1.Equals(p2);
		}

		public static bool operator !=(Pair<TFirst, TSecond> p1, Pair<TFirst, TSecond> p2)
		{
			return !p1.Equals(p2);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Pair<TFirst, TSecond>))
			{
				return false;
			}

			if (Equals(obj, this))
			{
				return true;
			}

			var pair2 = (Pair<TFirst, TSecond>)obj;

			return
				((this.First == null && pair2.First == null) || (this.First != null && this.First.Equals(pair2.First))) &&
				((this.Second == null && pair2.Second == null) || (this.Second != null && this.Second.Equals(pair2.Second)));
		}

		public override int GetHashCode()
		{
			return (this.First?.GetHashCode() ?? 0) ^ (this.Second == null ? 0 : this.Second.GetHashCode());
		}

		public override string ToString()
		{
			return string.Format("[{0}, {1}]", this.First, this.Second);
		}
	}
}
