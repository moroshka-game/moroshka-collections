using System;
using Moroshka.Protect;
using Moroshka.Xcp;

namespace Moroshka.Collections
{

/// <summary>
/// A specialized hash map implementation that uses integers as keys and stores values of type T.
/// </summary>
/// <typeparam name="T">The type of values stored in the hash map.</typeparam>
public sealed partial class IntHashMap<T>
{
	private readonly ICapacityStrategy _capacityStrategy;
	private int _capacity;
	private int _capacityMinusOne;
	private int _lastIndex;
	private int _freeIndex;
	private int[] _buckets;
	private T[] _data;
	private IntHashMapItem[] _slots;

	/// <summary>
	/// Initializes a new instance of the <see cref="IntHashMap{T}"/> class with the specified capacity and capacity strategy.
	/// </summary>
	/// <param name="capacity">The initial capacity of the hash map. Must be non-negative.</param>
	/// <param name="capacityStrategy">The strategy to determine capacity growth. If null, a default strategy is used.</param>
	/// <exception cref="RequireException">Thrown when the capacity is negative.</exception>
	public IntHashMap(int capacity = 0, ICapacityStrategy capacityStrategy = null)
	{
		this.Require(capacity, nameof(capacity), Is.InRange(0, int.MaxValue));
		_capacityStrategy = capacityStrategy ?? new CapacityStrategy();
		_lastIndex = 0;
		Length = 0;
		_freeIndex = -1;
		_capacityMinusOne = _capacityStrategy.CalculateCapacity(_capacity - 1, 0);
		_capacity = _capacityMinusOne + 1;
		_buckets = new int[_capacity];
		_slots = new IntHashMapItem[_capacity];
		_data = new T[_capacity];
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="IntHashMap{T}"/> class by copying another instance.
	/// </summary>
	/// <param name="other">The <see cref="IntHashMap{T}"/> to copy from.</param>
	public IntHashMap(IntHashMap<T> other)
	{
		_lastIndex = other._lastIndex;
		Length = other.Length;
		_freeIndex = other._freeIndex;
		_capacityMinusOne = other._capacityMinusOne;
		_capacity = other._capacity;
		_buckets = new int[_capacity];
		_slots = new IntHashMapItem[_capacity];
		_data = new T[_capacity];

		for (int i = 0, len = _capacity; i < len; i++)
		{
			_buckets[i] = other._buckets[i];
			_slots[i] = other._slots[i];
			_data[i] = other._data[i];
		}
	}

	/// <summary>
	/// Gets the number of key-value pairs in the hash map.
	/// </summary>
	public int Length { get; private set; }

	/// <summary>
	/// Returns an enumerator that iterates through the indices of valid key-value pairs in the hash map.
	/// </summary>
	/// <returns>An <see cref="Enumerator"/> for the hash map.</returns>
	public Enumerator GetEnumerator() => new(this);

	/// <summary>
	/// Adds a key-value pair to the hash map if the key does not already exist.
	/// </summary>
	/// <param name="key">The integer key to add.</param>
	/// <param name="value">The value to associate with the key.</param>
	/// <param name="slotIndex">The index of the slot where the key-value pair was added, or -1 if the key already exists.</param>
	/// <returns>True if the key-value pair was added; false if the key already exists.</returns>
	public bool Add(in int key, in T value, out int slotIndex)
	{
		var rem = key & _capacityMinusOne;

		for (var i = _buckets[rem] - 1; i >= 0; i = _slots[i].Next)
		{
			if (_slots[i].Key - 1 != key) continue;
			slotIndex = -1;
			return false;
		}

		if (_freeIndex >= 0)
		{
			slotIndex = _freeIndex;
			_freeIndex = _slots[slotIndex].Next;
		}
		else
		{
			if (_lastIndex == _capacity)
			{
				Expand();
				rem = key & _capacityMinusOne;
			}

			slotIndex = _lastIndex;
			++_lastIndex;
		}

		ref var newSlot = ref _slots[slotIndex];
		newSlot.Key = key + 1;
		newSlot.Next = _buckets[rem] - 1;
		_data[slotIndex] = value;
		_buckets[rem] = slotIndex + 1;
		++Length;
		return true;
	}

	/// <summary>
	/// Sets the value for a key, adding the key-value pair if the key does not exist or updating the value if it does.
	/// </summary>
	/// <param name="key">The integer key to set.</param>
	/// <param name="value">The value to associate with the key.</param>
	/// <param name="slotIndex">The index of the slot where the key-value pair was added or updated.</param>
	/// <returns>True if the key-value pair was added; false if the key already existed and the value was updated.</returns>
	public bool Set(in int key, in T value, out int slotIndex)
	{
		var rem = key & _capacityMinusOne;

		for (var i = _buckets[rem] - 1; i >= 0; i = _slots[i].Next)
		{
			if (_slots[i].Key - 1 != key) continue;
			_data[i] = value;
			slotIndex = i;
			return false;
		}

		if (_freeIndex >= 0)
		{
			slotIndex = _freeIndex;
			_freeIndex = _slots[slotIndex].Next;
		}
		else
		{
			if (_lastIndex == _capacity)
			{
				Expand();
				rem = key & _capacityMinusOne;
			}

			slotIndex = _lastIndex;
			++_lastIndex;
		}

		ref var newSlot = ref _slots[slotIndex];
		newSlot.Key = key + 1;
		newSlot.Next = _buckets[rem] - 1;
		_data[slotIndex] = value;
		_buckets[rem] = slotIndex + 1;
		++Length;
		return true;
	}

	/// <summary>
	/// Removes a key-value pair from the hash map.
	/// </summary>
	/// <param name="key">The integer key to remove.</param>
	/// <param name="lastValue">The value associated with the key, or the default value if the key was not found.</param>
	/// <returns>True if the key-value pair was removed; false if the key was not found.</returns>
	public bool Remove(in int key, out T lastValue)
	{
		var rem = key & _capacityMinusOne;
		int next;
		var num = -1;

		for (var i = _buckets[rem] - 1; i >= 0; i = next)
		{
			ref var slot = ref _slots[i];

			if (slot.Key - 1 == key)
			{
				if (num < 0) _buckets[rem] = slot.Next + 1;
				else _slots[num].Next = slot.Next;
				lastValue = _data[i];
				_data[i] = default;
				slot.Key = -1;
				slot.Next = _freeIndex;
				--Length;

				if (Length == 0)
				{
					_lastIndex = 0;
					_freeIndex = -1;
				}
				else
				{
					_freeIndex = i;
				}

				return true;
			}

			next = slot.Next;
			num = i;
		}

		lastValue = default;
		return false;
	}

	/// <summary>
	/// Checks if a key exists in the hash map.
	/// </summary>
	/// <param name="key">The integer key to check.</param>
	/// <returns>True if the key exists; false otherwise.</returns>
	public bool Has(in int key)
	{
		var rem = key & _capacityMinusOne;
		int next;

		for (var i = _buckets[rem] - 1; i >= 0; i = next)
		{
			ref var slot = ref _slots[i];
			if (slot.Key - 1 == key) return true;
			next = slot.Next;
		}

		return false;
	}

	/// <summary>
	/// Attempts to get the value associated with a key.
	/// </summary>
	/// <param name="key">The integer key to look up.</param>
	/// <param name="value">The value associated with the key, or the default value if the key was not found.</param>
	/// <returns>True if the key was found; false otherwise.</returns>
	public bool TryGetValue(in int key, out T value)
	{
		var rem = key & _capacityMinusOne;
		int next;

		for (var i = _buckets[rem] - 1; i >= 0; i = next)
		{
			ref var slot = ref _slots[i];

			if (slot.Key - 1 == key)
			{
				value = _data[i];
				return true;
			}

			next = slot.Next;
		}

		value = default;
		return false;
	}

	/// <summary>
	/// Gets the value associated with a key, throwing an exception if the key is not found.
	/// </summary>
	/// <param name="key">The integer key to look up.</param>
	/// <returns>The value associated with the key.</returns>
	/// <exception cref="InvOpException">Thrown when the key is not found.</exception>
	public T GetValueByKey(in int key)
	{
		var rem = key & _capacityMinusOne;
		int next;

		for (var i = _buckets[rem] - 1; i >= 0; i = next)
		{
			ref var slot = ref _slots[i];
			if (slot.Key - 1 == key) return _data[i];
			next = slot.Next;
		}

		throw new InvOpException($"Key {key} not found");
	}

	/// <summary>
	/// Gets a reference to the value associated with a key, indicating whether the key exists.
	/// </summary>
	/// <param name="key">The integer key to look up.</param>
	/// <param name="exist">True if the key was found; false otherwise.</param>
	/// <returns>A reference to the value associated with the key, or a reference to a default value if the key was not found.</returns>
	public ref T TryGetValueRefByKey(in int key, out bool exist)
	{
		var rem = key & _capacityMinusOne;
		int next;

		for (var i = _buckets[rem] - 1; i >= 0; i = next)
		{
			ref var slot = ref _slots[i];

			if (slot.Key - 1 == key)
			{
				exist = true;
				return ref _data[i];
			}

			next = slot.Next;
		}

		exist = false;
		return ref _data[0];
	}

	/// <summary>
	/// Gets a reference to the value associated with a key, throwing an exception if the key is not found.
	/// </summary>
	/// <param name="key">The integer key to look up.</param>
	/// <returns>A reference to the value associated with the key.</returns>
	/// <exception cref="InvOpException">Thrown when the key is not found.</exception>
	public ref T GetValueRefByKey(in int key)
	{
		var rem = key & _capacityMinusOne;
		int next;

		for (var i = _buckets[rem] - 1; i >= 0; i = next)
		{
			ref var slot = ref _slots[i];
			if (slot.Key - 1 == key)
			{
				return ref _data[i];
			}

			next = slot.Next;
		}

		throw new InvOpException($"Key {key} not found");
	}

	/// <summary>
	/// Gets the value at the specified index.
	/// </summary>
	/// <param name="index">The index of the value to retrieve.</param>
	/// <returns>The value at the specified index.</returns>
	public T GetValueByIndex(in int index)
	{
		return _data[index];
	}

	/// <summary>
	/// Gets a reference to the value at the specified index.
	/// </summary>
	/// <param name="index">The index of the value to retrieve.</param>
	/// <returns>A reference to the value at the specified index.</returns>
	public ref T GetValueRefByIndex(in int index)
	{
		return ref _data[index];
	}

	/// <summary>
	/// Gets the key at the specified index.
	/// </summary>
	/// <param name="index">The index of the key to retrieve.</param>
	/// <returns>The key at the specified index.</returns>
	public int GetKeyByIndex(in int index)
	{
		return _slots[index].Key - 1;
	}

	/// <summary>
	/// Attempts to get the index of a key in the hash map.
	/// </summary>
	/// <param name="key">The integer key to look up.</param>
	/// <returns>The index of the key if found; otherwise, -1.</returns>
	public int TryGetIndex(in int key)
	{
		var rem = key & _capacityMinusOne;
		int next;

		for (var i = _buckets[rem] - 1; i >= 0; i = next)
		{
			ref var slot = ref _slots[i];
			if (slot.Key - 1 == key) return i;
			next = slot.Next;
		}

		return -1;
	}

	/// <summary>
	/// Copies the values of the hash map to an array.
	/// </summary>
	/// <param name="array">The target array to copy values to.</param>
	public void CopyTo(T[] array)
	{
		var num = 0;

		for (int i = 0, li = _lastIndex; i < li && num < Length; ++i)
		{
			if (_slots[i].Key - 1 < 0) continue;
			array[num] = _data[i];
			++num;
		}
	}

	/// <summary>
	/// Copies the contents of another hash map into this one.
	/// </summary>
	/// <param name="from">The source <see cref="IntHashMap{T}"/> to copy from.</param>
	public void CopyFrom(IntHashMap<T> from)
	{
		_lastIndex = from._lastIndex;
		Length = from.Length;
		_freeIndex = from._freeIndex;
		var needResize = _capacity < from._capacity;
		_capacityMinusOne = from._capacityMinusOne;
		_capacity = from._capacity;

		if (needResize)
		{
			_buckets = new int[_capacity];
			_slots = new IntHashMapItem[_capacity];
			_data = new T[_capacity];
		}

		for (int i = 0, len = _capacity; i < len; i++)
		{
			_buckets[i] = from._buckets[i];
			_slots[i] = from._slots[i];
			_data[i] = from._data[i];
		}
	}

	/// <summary>
	/// Clears all key-value pairs from the hash map.
	/// </summary>
	public void Clear()
	{
		if (_lastIndex <= 0) return;
		Array.Clear(_slots, 0, _lastIndex);
		Array.Clear(_buckets, 0, _capacity);
		Array.Clear(_data, 0, _lastIndex);
		_lastIndex = 0;
		Length = 0;
		_freeIndex = -1;
	}

	private void Expand()
	{
		var newCapacityMinusOne = _capacityStrategy.CalculateCapacity(Length, Length);
		var newCapacity = newCapacityMinusOne + 1;
		ArrayGrow(ref _slots, newCapacity);
		ArrayGrow(ref _data, newCapacity);
		var newBuckets = new int[newCapacity];

		for (int i = 0, len = _lastIndex; i < len; ++i)
		{
			ref var slot = ref _slots[i];
			var newResizeIndex = (slot.Key - 1) & newCapacityMinusOne;
			slot.Next = newBuckets[newResizeIndex] - 1;
			newBuckets[newResizeIndex] = i + 1;
		}

		_buckets = newBuckets;
		_capacity = newCapacity;
		_capacityMinusOne = newCapacityMinusOne;
	}

	private static void ArrayGrow<T1>(ref T1[] array, int newSize)
	{
		var newArray = new T1[newSize];
		Array.Copy(array, 0, newArray, 0, array.Length);
		array = newArray;
	}

}

}
