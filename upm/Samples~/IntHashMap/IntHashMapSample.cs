using UnityEngine;

namespace Moroshka.Collections.Samples
{

/// <summary>
/// Simple example of using IntHashMap API.
/// Demonstrates basic operations: adding, retrieving, removing and iterating over a hash table.
/// </summary>
public class IntHashMapSample : MonoBehaviour
{
	private IntHashMap<string> _itemMap;

	private void Start()
	{
		// Creating IntHashMap with initial capacity
		_itemMap = new IntHashMap<string>(capacity: 5);
		Debug.Log($"IntHashMap created with capacity: {_itemMap.Length}");

		// Adding elements
		var added1 = _itemMap.Add(1, "Sword", out var slot1);
		var added2 = _itemMap.Add(2, "Shield", out var slot2);
		var added3 = _itemMap.Add(3, "Potion", out var slot3);

		Debug.Log($"Elements added: {added1} (slot {slot1}), {added2} (slot {slot2}), {added3} (slot {slot3})");
		Debug.Log($"Total elements: {_itemMap.Length}");

		// Checking key existence
		var hasKey2 = _itemMap.Has(2);
		var hasKey999 = _itemMap.Has(999);
		Debug.Log($"Key 2 exists: {hasKey2}, key 999 exists: {hasKey999}");

		// Retrieving values
		if (_itemMap.TryGetValue(1, out var item1))
		{
			Debug.Log($"Element with key 1: {item1}");
		}

		if (_itemMap.TryGetValue(2, out var item2))
		{
			Debug.Log($"Element with key 2: {item2}");
		}

		// Getting index by key
		var index1 = _itemMap.TryGetIndex(1);
		var index2 = _itemMap.TryGetIndex(2);
		Debug.Log($"Indices: key 1 -> {index1}, key 2 -> {index2}");

		// Retrieving by index
		if (index1 >= 0)
		{
			var key = _itemMap.GetKeyByIndex(index1);
			var value = _itemMap.GetValueByIndex(index1);
			Debug.Log($"By index {index1}: key {key} -> {value}");
		}

		// Iterating over all elements
		foreach (var index in _itemMap)
		{
			Debug.Log($"ID {_itemMap.GetKeyByIndex(index)}: {_itemMap.GetValueByIndex(index)}");
		}

		// Updating value (Set)
		var updated = _itemMap.Set(1, "Enhanced Sword", out var updatedSlot);
		Debug.Log($"Element updated: {updated}, new slot: {updatedSlot}");

		// Retrieving updated value
		if (_itemMap.TryGetValue(1, out var updatedItem))
		{
			Debug.Log($"Updated element: {updatedItem}");
		}

		// Adding new element via Set
		var addedNew = _itemMap.Set(4, "Ring", out var newSlot);
		Debug.Log($"New element added: {addedNew}, slot: {newSlot}");

		// Removing element
		var removed = _itemMap.Remove(2, out var removedItem);
		Debug.Log($"Element removed: {removed}, removed element: {removedItem}");
		Debug.Log($"Size after removal: {_itemMap.Length}");

		// Clearing hash table
		_itemMap.Clear();
		Debug.Log($"Hash table cleared, size: {_itemMap.Length}");
	}

	private void OnDestroy()
	{
		// Clearing resources
		_itemMap?.Clear();
	}
}

}
