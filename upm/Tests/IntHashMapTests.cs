using System.Collections.Generic;
using Moroshka.Xcp;
using NUnit.Framework;

namespace Moroshka.Collections.Tests
{

[TestFixture]
internal sealed class IntHashMapTests
{
	[Test]
	public void Constructor_CreatesEmptyHashMap()
	{
		// Act
		var hashMap = new IntHashMap<int>();

		// Assert
		Assert.That(hashMap.Length, Is.EqualTo(0));
	}

	[Test]
	public void Add_AddsNewKey_IncreasesLength()
	{
		// Arrange
		var hashMap = new IntHashMap<int>();

		// Act
		var addResult = hashMap.Add(1, 100, out _);
		var getResult = hashMap.TryGetValue(1, out var value);

		// Assert
		Assert.That(addResult, Is.True);
		Assert.That(hashMap.Length, Is.EqualTo(1));
		Assert.That(getResult, Is.True);
		Assert.That(value, Is.EqualTo(100));
	}

	[Test]
	public void Add_DuplicateKey_ReturnsFalse()
	{
		// Arrange
		var hashMap = new IntHashMap<int>();
		hashMap.Add(1, 100, out _);

		// Act
		var addResult = hashMap.Add(1, 200, out _);
		var getResult = hashMap.TryGetValue(1, out var value);

		// Assert
		Assert.That(addResult, Is.False);
		Assert.That(hashMap.Length, Is.EqualTo(1));
		Assert.That(getResult, Is.True);
		Assert.That(value, Is.EqualTo(100));
	}

	[Test]
	public void Set_AddsNewKey_IncreasesLength()
	{
		// Arrange
		var hashMap = new IntHashMap<int>();

		// Act
		var setResult = hashMap.Set(2, 200, out _);
		var getResult = hashMap.TryGetValue(2, out var value);

		// Assert
		Assert.That(setResult, Is.True);
		Assert.That(hashMap.Length, Is.EqualTo(1));
		Assert.That(getResult, Is.True);
		Assert.That(value, Is.EqualTo(200));
	}

	[Test]
	public void Set_UpdatesExistingKey_DoesNotIncreaseLength()
	{
		// Arrange
		var hashMap = new IntHashMap<int>();
		hashMap.Set(2, 200, out _);

		// Act
		var setResult = hashMap.Set(2, 300, out _);
		var getResult = hashMap.TryGetValue(2, out var value);

		// Assert
		Assert.That(setResult, Is.False);
		Assert.That(hashMap.Length, Is.EqualTo(1));
		Assert.That(getResult, Is.True);
		Assert.That(value, Is.EqualTo(300));
	}

	[Test]
	public void Remove_ExistingKey_DecreasesLength()
	{
		// Arrange
		var hashMap = new IntHashMap<int>();
		hashMap.Add(3, 300, out _);

		// Act
		var removeResult = hashMap.Remove(3, out var removedValue);
		var hasResult = hashMap.Has(3);

		// Assert
		Assert.That(removeResult, Is.True);
		Assert.That(removedValue, Is.EqualTo(300));
		Assert.That(hashMap.Length, Is.EqualTo(0));
		Assert.That(hasResult, Is.False);
	}

	[Test]
	public void Remove_NonExistingKey_ReturnsFalse()
	{
		// Arrange
		var hashMap = new IntHashMap<int>();

		// Act
		var result = hashMap.Remove(4, out var removedValue);

		// Assert
		Assert.That(result, Is.False);
		Assert.That(removedValue, Is.EqualTo(0));
		Assert.That(hashMap.Length, Is.EqualTo(0));
	}

	[Test]
	public void Has_ExistingKey_ReturnsTrue()
	{
		// Arrange
		var hashMap = new IntHashMap<int>();
		hashMap.Add(5, 500, out _);

		// Act
		var exists = hashMap.Has(5);

		// Assert
		Assert.That(exists, Is.True);
	}

	[Test]
	public void Has_NonExistingKey_ReturnsFalse()
	{
		// Arrange
		var hashMap = new IntHashMap<int>();

		// Act
		var exists = hashMap.Has(6);

		// Assert
		Assert.That(exists, Is.False);
	}

	[Test]
	public void TryGetValue_ExistingKey_ReturnsTrueAndValue()
	{
		// Arrange
		var hashMap = new IntHashMap<int>();
		hashMap.Add(7, 700, out _);

		// Act
		var result = hashMap.TryGetValue(7, out var value);

		// Assert
		Assert.That(result, Is.True);
		Assert.That(value, Is.EqualTo(700));
	}

	[Test]
	public void TryGetValue_NonExistingKey_ReturnsFalse()
	{
		// Arrange
		var hashMap = new IntHashMap<int>();

		// Act
		var result = hashMap.TryGetValue(8, out var value);

		// Assert
		Assert.That(result, Is.False);
		Assert.That(value, Is.EqualTo(0));
	}

	[Test]
	public void GetValueByKey_ExistingKey_ReturnsValue()
	{
		// Arrange
		var hashMap = new IntHashMap<int>();
		hashMap.Add(9, 900, out _);

		// Act
		var value = hashMap.GetValueByKey(9);

		// Assert
		Assert.That(value, Is.EqualTo(900));
	}

	[Test]
	public void GetValueByKey_NonExistingKey_ThrowsException()
	{
		// Arrange
		var hashMap = new IntHashMap<int>();

		// Act & Assert
		Assert.Throws<InvOpException>(() => hashMap.GetValueByKey(10));
	}

	[Test]
	public void Clear_RemovesAllElements()
	{
		// Arrange
		var hashMap = new IntHashMap<int>();
		hashMap.Add(11, 1100, out _);
		hashMap.Add(12, 1200, out _);

		// Act
		hashMap.Clear();
		var hasResult = hashMap.Has(11);
		var hasResultToo = hashMap.Has(12);

		// Assert
		Assert.That(hashMap.Length, Is.EqualTo(0));
		Assert.That(hasResult, Is.False);
		Assert.That(hasResultToo, Is.False);
	}

	[Test]
	public void Enumerator_IteratesThroughAllElements()
	{
		// Arrange
		var hashMap = new IntHashMap<int>();
		hashMap.Add(13, 1300, out _);
		hashMap.Add(14, 1400, out _);
		hashMap.Add(15, 1500, out _);
		var expectedKeys = new[] { 13, 14, 15 };
		var actualKeys = new List<int>();

		// Act
		foreach (var index in hashMap)
		{
			var key = hashMap.GetKeyByIndex(index);
			actualKeys.Add(key);
		}

		// Assert
		Assert.That(actualKeys, Is.EquivalentTo(expectedKeys));
	}

	[Test]
	public void CopyTo_CopiesElementsCorrectly()
	{
		// Arrange
		var hashMap = new IntHashMap<int>();
		hashMap.Add(16, 1600, out _);
		hashMap.Add(17, 1700, out _);
		var array = new int[2];

		// Act
		hashMap.CopyTo(array);

		// Assert
		Assert.That(new[] { 1600, 1700 }, Is.EquivalentTo(array));
	}

	[Test]
	public void CopyFrom_CopiesFromAnotherHashMap()
	{
		// Arrange
		var source = new IntHashMap<int>();
		source.Add(18, 1800, out _);
		source.Add(19, 1900, out _);
		var destination = new IntHashMap<int>();

		// Act
		destination.CopyFrom(source);
		var getResult = destination.TryGetValue(18, out var value18);
		var getResultToo = destination.TryGetValue(19, out var value19);

		// Assert
		Assert.That(destination.Length, Is.EqualTo(2));
		Assert.That(getResult, Is.True);
		Assert.That(getResultToo, Is.True);
		Assert.That(value18, Is.EqualTo(1800));
		Assert.That(value19, Is.EqualTo(1900));
	}

	[Test]
	public void CopyConstructor_CreatesIdenticalHashMap()
	{
		// Arrange
		var original = new IntHashMap<int>();
		original.Add(20, 2000, out _);
		original.Add(21, 2100, out _);

		// Act
		var copy = new IntHashMap<int>(original);
		var getResult20 = copy.TryGetValue(20, out var value20);
		var getResult21 = copy.TryGetValue(21, out var value21);

		// Assert
		Assert.That(copy.Length, Is.EqualTo(2));
		Assert.That(getResult20, Is.True);
		Assert.That(value20, Is.EqualTo(2000));
		Assert.That(getResult21, Is.True);
		Assert.That(value21, Is.EqualTo(2100));
	}

	[Test]
	public void TryGetValueRefByKey_ExistingKey_ReturnsReferenceAndTrue()
	{
		// Arrange
		var hashMap = new IntHashMap<int>();
		hashMap.Add(22, 2200, out _);

		// Act
		ref var valueRef = ref hashMap.TryGetValueRefByKey(22, out var exists);

		// Assert
		Assert.That(exists, Is.True);
		Assert.That(valueRef, Is.EqualTo(2200));

		// Modify the value through the reference
		valueRef = 2201;
		Assert.That(hashMap.GetValueByKey(22), Is.EqualTo(2201));
	}

	[Test]
	public void TryGetValueRefByKey_NonExistingKey_ReturnsFalse()
	{
		// Arrange
		var hashMap = new IntHashMap<int>();

		// Act
		ref var valueRef = ref hashMap.TryGetValueRefByKey(23, out var exists);

		// Assert
		Assert.That(exists, Is.False);
		Assert.That(valueRef, Is.EqualTo(0)); // Default value for int
	}

	[Test]
	public void GetValueRefByKey_ExistingKey_ReturnsReference()
	{
		// Arrange
		var hashMap = new IntHashMap<int>();
		hashMap.Add(24, 2400, out _);

		// Act
		ref var valueRef = ref hashMap.GetValueRefByKey(24);

		// Assert
		Assert.That(valueRef, Is.EqualTo(2400));

		// Modify the value through the reference
		valueRef = 2401;
		Assert.That(hashMap.GetValueByKey(24), Is.EqualTo(2401));
	}

	[Test]
	public void GetValueRefByKey_NonExistingKey_ThrowsException()
	{
		// Arrange
		var hashMap = new IntHashMap<int>();

		// Act & Assert
		Assert.Throws<InvOpException>(() => hashMap.GetValueRefByKey(25));
	}

	[Test]
	public void GetValueByIndex_ReturnsCorrectValue()
	{
		// Arrange
		var hashMap = new IntHashMap<int>();
		hashMap.Add(26, 2600, out var slotIndex);

		// Act
		var value = hashMap.GetValueByIndex(slotIndex);

		// Assert
		Assert.That(value, Is.EqualTo(2600));
	}

	[Test]
	public void GetValueRefByIndex_ReturnsReference()
	{
		// Arrange
		var hashMap = new IntHashMap<int>();
		hashMap.Add(27, 2700, out var slotIndex);

		// Act
		ref var valueRef = ref hashMap.GetValueRefByIndex(slotIndex);

		// Assert
		Assert.That(valueRef, Is.EqualTo(2700));

		// Modify the value through the reference
		valueRef = 2701;
		Assert.That(hashMap.GetValueByIndex(slotIndex), Is.EqualTo(2701));
	}

	[Test]
	public void TryGetIndex_ExistingKey_ReturnsIndex()
	{
		// Arrange
		var hashMap = new IntHashMap<int>();
		hashMap.Add(28, 2800, out var expectedIndex);

		// Act
		var index = hashMap.TryGetIndex(28);

		// Assert
		Assert.That(index, Is.EqualTo(expectedIndex));
	}

	[Test]
	public void TryGetIndex_NonExistingKey_ReturnsMinusOne()
	{
		// Arrange
		var hashMap = new IntHashMap<int>();

		// Act
		var index = hashMap.TryGetIndex(29);

		// Assert
		Assert.That(index, Is.EqualTo(-1));
	}

	[Test]
	public void Length_AfterMultipleOperations_ReflectsCorrectCount()
	{
		// Arrange
		var hashMap = new IntHashMap<int>();

		// Act
		hashMap.Add(30, 3000, out _); // Length = 1
		hashMap.Add(31, 3100, out _); // Length = 2
		hashMap.Set(30, 3001, out _); // Length = 2
		hashMap.Remove(31, out _); // Length = 1
		hashMap.Clear(); // Length = 0

		// Assert
		Assert.That(hashMap.Length, Is.EqualTo(0));
	}

	[Test]
	public void Enumerator_EmptyHashMap_DoesNotMoveNext()
	{
		// Arrange
		var hashMap = new IntHashMap<int>();
		var enumerator = hashMap.GetEnumerator();

		// Act
		var moved = enumerator.MoveNext();

		// Assert
		Assert.That(moved, Is.False);
		Assert.That(enumerator.Current, Is.EqualTo(0));
	}

	[Test]
	public void Enumerator_SingleElement_IteratesCorrectly()
	{
		// Arrange
		var hashMap = new IntHashMap<int>();
		hashMap.Add(32, 3200, out var expectedIndex);
		var enumerator = hashMap.GetEnumerator();

		// Act
		var moved = enumerator.MoveNext();
		var current = enumerator.Current;
		var movedAgain = enumerator.MoveNext();

		// Assert
		Assert.That(moved, Is.True);
		Assert.That(current, Is.EqualTo(expectedIndex));
		Assert.That(hashMap.GetValueByIndex(current), Is.EqualTo(3200));
		Assert.That(movedAgain, Is.False);
		Assert.That(enumerator.Current, Is.EqualTo(0));
	}

	[Test]
	public void Enumerator_SparseHashMap_SkipsFreeSlots()
	{
		// Arrange
		var hashMap = new IntHashMap<int>();
		hashMap.Add(33, 3300, out _);
		hashMap.Add(34, 3400, out _);
		hashMap.Add(35, 3500, out _);
		hashMap.Remove(34, out _); // Creates a free slot
		var actualKeys = new List<int>();

		// Act
		foreach (var index in hashMap)
		{
			var key = hashMap.GetKeyByIndex(index);
			actualKeys.Add(key);
		}

		// Assert
		var expectedKeys = new[] { 33, 35 };
		Assert.That(actualKeys, Is.EquivalentTo(expectedKeys));
		Assert.That(hashMap.Length, Is.EqualTo(2));
	}

	[Test]
	public void Enumerator_CurrentBeforeMoveNext_ReturnsZero()
	{
		// Arrange
		var hashMap = new IntHashMap<int>();
		hashMap.Add(36, 3600, out _);
		var enumerator = hashMap.GetEnumerator();

		// Act
		var current = enumerator.Current;

		// Assert
		Assert.That(current, Is.EqualTo(0));
	}

	[Test]
	public void Enumerator_MultipleIterations_ProducesConsistentResults()
	{
		// Arrange
		var hashMap = new IntHashMap<int>();
		hashMap.Add(37, 3700, out _);
		hashMap.Add(38, 3800, out _);
		var firstIterationKeys = new List<int>();
		var secondIterationKeys = new List<int>();

		// Act
		// First iteration
		foreach (var index in hashMap)
		{
			firstIterationKeys.Add(hashMap.GetKeyByIndex(index));
		}

		// Second iteration
		foreach (var index in hashMap)
		{
			secondIterationKeys.Add(hashMap.GetKeyByIndex(index));
		}

		// Assert
		var expectedKeys = new[] { 37, 38 };
		Assert.That(firstIterationKeys, Is.EquivalentTo(expectedKeys));
		Assert.That(secondIterationKeys, Is.EquivalentTo(expectedKeys));
	}

	[Test]
	public void Add_WhenCapacityExceeded_ExpandsHashMap()
	{
		// Arrange
		var hashMap = new IntHashMap<int>(capacity: 2); // Small initial capacity
		hashMap.Add(1, 100, out _);
		hashMap.Add(2, 200, out _);

		// Act
		var addResult = hashMap.Add(3, 300, out _);

		// Assert
		Assert.That(addResult, Is.True);
		Assert.That(hashMap.Length, Is.EqualTo(3));
		Assert.That(hashMap.TryGetValue(3, out var value), Is.True);
		Assert.That(value, Is.EqualTo(300));
	}

	[Test]
	public void Set_WhenCapacityExceeded_ExpandsHashMap()
	{
		// Arrange
		var hashMap = new IntHashMap<int>(capacity: 2); // Small initial capacity
		hashMap.Set(1, 100, out _);
		hashMap.Set(2, 200, out _);

		// Act
		var setResult = hashMap.Set(3, 300, out _);

		// Assert
		Assert.That(setResult, Is.True);
		Assert.That(hashMap.Length, Is.EqualTo(3));
		Assert.That(hashMap.TryGetValue(3, out var value), Is.True);
		Assert.That(value, Is.EqualTo(300));
	}

	[Test]
	public void Add_ReusesFreeSlots_AfterRemoval()
	{
		// Arrange
		var hashMap = new IntHashMap<int>();
		hashMap.Add(1, 100, out _);
		hashMap.Add(2, 200, out _);
		hashMap.Add(3, 300, out _);
		hashMap.Remove(2, out _);

		// Act
		var addResult = hashMap.Add(4, 400, out _);

		// Assert
		Assert.That(addResult, Is.True);
		Assert.That(hashMap.Length, Is.EqualTo(3));
		Assert.That(hashMap.TryGetValue(4, out var value), Is.True);
		Assert.That(value, Is.EqualTo(400));
	}

	[Test]
	public void Set_ReusesFreeSlots_AfterRemoval()
	{
		// Arrange
		var hashMap = new IntHashMap<int>();
		hashMap.Set(1, 100, out _);
		hashMap.Set(2, 200, out _);
		hashMap.Set(3, 300, out _);
		hashMap.Remove(2, out _);

		// Act
		var setResult = hashMap.Set(4, 400, out _);

		// Assert
		Assert.That(setResult, Is.True);
		Assert.That(hashMap.Length, Is.EqualTo(3));
		Assert.That(hashMap.TryGetValue(4, out var value), Is.True);
		Assert.That(value, Is.EqualTo(400));
	}

	[Test]
	public void Remove_FromMiddleOfChain_UpdatesLinksCorrectly()
	{
		// Arrange
		var hashMap = new IntHashMap<int>(capacity: 4);
		hashMap.Add(1, 100, out _);
		hashMap.Add(5, 500, out _);
		hashMap.Add(9, 900, out _);

		// Act
		var removeResult = hashMap.Remove(5, out var removedValue);

		// Assert
		Assert.That(removeResult, Is.True);
		Assert.That(removedValue, Is.EqualTo(500));
		Assert.That(hashMap.Length, Is.EqualTo(2));
		Assert.That(hashMap.Has(1), Is.True);
		Assert.That(hashMap.Has(5), Is.False);
		Assert.That(hashMap.Has(9), Is.True);
	}

	[Test]
	public void Has_WhenKeyNotFoundInChain_ReturnsFalse()
	{
		// Arrange
		var hashMap = new IntHashMap<int>(capacity: 4);
		hashMap.Add(1, 100, out _);
		hashMap.Add(5, 500, out _);

		// Act
		var exists = hashMap.Has(9);

		// Assert
		Assert.That(exists, Is.False);
	}

	[Test]
	public void TryGetValue_WhenKeyNotFoundInChain_ReturnsFalse()
	{
		// Arrange
		var hashMap = new IntHashMap<int>(capacity: 4);
		hashMap.Add(1, 100, out _);
		hashMap.Add(5, 500, out _);

		// Act
		var result = hashMap.TryGetValue(9, out var value);

		// Assert
		Assert.That(result, Is.False);
		Assert.That(value, Is.EqualTo(0));
	}

	[Test]
	public void GetValueByKey_WhenKeyNotFoundInChain_ThrowsException()
	{
		// Arrange
		var hashMap = new IntHashMap<int>(capacity: 4);
		hashMap.Add(1, 100, out _);
		hashMap.Add(5, 500, out _);

		// Act & Assert
		Assert.Throws<InvOpException>(() => hashMap.GetValueByKey(9));
	}

	[Test]
	public void TryGetValueRefByKey_WhenKeyNotFoundInChain_ReturnsFalse()
	{
		// Arrange
		var hashMap = new IntHashMap<int>(capacity: 4);
		hashMap.Add(1, 100, out _);
		hashMap.Add(5, 500, out _);

		// Act
		ref var valueRef = ref hashMap.TryGetValueRefByKey(9, out var exists);

		// Assert
		Assert.That(exists, Is.False);
		Assert.That(valueRef, Is.EqualTo(100));
	}

	[Test]
	public void GetValueRefByKey_WhenKeyNotFoundInChain_ThrowsException()
	{
		// Arrange
		var hashMap = new IntHashMap<int>(capacity: 4);
		hashMap.Add(1, 100, out _);
		hashMap.Add(5, 500, out _);

		// Act & Assert
		Assert.Throws<InvOpException>(() => hashMap.GetValueRefByKey(9));
	}

	[Test]
	public void TryGetIndex_WhenKeyNotFoundInChain_ReturnsMinusOne()
	{
		// Arrange
		var hashMap = new IntHashMap<int>(capacity: 4);
		hashMap.Add(1, 100, out _);
		hashMap.Add(5, 500, out _);

		// Act
		var index = hashMap.TryGetIndex(9);

		// Assert
		Assert.That(index, Is.EqualTo(-1));
	}

	[Test]
	public void Remove_WhenKeyNotFoundInChain_ReturnsFalse()
	{
		// Arrange
		var hashMap = new IntHashMap<int>(capacity: 4);
		hashMap.Add(1, 100, out _);
		hashMap.Add(5, 500, out _);

		// Act
		var result = hashMap.Remove(9, out var removedValue);

		// Assert
		Assert.That(result, Is.False);
		Assert.That(removedValue, Is.EqualTo(0));
		Assert.That(hashMap.Length, Is.EqualTo(2));
	}

	[Test]
	public void Add_WhenCapacityExceeded_CallsExpandMethod()
	{
		// Arrange
		var hashMap = new IntHashMap<int>(capacity: 1);
		hashMap.Add(1, 100, out _);

		// Act
		var addResult = hashMap.Add(2, 200, out var slotIndex);

		// Assert
		Assert.That(addResult, Is.True);
		Assert.That(slotIndex, Is.GreaterThanOrEqualTo(0));
		Assert.That(hashMap.Length, Is.EqualTo(2));
		Assert.That(hashMap.TryGetValue(1, out var value1), Is.True);
		Assert.That(hashMap.TryGetValue(2, out var value2), Is.True);
		Assert.That(value1, Is.EqualTo(100));
		Assert.That(value2, Is.EqualTo(200));
	}

	[Test]
	public void Set_WhenCapacityExceeded_CallsExpandMethod()
	{
		// Arrange
		var hashMap = new IntHashMap<int>(capacity: 1);
		hashMap.Set(1, 100, out _);

		// Act
		var setResult = hashMap.Set(2, 200, out var slotIndex);

		// Assert
		Assert.That(setResult, Is.True);
		Assert.That(slotIndex, Is.GreaterThanOrEqualTo(0));
		Assert.That(hashMap.Length, Is.EqualTo(2));
		Assert.That(hashMap.TryGetValue(1, out var value1), Is.True);
		Assert.That(hashMap.TryGetValue(2, out var value2), Is.True);
		Assert.That(value1, Is.EqualTo(100));
		Assert.That(value2, Is.EqualTo(200));
	}

	[Test]
	public void CopyFrom_WhenTargetCapacitySmaller_ResizesArrays()
	{
		// Arrange
		var source = new IntHashMap<int>(capacity: 8);
		source.Add(1, 100, out _);
		source.Add(2, 200, out _);
		source.Add(3, 300, out _);
		source.Add(4, 400, out _);
		source.Add(5, 500, out _);

		// Arrange
		var destination = new IntHashMap<int>(capacity: 2);

		// Act
		destination.CopyFrom(source);

		// Assert
		Assert.That(destination.Length, Is.EqualTo(5));
		Assert.That(destination.TryGetValue(1, out var value1), Is.True);
		Assert.That(destination.TryGetValue(2, out var value2), Is.True);
		Assert.That(destination.TryGetValue(3, out var value3), Is.True);
		Assert.That(destination.TryGetValue(4, out var value4), Is.True);
		Assert.That(destination.TryGetValue(5, out var value5), Is.True);
		Assert.That(value1, Is.EqualTo(100));
		Assert.That(value2, Is.EqualTo(200));
		Assert.That(value3, Is.EqualTo(300));
		Assert.That(value4, Is.EqualTo(400));
		Assert.That(value5, Is.EqualTo(500));
	}

	[Test]
	public void Add_WithMultipleExpansions_HandlesGrowthCorrectly()
	{
		// Arrange
		var hashMap = new IntHashMap<int>(capacity: 1);

		// Act
		for (var i = 1; i <= 10; i++)
		{
			var addResult = hashMap.Add(i, i * 100, out _);
			Assert.That(addResult, Is.True, $"Failed to add element {i}");
		}

		// Assert
		Assert.That(hashMap.Length, Is.EqualTo(10));
		for (var i = 1; i <= 10; i++)
		{
			Assert.That(hashMap.TryGetValue(i, out var value), Is.True, $"Key {i} not found");
			Assert.That(value, Is.EqualTo(i * 100), $"Value for key {i} is incorrect");
		}
	}

	[Test]
	public void Set_WithMultipleExpansions_HandlesGrowthCorrectly()
	{
		// Arrange
		var hashMap = new IntHashMap<int>(capacity: 1);

		// Act
		for (var i = 1; i <= 10; i++)
		{
			var setResult = hashMap.Set(i, i * 200, out _);
			Assert.That(setResult, Is.True, $"Failed to set element {i}");
		}

		// Assert
		Assert.That(hashMap.Length, Is.EqualTo(10));
		for (var i = 1; i <= 10; i++)
		{
			Assert.That(hashMap.TryGetValue(i, out var value), Is.True, $"Key {i} not found");
			Assert.That(value, Is.EqualTo(i * 200), $"Value for key {i} is incorrect");
		}
	}

	[Test]
	public void CopyFrom_WithLargeCapacityDifference_HandlesResizeCorrectly()
	{
		// Arrange
		var source = new IntHashMap<int>(capacity: 16);
		for (var i = 1; i <= 10; i++)
		{
			source.Add(i, i * 1000, out _);
		}

		// Arrange
		var destination = new IntHashMap<int>(capacity: 1);

		// Act
		destination.CopyFrom(source);

		// Assert
		Assert.That(destination.Length, Is.EqualTo(10));
		for (var i = 1; i <= 10; i++)
		{
			Assert.That(destination.TryGetValue(i, out var value), Is.True, $"Key {i} not found after copy");
			Assert.That(value, Is.EqualTo(i * 1000), $"Value for key {i} is incorrect after copy");
		}
	}
}

}
