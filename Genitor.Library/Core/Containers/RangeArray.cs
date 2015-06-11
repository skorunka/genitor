namespace Genitor.Library.Core
{
	/// <summary>
	/// Represents a range of array items, with an associated value.
	/// </summary>
	/// <typeparam name="T">The value type.</typeparam>
	public sealed class RangeArray<T> : Range<int>
	{
		#region Declarations

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
			this.Values = values;
		}
		#endregion

		#region Properties
		/// <summary>
		/// The values for the range.
		/// </summary>
		public T[] Values { get; }

		#endregion
	}
}