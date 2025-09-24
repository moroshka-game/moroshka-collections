namespace Moroshka.Collections
{

public sealed partial class IntHashMap<T>
{
	/// <summary>
	/// A struct that lists the indices of valid key-value pairs in the hash map.
	/// </summary>
	public struct Enumerator
	{
		private readonly IntHashMap<T> _hashMap;
		private int _index;

		/// <summary>
		/// Initializes a new instance of the <see cref="Enumerator"/> struct.
		/// </summary>
		/// <param name="hashMap">The hash map to enumerate.</param>
		internal Enumerator(IntHashMap<T> hashMap)
		{
			_hashMap = hashMap;
			_index = 0;
			Current = 0;
		}

		/// <summary>
		/// Gets the current index of the enumeration.
		/// </summary>
		public int Current { get; private set; }

		/// <summary>
		/// Advances the enumerator to the next valid key-value pair.
		/// </summary>
		/// <returns>True if the enumerator was advanced; false if there are no more pairs.</returns>
		public bool MoveNext()
		{
			for (; _index < _hashMap._lastIndex; ++_index)
			{
				ref var slot = ref _hashMap._slots[_index];
				if (slot.Key - 1 < 0) continue;
				Current = _index;
				++_index;
				return true;
			}

			_index = _hashMap._lastIndex + 1;
			Current = 0;
			return false;
		}
	}
}

}
