namespace Genitor.Library.Core
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>
	/// Represents a range of items.
	/// </summary>
	/// <typeparam name="T">The range type.</typeparam>
	public class Range<T> : IComparable<Range<T>>, IComparable<T>, IComparable where T : IComparable<T>
	{
		#region Declarations
		private readonly T _lowerBound;
		private readonly T _upperBound;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates the range.
		/// </summary>
		/// <param name="lowerBound">The lower bound of the range.</param>
		/// <param name="upperBound">The upper bound of the range.</param>
		internal Range(T lowerBound, T upperBound)
		{
			Guard.IsNotNull(lowerBound, "lowerBound");
			Guard.IsNotNull(upperBound, "upperBound");
			Guard.IsTrue(lowerBound.CompareTo(upperBound) <= 0, "lowerBound");

			_lowerBound = lowerBound;
			_upperBound = upperBound;
		}
		#endregion

		#region Properties
		/// <summary>
		/// The start of the range.
		/// </summary>
		public T LowerBound
		{
			get
			{
				return _lowerBound;
			}
		}

		/// <summary>
		/// The upper bound of the range.
		/// </summary>
		public T UpperBound
		{
			get
			{
				return _upperBound;
			}
		}
		#endregion

		#region Methods
		/// <summary>
		/// Indicates if the range contains <code>value</code>.
		/// </summary>
		/// <param name="value">The value to look for.</param>
		/// <returns>true if the range contains <code>value</code>, false otherwise.</returns>
		/// <exception cref="System.ArgumentNullException"><code>value</code> is null.</exception>
		public bool Contains(T value)
		{
			Guard.IsNotNull(value, "value");

			return ((LowerBound.CompareTo(value) <= 0) && (UpperBound.CompareTo(value) >= 0));
		}

		/// <summary>
		/// Indicates if the range contains <code>value</code>.
		/// </summary>
		/// <param name="value">A range to test.</param>
		/// <returns>true if the entire range in <code>value</code> is within this range.</returns>
		/// <exception cref="System.ArgumentNullException"><code>value</code> is null.</exception>
		public bool Contains(Range<T> value)
		{
			Guard.IsNotNull(value, "value");

			return ((LowerBound.CompareTo(value.LowerBound) <= 0) && (UpperBound.CompareTo(value.UpperBound) >= 0));
		}

		/// <summary>
		/// Indicates if the range is contained by <code>value</code>.
		/// </summary>
		/// <param name="value">A range to test.</param>
		/// <returns>true if the entire range is within <code>value</code>.</returns>
		/// <exception cref="System.ArgumentNullException"><code>value</code> is null.</exception>
		public bool IsContainedBy(Range<T> value)
		{
			Guard.IsNotNull(value, "value");

			return value.Contains(this);
		}

		/// <summary>
		/// Indicates if the range overlaps <code>value</code>.
		/// </summary>
		/// <param name="value">A range to test.</param>
		/// <returns>true if any of the range in <code>value</code> is within this range.</returns>
		/// <exception cref="System.ArgumentNullException"><code>value</code> is null.</exception>
		public bool Overlaps(Range<T> value)
		{
			Guard.IsNotNull(value, "value");

			return (Contains(value.LowerBound) || Contains(value.UpperBound) || value.Contains(LowerBound) || value.Contains(UpperBound));
		}

		/// <summary>
		/// Returns the range that represents the intersection of this range and <code>value</code>.
		/// </summary>
		/// <param name="value">The range to intersect with.</param>
		/// <returns>A range that contains the values that are common in both ranges, or null if there is no intersection.</returns>
		/// <exception cref="System.ArgumentNullException"><code>value</code> is null.</exception>
		/// <exception cref="System.ArgumentException"><code>value</code> does not overlap the range.</exception>
		public Range<T> Intersect(Range<T> value)
		{
			Guard.IsNotNull(value, "value");
			Guard.IsTrue(Overlaps(value), "value");    // Intersect makes no sense unless there is an overlap

			var start = LowerBound.CompareTo(value.LowerBound) > 0 ? LowerBound : value.LowerBound;
			return UpperBound.CompareTo(value.UpperBound) < 0 ? new Range<T>(start, UpperBound) : new Range<T>(start, value.UpperBound);
		}

		/// <summary>
		/// Returns the range that represents the union of this range and <code>value</code>.
		/// </summary>
		/// <param name="value">The range to union with.</param>
		/// <returns>A range that contains both ranges, or null if there is no union.</returns>
		/// <exception cref="System.ArgumentNullException"><code>value</code> is null.</exception>
		/// <exception cref="System.ArgumentException"><code>value</code> is not contiguous with the range.</exception>
		public Range<T> Union(Range<T> value)
		{
			Guard.IsNotNull(value, "value");
			Guard.IsTrue(IsContiguousWith(value), "value");    // Union makes no sense unless there is a contiguous border

			// If either one is a subset of the other, then is it the union
			if (Contains(value))
			{
				return this;
			}
			if (value.Contains(this))
			{
				return value;
			}
			var start = LowerBound.CompareTo(value.LowerBound) < 0 ? LowerBound : value.LowerBound;
			return UpperBound.CompareTo(value.UpperBound) > 0 ? new Range<T>(start, UpperBound) : new Range<T>(start, value.UpperBound);
		}

		/// <summary>
		/// Returns a range which contains the current range, minus <code>value</code>.
		/// </summary>
		/// <param name="value">The value to complement the range by.</param>
		/// <returns>The complemented range.</returns>
		/// <exception cref="System.ArgumentNullException"><code>value</code> is null.</exception>
		/// <exception cref="System.ArgumentException">
		/// <code>value</code> is contained by this range, complementing would lead to a split range.
		/// </exception>
		public Range<T> Complement(Range<T> value)
		{
			Guard.IsNotNull(value, "value");
			Guard.IsTrue(!Contains(value), "value");

			if (Overlaps(value))
			{
				T start;

				// If value's start and end straddle our start, move our start up to be values end.
				if ((LowerBound.CompareTo(value.LowerBound) > 0) && (LowerBound.CompareTo(value.UpperBound) < 0))
				{
					start = value.UpperBound;
				}
				else
				{
					start = LowerBound;
				}

				// If value's start and end straddle our end, move our end back down to be values start.
				if ((UpperBound.CompareTo(value.LowerBound) > 0) && (UpperBound.CompareTo(value.UpperBound) < 0))
				{
					return new Range<T>(start, value.LowerBound);
				}
				return new Range<T>(start, UpperBound);
			}
			return this;
		}

		/// <summary>
		/// Splits the range into two.
		/// </summary>
		/// <param name="position">The position to split the range at.</param>
		/// <returns>The split ranges.</returns>
		/// <exception cref="System.ArgumentNullException"><code>position</code> is null.</exception>
		/// <exception cref="System.ArgumentException"><code>position</code> is not contained within the range.</exception>
		public IEnumerable<Range<T>> Split(T position)
		{
			Guard.IsNotNull(position, "position");
			Guard.IsTrue(Contains(position), "position");

			if ((LowerBound.CompareTo(position) == 0) || (UpperBound.CompareTo(position) == 0))
			{
				// The position is at a boundary, so a split does not happen
				yield return this;
			}
			else
			{
				yield return Range.Create(LowerBound, position);
				yield return Range.Create(position, UpperBound);
			}
		}
		/// <summary>
		/// Iterates the range.
		/// </summary>
		/// <param name="incrementor">A function which takes a value, and returns the next value.</param>
		/// <returns>The items in the range.</returns>
		public IEnumerable<T> Iterate(Func<T, T> incrementor)
		{
			yield return LowerBound;
			T item = incrementor(LowerBound);
			while (UpperBound.CompareTo(item) >= 0)
			{
				yield return item;
				item = incrementor(item);
			}
		}

		/// <summary>
		/// Iterates the range in reverse.
		/// </summary>
		/// <param name="decrementor">A function which takes a value, and returns the previous value.</param>
		/// <returns>The items in the range.</returns>
		public IEnumerable<T> ReverseIterate(Func<T, T> decrementor)
		{
			yield return UpperBound;
			T item = decrementor(UpperBound);
			while (CompareTo(item) <= 0)
			{
				yield return item;
				item = decrementor(item);
			}
		}

		/// <summary>
		/// Indicates if this range is contiguous with <code>range</code>.
		/// </summary>
		/// <param name="range">The range to check.</param>
		/// <returns>true if the two ranges are contiguous, false otherwise.</returns>
		/// <remarks>Contiguous can mean containing, overlapping, or being next to.</remarks>
		public bool IsContiguousWith(Range<T> range)
		{
			if (Overlaps(range) || range.Overlaps(this) || range.Contains(this) || Contains(range))
			{
				return true;
			}

			// Once we remove overlapping and containing, only touching if available
			return ((UpperBound.Equals(range.LowerBound)) || (LowerBound.Equals(range.UpperBound)));
		}
		#endregion

		#region Overrides
		/// <summary>
		/// See <see cref="System.Object.ToString"/>.
		/// </summary>
		public override string ToString()
		{
			return "{" + LowerBound + "->" + UpperBound + "}";
		}

		/// <summary>
		/// See <see cref="System.Object.Equals(object)"/>.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is Range<T>)
			{
				var other = (Range<T>)obj;
				return ((CompareTo(other) == 0) && (UpperBound.CompareTo(other.UpperBound) == 0));
			}

			return false;
		}

		/// <summary>
		/// See <see cref="System.Object.GetHashCode"/>.
		/// </summary>
		public override int GetHashCode()
		{
			return LowerBound.GetHashCode();
		}
		#endregion

		#region Operators
		/// <summary>
		/// Overrides the equals operator.
		/// </summary>
		/// <param name="left">The left range.</param>
		/// <param name="right">The right range.</param>
		/// <returns>true if the two ranges are equal, false otherwise.</returns>
		public static bool operator ==(Range<T> left, Range<T> right)
		{
			// If both are null, or both are same instance, return true.
			if (ReferenceEquals(left, right))
			{
				return true;
			}

			// If one is null, but not both, return false.
			if (((object)left == null) || ((object)right == null))
			{
				return false;
			}

			return (left.CompareTo(right) == 0);
		}

		/// <summary>
		/// Overrides the not equals operator.
		/// </summary>
		/// <param name="left">The left range.</param>
		/// <param name="right">The right range.</param>
		/// <returns>true if the two ranges are equal, false otherwise.</returns>
		/// <summary>
		/// Overrides the equals operator.
		/// </summary>
		/// <returns>true if the two ranges are equal, false otherwise.</returns>
		public static bool operator !=(Range<T> left, Range<T> right)
		{
			return !(left == right);
		}

		/// <summary>
		/// Overrides the greater than operator.
		/// </summary>
		/// <param name="left">The left range.</param>
		/// <param name="right">The right range.</param>
		/// <returns>true if the <code>left</code> is greater than <code>right</code>, false otherwise.</returns>
		public static bool operator >(Range<T> left, Range<T> right)
		{
			return (left.CompareTo(right) > 0);
		}

		/// <summary>
		/// Overrides the less than operator.
		/// </summary>
		/// <param name="left">The left range.</param>
		/// <param name="right">The right range.</param>
		/// <returns>true if the <code>left</code> is less than <code>right</code>, false otherwise.</returns>
		public static bool operator <(Range<T> left, Range<T> right)
		{
			return (left.CompareTo(right) < 0);
		}

		/// <summary>
		/// Overrides the greater than operator.
		/// </summary>
		/// <param name="left">The left range.</param>
		/// <param name="right">The right range.</param>
		/// <returns>true if the <code>left</code> is greater than <code>right</code>, false otherwise.</returns>
		public static bool operator >(Range<T> left, T right)
		{
			return (left.CompareTo(right) > 0);
		}

		/// <summary>
		/// Overrides the less than operator.
		/// </summary>
		/// <param name="left">The left range.</param>
		/// <param name="right">The right range.</param>
		/// <returns>true if the <code>left</code> is less than <code>right</code>, false otherwise.</returns>
		public static bool operator <(Range<T> left, T right)
		{
			return (left.CompareTo(right) < 0);
		}

		/// <summary>
		/// Overrides the greater than or equal operator.
		/// </summary>
		/// <param name="left">The left range.</param>
		/// <param name="right">The right range.</param>
		/// <returns>true if the <code>left</code> is greater than or equal to <code>right</code>, false otherwise.</returns>
		public static bool operator >=(Range<T> left, Range<T> right)
		{
			return (left.CompareTo(right) >= 0);
		}

		/// <summary>
		/// Overrides the less than or equal to operator.
		/// </summary>
		/// <param name="left">The left range.</param>
		/// <param name="right">The right range.</param>
		/// <returns>true if the <code>left</code> is less than or equal to <code>right</code>, false otherwise.</returns>
		public static bool operator <=(Range<T> left, Range<T> right)
		{
			return (left.CompareTo(right) <= 0);
		}

		/// <summary>
		/// Overrides the greater than or equals operator.
		/// </summary>
		/// <param name="left">The left range.</param>
		/// <param name="right">The right range.</param>
		/// <returns>true if the <code>left</code> is greater than or equal to <code>right</code>, false otherwise.</returns>
		public static bool operator >=(Range<T> left, T right)
		{
			return (left.CompareTo(right) >= 0);
		}

		/// <summary>
		/// Overrides the less than or equals operator.
		/// </summary>
		/// <param name="left">The left range.</param>
		/// <param name="right">The right range.</param>
		/// <returns>true if the <code>left</code> is less than or equal to <code>right</code>, false otherwise.</returns>
		public static bool operator <=(Range<T> left, T right)
		{
			return (left.CompareTo(right) <= 0);
		}

		/// <summary>
		/// The complement operator.
		/// </summary>
		/// <param name="left">The left operand.</param>
		/// <param name="right">The right operand.</param>
		/// <returns>The complement of <code>left</code> and <code>right</code>.</returns>
		public static Range<T> operator ^(Range<T> left, Range<T> right)
		{
			return left.Complement(right);
		}

		/// <summary>
		/// The union operator.
		/// </summary>
		/// <param name="left">The left operand.</param>
		/// <param name="right">The right operand.</param>
		/// <returns>The union of <code>left</code> and <code>right</code>.</returns>
		public static Range<T> operator |(Range<T> left, Range<T> right)
		{
			return left.Union(right);
		}

		/// <summary>
		/// The intersection operator.
		/// </summary>
		/// <param name="left">The left operand.</param>
		/// <param name="right">The right operand.</param>
		/// <returns>The intersection of <code>left</code> and <code>right</code>.</returns>
		public static Range<T> operator &(Range<T> left, Range<T> right)
		{
			return left.Intersect(right);
		}
		#endregion

		#region IComparable<Range<T>> Members
		/// <summary>
		/// See <see cref="System.IComparable{T}.CompareTo"/>.
		/// </summary>
		public int CompareTo(Range<T> other)
		{
			return LowerBound.CompareTo(other.LowerBound);
		}
		#endregion

		#region IComparable<T> Members
		/// <summary>
		/// See <see cref="System.IComparable{T}.CompareTo"/>.
		/// </summary>
		public int CompareTo(T other)
		{
			return LowerBound.CompareTo(other);
		}
		#endregion

		#region IComparable Members
		/// <summary>
		/// See <see cref="System.IComparable.CompareTo"/>.
		/// </summary>
		public int CompareTo(object obj)
		{
			if (obj is Range<T>)
			{
				var other = (Range<T>)obj;
				return CompareTo(other);
			}
			if (obj is T)
			{
				var other = (T)obj;
				return CompareTo(other);
			}

			throw new InvalidOperationException(string.Format("Cannot compare to {0}", obj));
		}
		#endregion
	}

	/// <summary>
	/// Represents a range of items, with an associated value.
	/// </summary>
	/// <typeparam name="TKey">The key type.</typeparam>
	/// <typeparam name="TValue">The value type.</typeparam>
	public sealed class Range<TKey, TValue> : Range<TKey> where TKey : IComparable<TKey>
	{
		#region Declarations
		private readonly TValue _value;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates the range.
		/// </summary>
		/// <param name="lowerBound">The lower bound of the range.</param>
		/// <param name="upperBound">The upper bound of the range.</param>
		/// <param name="value">The value.</param>
		internal Range(TKey lowerBound, TKey upperBound, TValue value)
			: base(lowerBound, upperBound)
		{
			_value = value;
		}
		#endregion

		#region Properties
		/// <summary>
		/// The value for the range.
		/// </summary>
		public TValue Value
		{
			get
			{
				return _value;
			}
		}
		#endregion
	}

	/// <summary>
	/// Represents a range of array items, with an associated value.
	/// </summary>
	/// <typeparam name="T">The value type.</typeparam>
	public sealed class RangeArray<T> : Range<int>
	{
		#region Declarations
		private readonly T[] _values;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates the range.
		/// </summary>
		/// <param name="startIndex">The start index.</param>
		/// <param name="values">The values.</param>
		internal RangeArray(int startIndex, T[] values)
			: base(startIndex, startIndex + values.Length - 1)
		{
			_values = values;
		}
		#endregion

		#region Properties
		/// <summary>
		/// The values for the range.
		/// </summary>
		public T[] Values
		{
			get
			{
				return _values;
			}
		}
		#endregion
	}

	/// <summary>
	/// Contains a set of range methods.
	/// </summary>
	public static class Range
	{
		#region Creation
		/// <summary>
		/// Creates a range array.
		/// </summary>
		/// <typeparam name="T">The array type.</typeparam>
		/// <param name="startIndex">The start index for the array.</param>
		/// <param name="values">The array values.</param>
		/// <returns>The range array.</returns>
		/// <exception cref="System.ArgumentNullException"><code>values</code> is null.</exception>
		/// <exception cref="System.ArgumentException">
		/// <code>values</code> is empty, or <code>startIndex</code> is less than zero.
		/// </exception>
		public static RangeArray<T> Create<T>(int startIndex, T[] values)
		{
			Guard.IsTrue(startIndex >= 0, "startIndex");
			Guard.IsNotNullOrEmpty(values, "values");

			return new RangeArray<T>(startIndex, values);
		}

		/// <summary>
		/// Creates a range array.
		/// </summary>
		/// <typeparam name="T">The array type.</typeparam>
		/// <param name="range">The range for the array.</param>
		/// <param name="values">The array values.</param>
		/// <returns>The range array.</returns>
		/// <exception cref="System.ArgumentNullException"><code>range</code> or <code>values</code> is null.</exception>
		/// <exception cref="System.ArgumentException">
		/// <code>values</code> is empty, or the range does not match the values.Length.
		/// </exception>
		public static RangeArray<T> Create<T>(Range<int> range, T[] values)
		{
			Guard.IsNotNull(range, "range");
			Guard.IsNotNullOrEmpty(values, "values");
			Guard.IsTrue((values.Length == (range.UpperBound - range.LowerBound)), "range");

			return new RangeArray<T>(range.LowerBound, values);
		}

		/// <summary>
		/// Creates an inclusive range.
		/// </summary>
		/// <typeparam name="T">The type of range.</typeparam>
		/// <param name="from">The from value.</param>
		/// <param name="to">The to value.</param>
		/// <returns>The range.</returns>
		/// <exception cref="System.ArgumentNullException"><code>from</code> or <code>to</code> is null.</exception>
		public static Range<T> Create<T>(T from, T to) where T : IComparable<T>
		{
			Guard.IsNotNull(from, "from");
			Guard.IsNotNull(to, "to");

			return new Range<T>(from, to);
		}

		/// <summary>
		/// Creates an inclusive range.
		/// </summary>
		/// <typeparam name="TKey">The type of range.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="from">The from value.</param>
		/// <param name="to">The to value.</param>
		/// <param name="value">The value.</param>
		/// <returns>The range.</returns>
		/// <exception cref="System.ArgumentNullException"><code>from</code> or <code>to</code> is null.</exception>
		public static Range<TKey, TValue> Create<TKey, TValue>(TKey from, TKey to, TValue value) where TKey : IComparable<TKey>
		{
			Guard.IsNotNull(from, "from");
			Guard.IsNotNull(to, "to");

			return new Range<TKey, TValue>(from, to, value);
		}
		#endregion

		#region Enumerables
		/// <summary>
		/// Makes a Range{TKey, TValue} enumerator covariant with Range{TKey}
		/// </summary>
		/// <typeparam name="TKey">The range type.</typeparam>
		/// <typeparam name="TValue">The value type.</typeparam>
		/// <param name="ranges">The ranges to make covariant.</param>
		/// <remarks>Yes I know it doesn't actually MAKE them covariant, but how else would you describe it?</remarks>
		internal static IEnumerable<Range<TKey>> MakeCovariant<TKey, TValue>(this IEnumerable<Range<TKey, TValue>> ranges) where TKey : IComparable<TKey>
		{
			Guard.IsNotNull(ranges, "ranges");

			foreach (Range<TKey, TValue> item in ranges)
			{
				yield return (Range<TKey>)item;
			}
		}

		/// <summary>
		/// Makes a RangeArray{T} enumerator covariant with Range{int}
		/// </summary>
		/// <typeparam name="T">The value type.</typeparam>
		/// <param name="ranges">The ranges to make covariant.</param>
		/// <remarks>Yes I know it doesn't actually MAKE them covariant, but how else would you describe it?</remarks>
		private static IEnumerable<Range<int>> MakeCovariant<T>(this IEnumerable<RangeArray<T>> ranges)
		{
			Guard.IsNotNull(ranges, "ranges");

			foreach (RangeArray<T> item in ranges)
			{
				yield return (Range<int>)item;
			}
		}

		/// <summary>
		/// Indicates if the ranges contain the range.
		/// </summary>
		/// <typeparam name="T">The range type.</typeparam>
		/// <param name="ranges">The ranges to look in.</param>
		/// <param name="from">The start to look for.</param>
		/// <param name="to">The end to look for.</param>
		/// <returns>true if <code>ranges</code> contain the range, false otherwise.</returns>
		/// <exception cref="System.ArgumentNullException"><code>ranges</code> or <code>from</code> is null, or <code>to</code> is null.</exception>
		/// <exception cref="System.ArgumentException"><code>from</code> is greater than <code>to</code>.</exception>
		public static bool Contains<T>(this IEnumerable<Range<T>> ranges, T from, T to) where T : IComparable<T>
		{
			Guard.IsNotNull(ranges, "ranges");
			Guard.IsNotNull(from, "from");
			Guard.IsNotNull(to, "to");
			Guard.IsTrue(from.CompareTo(to) <= 0, "from");

			return ranges.Contains(Create(from, to));
		}

		/// <summary>
		/// Indicates if the ranges contain the range.
		/// </summary>
		/// <typeparam name="TKey">The range key type.</typeparam>
		/// <typeparam name="TValue">THe range value type.</typeparam>
		/// <param name="ranges">The ranges to look in.</param>
		/// <param name="from">The start to look for.</param>
		/// <param name="to">The end to look for.</param>
		/// <returns>true if <code>ranges</code> contain the range, false otherwise.</returns>
		/// <exception cref="System.ArgumentNullException"><code>ranges</code> or <code>from</code> is null, or <code>to</code> is null.</exception>
		/// <exception cref="System.ArgumentException"><code>from</code> is greater than <code>to</code>.</exception>
		public static bool Contains<TKey, TValue>(this IEnumerable<Range<TKey, TValue>> ranges, TKey from, TKey to) where TKey : IComparable<TKey>
		{
			return ranges.MakeCovariant().Contains(from, to);
		}

		/// <summary>
		/// Indicates if the ranges contain the range.
		/// </summary>
		/// <typeparam name="T">The range array type.</typeparam>
		/// <param name="ranges">The ranges to look in.</param>
		/// <param name="from">The start to look for.</param>
		/// <param name="to">The end to look for.</param>
		/// <returns>true if <code>ranges</code> contain the range, false otherwise.</returns>
		/// <exception cref="System.ArgumentNullException"><code>ranges</code> or <code>from</code> is null, or <code>to</code> is null.</exception>
		/// <exception cref="System.ArgumentException"><code>from</code> is greater than <code>to</code>.</exception>
		public static bool Contains<T>(this IEnumerable<RangeArray<T>> ranges, int from, int to)
		{
			return ranges.MakeCovariant().Contains(from, to);
		}

		/// <summary>
		/// Indicates if the ranges contain the range.
		/// </summary>
		/// <typeparam name="T">The range type.</typeparam>
		/// <param name="ranges">The ranges to look in.</param>
		/// <param name="range">The item to look for.</param>
		/// <returns>true if <code>ranges</code> contain <code>range</code>, false otherwise.</returns>
		/// <exception cref="System.ArgumentNullException"><code>ranges</code> or <code>range</code> is null.</exception>
		public static bool Contains<T>(this IEnumerable<Range<T>> ranges, Range<T> range) where T : IComparable<T>
		{
			Guard.IsNotNull(ranges, "ranges");
			Guard.IsNotNull(range, "range");

			return ranges.Overlapped(range).Coalesce().Any(item => item.Contains(range));
		}

		/// <summary>
		/// Indicates if the ranges contain the range.
		/// </summary>
		/// <typeparam name="TKey">The range key type.</typeparam>
		/// <typeparam name="TValue">The range value type.</typeparam>
		/// <param name="ranges">The ranges to look in.</param>
		/// <param name="range">The item to look for.</param>
		/// <returns>true if <code>ranges</code> contain <code>range</code>, false otherwise.</returns>
		/// <exception cref="System.ArgumentNullException"><code>ranges</code> or <code>range</code> is null.</exception>
		public static bool Contains<TKey, TValue>(this IEnumerable<Range<TKey, TValue>> ranges, Range<TKey> range) where TKey : IComparable<TKey>
		{
			return ranges.MakeCovariant().Contains(range);
		}

		/// <summary>
		/// Indicates if the range arrays contain the range.
		/// </summary>
		/// <typeparam name="T">The range value type.</typeparam>
		/// <param name="ranges">The ranges to look in.</param>
		/// <param name="range">The item to look for.</param>
		/// <returns>true if <code>ranges</code> contain <code>range</code>, false otherwise.</returns>
		/// <exception cref="System.ArgumentNullException"><code>ranges</code> or <code>range</code> is null.</exception>
		public static bool Contains<T>(this IEnumerable<RangeArray<T>> ranges, Range<int> range)
		{
			return ranges.MakeCovariant().Contains(range);
		}

		/// <summary>
		/// Indicates if the ranges contain the item.
		/// </summary>
		/// <typeparam name="T">The range type.</typeparam>
		/// <param name="ranges">The ranges to look in.</param>
		/// <param name="item">The item to look for.</param>
		/// <returns>true if <code>ranges</code> contain <code>item</code>, false otherwise.</returns>
		/// <exception cref="System.ArgumentNullException"><code>ranges</code> or <code>item</code> is null.</exception>
		public static bool Contains<T>(this IEnumerable<Range<T>> ranges, T item) where T : IComparable<T>
		{
			Guard.IsNotNull(ranges, "ranges");
			Guard.IsNotNull(item, "item");

			return ranges.Any(range => range.Contains(item));
		}

		/// <summary>
		/// Indicates if the ranges contain the item.
		/// </summary>
		/// <typeparam name="TKey">The range key type.</typeparam>
		/// <typeparam name="TValue">The range value type.</typeparam>
		/// <param name="ranges">The ranges to look in.</param>
		/// <param name="item">The item to look for.</param>
		/// <returns>true if <code>ranges</code> contain <code>item</code>, false otherwise.</returns>
		/// <exception cref="System.ArgumentNullException"><code>ranges</code> or <code>item</code> is null.</exception>
		public static bool Contains<TKey, TValue>(this IEnumerable<Range<TKey, TValue>> ranges, TKey item) where TKey : IComparable<TKey>
		{
			return ranges.MakeCovariant().Contains(item);
		}

		/// <summary>
		/// Indicates if the range arrays contain the item.
		/// </summary>
		/// <typeparam name="T">The range value type.</typeparam>
		/// <param name="ranges">The ranges to look in.</param>
		/// <param name="item">The item to look for.</param>
		/// <returns>true if <code>ranges</code> contain <code>item</code>, false otherwise.</returns>
		/// <exception cref="System.ArgumentNullException"><code>ranges</code> or <code>item</code> is null.</exception>
		public static bool Contains<T>(this IEnumerable<RangeArray<T>> ranges, int item)
		{
			return ranges.MakeCovariant().Contains(item);
		}

		/// <summary>
		/// Sorts the ranges.
		/// </summary>
		/// <typeparam name="T">The type of range.</typeparam>
		/// <param name="ranges">The sorted ranges.</param>
		/// <returns>The sorted ranges.</returns>
		/// <exception cref="System.ArgumentNullException"><code>ranges</code> is null.</exception>
		public static IEnumerable<Range<T>> Sort<T>(this IEnumerable<Range<T>> ranges) where T : IComparable<T>
		{
			Guard.IsNotNull(ranges, "ranges");

			var list = new List<Range<T>>(ranges);
			list.Sort();
			return list;
		}

		/// <summary>
		/// Sorts the ranges.
		/// </summary>
		/// <typeparam name="TKey">The range key type.</typeparam>
		/// <typeparam name="TValue">The range value type.</typeparam>
		/// <param name="ranges">The sorted ranges.</param>
		/// <returns>The sorted ranges.</returns>
		/// <exception cref="System.ArgumentNullException"><code>ranges</code> is null.</exception>
		public static IEnumerable<Range<TKey>> Sort<TKey, TValue>(this IEnumerable<Range<TKey, TValue>> ranges) where TKey : IComparable<TKey>
		{
			return ranges.MakeCovariant().Sort();
		}

		/// <summary>
		/// Sorts the range arrays.
		/// </summary>
		/// <typeparam name="T">The range value type.</typeparam>
		/// <param name="ranges">The sorted ranges.</param>
		/// <returns>The sorted ranges.</returns>
		/// <exception cref="System.ArgumentNullException"><code>ranges</code> is null.</exception>
		public static IEnumerable<Range<int>> Sort<T>(this IEnumerable<RangeArray<T>> ranges)
		{
			return ranges.MakeCovariant().Sort();
		}

		/// <summary>
		/// Coaleses the ranges.
		/// </summary>
		/// <typeparam name="T">The range key type.</typeparam>
		/// <param name="ranges">The ranges to coalesce.</param>
		/// <returns>The coalesced ranges.</returns>
		/// <exception cref="System.ArgumentNullException"><code>ranges</code> is null.</exception>
		public static IEnumerable<Range<T>> Coalesce<T>(this IEnumerable<Range<T>> ranges) where T : IComparable<T>
		{
			Guard.IsNotNull(ranges, "ranges");

			Range<T> previous = null;
			foreach (Range<T> item in ranges.Sort())
			{
				if (previous == null)
				{
					previous = item;
				}
				else
				{
					// Possible coalescing
					if (previous.IsContiguousWith(item))
					{
						// Intersect the ranges
						previous = previous | item;
					}
					else
					{
						yield return previous;
						previous = item;
					}
				}
			}

			if (previous != null)
			{
				yield return previous;
			}
		}

		/// <summary>
		/// Coaleses the ranges.
		/// </summary>
		/// <typeparam name="TKey">The range key type.</typeparam>
		/// <typeparam name="TValue">The range value type.</typeparam>
		/// <param name="ranges">The ranges to coalesce.</param>
		/// <returns>The coalesced ranges.</returns>
		/// <exception cref="System.ArgumentNullException"><code>ranges</code> is null.</exception>
		public static IEnumerable<Range<TKey>> Coalesce<TKey, TValue>(this IEnumerable<Range<TKey, TValue>> ranges) where TKey : IComparable<TKey>
		{
			return ranges.MakeCovariant().Coalesce();
		}

		/// <summary>
		/// Coaleses the range arrays.
		/// </summary>
		/// <typeparam name="T">The range value type.</typeparam>
		/// <param name="ranges">The ranges to coalesce.</param>
		/// <returns>The coalesced ranges.</returns>
		/// <exception cref="System.ArgumentNullException"><code>ranges</code> is null.</exception>
		public static IEnumerable<Range<int>> Coalesce<T>(this IEnumerable<RangeArray<T>> ranges)
		{
			return ranges.MakeCovariant().Coalesce();
		}

		/// <summary>
		/// Fetches the ranges which are overlapped by this range.
		/// </summary>
		/// <typeparam name="T">The type of range.</typeparam>
		/// <param name="ranges">The ranges.</param>
		/// <param name="range">The range to test for overlappping.</param>
		/// <returns>The overlapped ranges.</returns>
		/// <exception cref="System.ArgumentNullException"><code>ranges</code> or <code>range</code> is null.</exception>
		public static IEnumerable<Range<T>> Overlapped<T>(this IEnumerable<Range<T>> ranges, Range<T> range) where T : IComparable<T>
		{
			Guard.IsNotNull(ranges, "ranges");
			Guard.IsNotNull(range, "range");

			return ranges.Where(item => item.Overlaps(range));
		}

		/// <summary>
		/// Fetches the ranges which are overlapped by this range.
		/// </summary>
		/// <typeparam name="TKey">The type of range.</typeparam>
		/// <typeparam name="TValue">The range value type.</typeparam>
		/// <param name="ranges">The ranges.</param>
		/// <param name="range">The range to test for overlappping.</param>
		/// <returns>The overlapped ranges.</returns>
		/// <exception cref="System.ArgumentNullException"><code>ranges</code> or <code>range</code> is null.</exception>
		public static IEnumerable<Range<TKey>> Overlapped<TKey, TValue>(this IEnumerable<Range<TKey, TValue>> ranges, Range<TKey> range) where TKey : IComparable<TKey>
		{
			return ranges.MakeCovariant().Overlapped(range);
		}

		/// <summary>
		/// Fetches the range arrays which are overlapped by this range.
		/// </summary>
		/// <typeparam name="T">The range value type.</typeparam>
		/// <param name="ranges">The ranges.</param>
		/// <param name="range">The range to test for overlappping.</param>
		/// <returns>The overlapped ranges.</returns>
		/// <exception cref="System.ArgumentNullException"><code>ranges</code> or <code>range</code> is null.</exception>
		public static IEnumerable<Range<int>> Overlapped<T>(this IEnumerable<RangeArray<T>> ranges, Range<int> range)
		{
			return ranges.MakeCovariant().Overlapped(range);
		}

		/// <summary>
		/// Searches a set of ranges, and returns the matching items.
		/// </summary>
		/// <typeparam name="T">The type of range.</typeparam>
		/// <param name="ranges">The ranges to search.</param>
		/// <param name="predicate">The predicate to match.</param>
		/// <returns>The matching ranges.</returns>
		public static IEnumerable<Range<T>> Find<T>(this IEnumerable<Range<T>> ranges, Func<Range<T>, bool> predicate) where T : IComparable<T>
		{
			Guard.IsNotNull(ranges, "ranges");
			Guard.IsNotNull(predicate, "predicate");

			return ranges.Where(predicate);
		}

		/// <summary>
		/// Searches a set of ranges, and returns the matching items.
		/// </summary>
		/// <typeparam name="TKey">The type of range.</typeparam>
		/// <typeparam name="TValue">The range value type.</typeparam>
		/// <param name="ranges">The ranges to search.</param>
		/// <param name="predicate">The predicate to match.</param>
		/// <returns>The matching ranges.</returns>
		public static IEnumerable<Range<TKey>> Find<TKey, TValue>(this IEnumerable<Range<TKey, TValue>> ranges, Func<Range<TKey>, bool> predicate) where TKey : IComparable<TKey>
		{
			return ranges.MakeCovariant().Find(predicate);
		}

		/// <summary>
		/// Searches a set of range arrays, and returns the matching items.
		/// </summary>
		/// <typeparam name="T">The range value type.</typeparam>
		/// <param name="ranges">The ranges to search.</param>
		/// <param name="predicate">The predicate to match.</param>
		/// <returns>The matching ranges.</returns>
		public static IEnumerable<Range<int>> Find<T>(this IEnumerable<RangeArray<T>> ranges, Func<Range<int>, bool> predicate)
		{
			return ranges.MakeCovariant().Find(predicate);
		}

		/// <summary>
		/// Searches a range, and returns the first matching item.
		/// </summary>
		/// <typeparam name="T">The type of range.</typeparam>
		/// <param name="ranges">The ranges to search.</param>
		/// <param name="predicate">The predicate to match.</param>
		/// <returns>The first matching range, or null if no ranges match the predicate.</returns>
		public static Range<T> FindFirst<T>(this IEnumerable<Range<T>> ranges, Func<Range<T>, bool> predicate) where T : IComparable<T>
		{
			Guard.IsNotNull(ranges, "ranges");
			Guard.IsNotNull(predicate, "predicate");

			return ranges.Find(predicate).FirstOrDefault();
		}

		/// <summary>
		/// Searches a set of ranges, and returns the first matching items.
		/// </summary>
		/// <typeparam name="TKey">The type of range.</typeparam>
		/// <typeparam name="TValue">The range value type.</typeparam>
		/// <param name="ranges">The ranges to search.</param>
		/// <param name="predicate">The predicate to match.</param>
		/// <returns>The first matching range, or null.</returns>
		public static Range<TKey, TValue> FindFirst<TKey, TValue>(this IEnumerable<Range<TKey, TValue>> ranges, Func<Range<TKey>, bool> predicate) where TKey : IComparable<TKey>
		{
			return ranges.MakeCovariant().FindFirst(predicate) as Range<TKey, TValue>;
		}

		/// <summary>
		/// Searches a set of range arrays, and returns the first matching items.
		/// </summary>
		/// <typeparam name="T">The range value type.</typeparam>
		/// <param name="ranges">The ranges to search.</param>
		/// <param name="predicate">The predicate to match.</param>
		/// <returns>The first matching range, or null.</returns>
		public static RangeArray<T> FindFirst<T>(this IEnumerable<RangeArray<T>> ranges, Func<Range<int>, bool> predicate)
		{
			return ranges.MakeCovariant().FindFirst(predicate) as RangeArray<T>;
		}
		#endregion
	}
}