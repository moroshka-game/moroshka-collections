# Usage Instructions

## Table of Contents

- [Overview](#overview)
- [Main Collections](#main-collections)
  - [FastList](#fastlist)
  - [IntHashMap](#inthashmap)
- [Capacity Management](#capacity-management)
- [Error Handling](#error-handling)
- [Performance](#performance)
- [Unity Integration](#unity-integration)
- [Conclusion](#conclusion)

## Overview

The `Moroshka.Collections` library provides high-performance collections optimized for game applications and other scenarios where predictable performance and memory management are crucial.

## Main Collections

### FastList

`FastList<T>` is a high-performance alternative to the standard `List<T>` that uses array pooling for efficient memory management.

#### When to use FastList

- When you need high performance for adding elements
- When working with temporary collections that are frequently created and destroyed
- In game loops where predictable performance is important

#### Usage Examples

```csharp
// Creating a list with initial capacity
var fastList = new FastList<int>(capacity: 100);

// Adding elements
fastList.Add(1);
fastList.Add(2);
fastList.Add(3);

// Accessing elements
int first = fastList[0]; // 1
int count = fastList.Count; // 3

// Iterating through elements
foreach (var item in fastList)
{
    Console.WriteLine(item);
}

// Don't forget to release resources
fastList.Dispose();
```

#### Best Practices

- Always call `Dispose()` after use
- Use `using` for automatic resource disposal
- Set a reasonable initial capacity to avoid memory reallocation

```csharp
using (var fastList = new FastList<GameObject>(capacity: 50))
{
    // Working with the list
    foreach (var obj in gameObjects)
    {
        if (obj.IsActive)
            fastList.Add(obj);
    }

    // fastList will be automatically disposed
}
```

### IntHashMap

`IntHashMap<T>` is a specialized hash table that uses integer keys. Optimized for cases where keys are indices or IDs.

#### When to use IntHashMap

- When working with array indices as keys
- For caching data by object IDs
- When you need fast insertion and lookup by integer keys

#### Usage Examples

```csharp
// Creating a hash table
var entityMap = new IntHashMap<Entity>(capacity: 1000);

// Adding elements
entityMap.Add(entityId: 1, entity: playerEntity, out int slotIndex);
entityMap.Add(entityId: 2, entity: enemyEntity, out slotIndex);

// Checking if key exists
if (entityMap.Has(1))
{
    Console.WriteLine("Player found");
}

// Getting value
if (entityMap.TryGetValue(1, out Entity player))
{
    player.Update();
}

// Removing element
if (entityMap.Remove(2, out Entity removedEntity))
{
    removedEntity.Destroy();
}

// Iterating through all elements
foreach (int key in entityMap)
{
    if (entityMap.TryGetValue(key, out Entity entity))
    {
        entity.Render();
    }
}
```

#### Best Practices

- Use `TryGetValue` instead of direct access to avoid exceptions
- Check the result of `Add` to determine if the element was added
- Use `Has` for quick key existence checks

```csharp
// Caching components by entity ID
var componentCache = new IntHashMap<Component>();

public Component GetComponent(int entityId, Type componentType)
{
    if (componentCache.TryGetValue(entityId, out Component cached))
    {
        return cached;
    }

    var component = CreateComponent(entityId, componentType);
    componentCache.Add(entityId, component, out _);
    return component;
}
```

## Capacity Management

### CapacityStrategy

By default, all collections use `CapacityStrategy` for optimal management of internal array sizes. The strategy selects sizes close to powers of two minus one (2^n - 1), which minimizes hash collisions.

#### Custom Capacity Strategy

```csharp
public class CustomCapacityStrategy : ICapacityStrategy
{
    public int CalculateCapacity(int currentCapacity, int requiredSize)
    {
        // Your capacity calculation logic
        return Math.Max(currentCapacity * 2, requiredSize);
    }
}

// Using custom strategy
var customList = new FastList<int>(capacity: 10, capacityStrategy: new CustomCapacityStrategy());
```

## Error Handling

### OutOfCapacityException

When the maximum capacity is exceeded, an `OutOfCapacityException` is thrown:

```csharp
try
{
    var largeList = new FastList<int>(capacity: 1);
    // Attempting to add too many elements
}
catch (OutOfCapacityException ex)
{
    Console.WriteLine($"Required capacity: {ex.RequiredSize}");
    Console.WriteLine($"Current capacity: {ex.CurrentCapacity}");
}
```

## Performance

### Performance Recommendations

1. **Initialize with correct capacity**: Set initial capacity close to the expected collection size
2. **Reuse collections**: Create collections once and reuse them
3. **Use using statements**: Always release resources with `Dispose()` or `using`
4. **Choose the right collection**: Use `IntHashMap` for integer keys, `FastList` for sequential data

### Performance Comparison

```csharp
// Slow - frequent reallocations
var slowList = new FastList<int>(capacity: 1);
for (int i = 0; i < 10000; i++)
{
    slowList.Add(i);
}

// Fast - proper initial capacity
var fastList = new FastList<int>(capacity: 10000);
for (int i = 0; i < 10000; i++)
{
    fastList.Add(i);
}
```

## Unity Integration

### Using in MonoBehaviour

```csharp
public class GameManager : MonoBehaviour
{
    private FastList<Enemy> _enemies;
    private IntHashMap<Player> _players;

    private void Awake()
    {
        _enemies = new FastList<Enemy>(capacity: 100);
        _players = new IntHashMap<Player>(capacity: 10);
    }

    private void OnDestroy()
    {
        _enemies?.Dispose();
        // IntHashMap doesn't require Dispose
    }

    public void SpawnEnemy(Enemy enemy)
    {
        _enemies.Add(enemy);
    }

    public void RegisterPlayer(int playerId, Player player)
    {
        _players.Add(playerId, player, out _);
    }
}
```

## Conclusion

The `Moroshka.Collections` library provides optimized collections for high-performance applications. Proper use of these collections will help improve your application's performance and ensure predictable memory management.
