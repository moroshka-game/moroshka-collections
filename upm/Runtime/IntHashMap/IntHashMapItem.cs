namespace Moroshka.Collections
{

/// <summary>
/// Represents an item in the hash map's internal slot array, containing a key and a reference to the next item in the collision chain.
/// </summary>
public struct IntHashMapItem
{
	/// <summary>
	/// The key value stored in this slot. The actual key is (Key - 1) to distinguish between empty slots (Key = 0) and valid keys.
	/// </summary>
	public int Key;

	/// <summary>
	/// The index of the next item in the collision chain, or -1 if this is the last item in the chain.
	/// </summary>
	public int Next;
}

}
