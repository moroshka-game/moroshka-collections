# Инструкции по использованию

## Содержание

- [Обзор](#обзор)
- [Основные коллекции](#основные-коллекции)
  - [FastList](#fastlist)
  - [IntHashMap](#inthashmap)
- [Управление емкостью](#управление-емкостью)
- [Обработка ошибок](#обработка-ошибок)
- [Производительность](#производительность)
- [Интеграция с Unity](#интеграция-с-unity)
- [Заключение](#заключение)

## Обзор

Библиотека `Moroshka.Collections` предоставляет высокопроизводительные коллекции, оптимизированные для игровых приложений и других сценариев, где важна предсказуемая производительность и управление памятью.

## Основные коллекции

### FastList

`FastList<T>` - это высокопроизводительная альтернатива стандартному `List<T>`, которая использует пулинг массивов для эффективного управления памятью.

#### Когда использовать FastList

- Когда нужна высокая производительность добавления элементов
- При работе с временными коллекциями, которые часто создаются и уничтожаются
- В игровых циклах, где важна предсказуемая производительность

#### Примеры использования

```csharp
// Создание списка с начальной емкостью
var fastList = new FastList<int>(capacity: 100);

// Добавление элементов
fastList.Add(1);
fastList.Add(2);
fastList.Add(3);

// Доступ к элементам
int first = fastList[0]; // 1
int count = fastList.Count; // 3

// Итерация по элементам
foreach (var item in fastList)
{
    Console.WriteLine(item);
}

// Не забудьте освободить ресурсы
fastList.Dispose();
```

#### Лучшие практики

- Всегда вызывайте `Dispose()` после использования
- Используйте `using` для автоматического освобождения ресурсов
- Устанавливайте разумную начальную емкость для избежания перераспределения памяти

```csharp
using (var fastList = new FastList<GameObject>(capacity: 50))
{
    // Работа со списком
    foreach (var obj in gameObjects)
    {
        if (obj.IsActive)
            fastList.Add(obj);
    }

    // fastList автоматически освободится
}
```

### IntHashMap

`IntHashMap<T>` - специализированная хеш-таблица, использующая целочисленные ключи. Оптимизирована для случаев, когда ключи являются индексами или ID.

#### Когда использовать IntHashMap

- При работе с индексами массивов как ключами
- Для кэширования данных по ID объектов
- Когда нужна быстрая вставка и поиск по целочисленным ключам

#### Примеры использования

```csharp
// Создание хеш-таблицы
var entityMap = new IntHashMap<Entity>(capacity: 1000);

// Добавление элементов
entityMap.Add(entityId: 1, entity: playerEntity, out int slotIndex);
entityMap.Add(entityId: 2, entity: enemyEntity, out slotIndex);

// Проверка существования ключа
if (entityMap.Has(1))
{
    Console.WriteLine("Игрок найден");
}

// Получение значения
if (entityMap.TryGetValue(1, out Entity player))
{
    player.Update();
}

// Удаление элемента
if (entityMap.Remove(2, out Entity removedEntity))
{
    removedEntity.Destroy();
}

// Итерация по всем элементам
foreach (int key in entityMap)
{
    if (entityMap.TryGetValue(key, out Entity entity))
    {
        entity.Render();
    }
}
```

#### Лучшие практики

- Используйте `TryGetValue` вместо прямого доступа для избежания исключений
- Проверяйте результат `Add` для определения, был ли элемент добавлен
- Используйте `Has` для быстрой проверки существования ключа

```csharp
// Кэширование компонентов по ID сущности
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

## Управление емкостью

### CapacityStrategy

По умолчанию все коллекции используют `CapacityStrategy` для оптимального управления размером внутренних массивов. Стратегия выбирает размеры, близкие к степеням двойки минус один (2^n - 1), что минимизирует коллизии хеширования.

#### Кастомная стратегия емкости

```csharp
public class CustomCapacityStrategy : ICapacityStrategy
{
    public int CalculateCapacity(int currentCapacity, int requiredSize)
    {
        // Ваша логика расчета емкости
        return Math.Max(currentCapacity * 2, requiredSize);
    }
}

// Использование кастомной стратегии
var customList = new FastList<int>(capacity: 10, capacityStrategy: new CustomCapacityStrategy());
```

## Обработка ошибок

### OutOfCapacityException

При превышении максимальной емкости коллекции выбрасывается `OutOfCapacityException`:

```csharp
try
{
    var largeList = new FastList<int>(capacity: 1);
    // Попытка добавить слишком много элементов
}
catch (OutOfCapacityException ex)
{
    Console.WriteLine($"Требуемая емкость: {ex.RequiredSize}");
    Console.WriteLine($"Текущая емкость: {ex.CurrentCapacity}");
}
```

## Производительность

### Рекомендации по производительности

1. **Инициализация с правильной емкостью**: Устанавливайте начальную емкость близко к ожидаемому размеру коллекции
2. **Переиспользование коллекций**: Создавайте коллекции один раз и переиспользуйте их
3. **Использование using**: Всегда освобождайте ресурсы с помощью `Dispose()` или `using`
4. **Выбор правильной коллекции**: Используйте `IntHashMap` для целочисленных ключей, `FastList` для последовательных данных

### Сравнение производительности

```csharp
// Медленно - частые перераспределения
var slowList = new FastList<int>(capacity: 1);
for (int i = 0; i < 10000; i++)
{
    slowList.Add(i);
}

// Быстро - правильная начальная емкость
var fastList = new FastList<int>(capacity: 10000);
for (int i = 0; i < 10000; i++)
{
    fastList.Add(i);
}
```

## Интеграция с Unity

### Использование в MonoBehaviour

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
        // IntHashMap не требует Dispose
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

## Заключение

Библиотека `Moroshka.Collections` предоставляет оптимизированные коллекции для высокопроизводительных приложений. Правильное использование этих коллекций поможет улучшить производительность вашего приложения и обеспечить предсказуемое управление памятью.
