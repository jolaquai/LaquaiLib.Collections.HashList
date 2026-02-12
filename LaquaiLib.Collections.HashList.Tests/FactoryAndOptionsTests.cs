using Xunit;

using LaquaiLib.Collections;

namespace LaquaiLib.Collections.Tests;

/// <summary>
/// Tests for HashList factory methods and HashListOptions.
/// </summary>
public class FactoryAndOptionsTests
{
    #region Non-concurrent factory methods
    [Fact]
    public void Create_Default_ReturnsNonConcurrent()
    {
        var list = HashList.Create<int>();
        Assert.NotNull(list);
        Assert.IsNotType<ConcurrentHashList<int>>(list);
    }

    [Fact]
    public void Create_WithCapacity_ReturnsFunctionalList()
    {
        var list = HashList.Create<int>(16);
        list.Add(1);
        Assert.Equal(1, list.Count);
    }

    [Fact]
    public void Create_WithComparer_UsesComparer()
    {
        var list = HashList.Create<string>(StringComparer.OrdinalIgnoreCase);
        list.Add("Test");
        Assert.True(list.Contains("test"));
    }

    [Fact]
    public void Create_WithCapacityAndComparer_Works()
    {
        var list = HashList.Create<string>(16, StringComparer.OrdinalIgnoreCase);
        list.Add("Test");
        Assert.True(list.Contains("test"));
    }

    [Fact]
    public void Create_OptimizeForRemoveFalse_CreatesDefaultList()
    {
        var list = HashList.Create<int>(optimizeForRemove: false);
        list.Add(1);
        list.Add(2);
        Assert.Equal(1, list[0]);
    }

    [Fact]
    public void Create_OptimizeForRemoveTrue_CreatesLinkedList()
    {
        var list = HashList.Create<int>(optimizeForRemove: true);
        list.Add(1);
        list.Add(2);
        Assert.Equal(1, list[0]);
    }

    [Fact]
    public void Create_CapacityAndOptimizeForRemove_Works()
    {
        var list = HashList.Create<int>(32, optimizeForRemove: true);
        list.Add(1);
        Assert.Equal(1, list.Count);
    }

    [Fact]
    public void Create_ComparerAndOptimizeForRemove_Works()
    {
        var list = HashList.Create<string>(StringComparer.OrdinalIgnoreCase, optimizeForRemove: true);
        list.Add("Test");
        Assert.True(list.Contains("TEST"));
    }

    [Fact]
    public void Create_AllParams_Works()
    {
        var list = HashList.Create<string>(16, StringComparer.OrdinalIgnoreCase, optimizeForRemove: true);
        list.Add("Test");
        Assert.True(list.Contains("TEST"));
        Assert.Equal(1, list.Count);
    }

    [Fact]
    public void Create_WithOptions_Works()
    {
        var options = new HashListOptions<string>
        {
            Capacity = 32,
            EqualityComparer = StringComparer.OrdinalIgnoreCase,
            OptimizeForRemove = true
        };
        var list = HashList.Create(options);
        list.Add("Test");
        Assert.True(list.Contains("TEST"));
    }

    [Fact]
    public void Create_WithNullOptions_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => HashList.Create<int>((HashListOptions<int>)null));
    }
    #endregion

    #region Concurrent factory methods
    [Fact]
    public void CreateConcurrent_Default_ReturnsConcurrent()
    {
        var list = HashList.CreateConcurrent<int>();
        Assert.IsType<ConcurrentHashList<int>>(list);
    }

    [Fact]
    public void CreateConcurrent_WithCapacity_ReturnsFunctionalList()
    {
        var list = HashList.CreateConcurrent<int>(16);
        Assert.IsType<ConcurrentHashList<int>>(list);
        list.Add(1);
        Assert.Equal(1, list.Count);
    }

    [Fact]
    public void CreateConcurrent_WithComparer_UsesComparer()
    {
        var list = HashList.CreateConcurrent<string>(StringComparer.OrdinalIgnoreCase);
        Assert.IsType<ConcurrentHashList<string>>(list);
        list.Add("Test");
        Assert.True(list.Contains("test"));
    }

    [Fact]
    public void CreateConcurrent_WithCapacityAndComparer_Works()
    {
        var list = HashList.CreateConcurrent<string>(16, StringComparer.OrdinalIgnoreCase);
        Assert.IsType<ConcurrentHashList<string>>(list);
        list.Add("Test");
        Assert.True(list.Contains("test"));
    }

    [Fact]
    public void CreateConcurrent_OptimizeForRemoveFalse_Works()
    {
        var list = HashList.CreateConcurrent<int>(optimizeForRemove: false);
        Assert.IsType<ConcurrentHashList<int>>(list);
        list.Add(1);
        Assert.Equal(1, list[0]);
    }

    [Fact]
    public void CreateConcurrent_OptimizeForRemoveTrue_Works()
    {
        var list = HashList.CreateConcurrent<int>(optimizeForRemove: true);
        Assert.IsType<ConcurrentHashList<int>>(list);
        list.Add(1);
        Assert.Equal(1, list[0]);
    }

    [Fact]
    public void CreateConcurrent_CapacityAndOptimizeForRemove_Works()
    {
        var list = HashList.CreateConcurrent<int>(32, optimizeForRemove: true);
        Assert.IsType<ConcurrentHashList<int>>(list);
    }

    [Fact]
    public void CreateConcurrent_ComparerAndOptimizeForRemove_Works()
    {
        var list = HashList.CreateConcurrent<string>(StringComparer.OrdinalIgnoreCase, optimizeForRemove: true);
        Assert.IsType<ConcurrentHashList<string>>(list);
        list.Add("Test");
        Assert.True(list.Contains("TEST"));
    }

    [Fact]
    public void CreateConcurrent_AllParams_Works()
    {
        var list = HashList.CreateConcurrent<string>(16, StringComparer.OrdinalIgnoreCase, optimizeForRemove: true);
        Assert.IsType<ConcurrentHashList<string>>(list);
        list.Add("Test");
        Assert.True(list.Contains("TEST"));
    }

    [Fact]
    public void CreateConcurrent_WithOptions_Works()
    {
        var options = new HashListOptions<string>
        {
            Capacity = 32,
            EqualityComparer = StringComparer.OrdinalIgnoreCase,
            OptimizeForRemove = true
        };
        var list = HashList.CreateConcurrent(options);
        Assert.IsType<ConcurrentHashList<string>>(list);
        list.Add("Test");
        Assert.True(list.Contains("TEST"));
    }

    [Fact]
    public void CreateConcurrent_WithNullOptions_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => HashList.CreateConcurrent<int>((HashListOptions<int>)null));
    }
    #endregion

    #region HashListOptions defaults
    [Fact]
    public void Options_DefaultEqualityComparer_IsDefault()
    {
        var options = new HashListOptions<int>();
        Assert.Equal(EqualityComparer<int>.Default, options.EqualityComparer);
    }

    [Fact]
    public void Options_DefaultOptimizeForRemove_IsFalse()
    {
        var options = new HashListOptions<int>();
        Assert.False(options.OptimizeForRemove);
    }

    [Fact]
    public void Options_CanSetAllProperties()
    {
        var comparer = StringComparer.OrdinalIgnoreCase;
        var options = new HashListOptions<string>
        {
            Capacity = 100,
            EqualityComparer = comparer,
            OptimizeForRemove = true
        };
        Assert.Equal(100, options.Capacity);
        Assert.Same(comparer, options.EqualityComparer);
        Assert.True(options.OptimizeForRemove);
    }
    #endregion

    #region Both strategies produce equivalent results
    [Fact]
    public void BothStrategies_SameAddRemoveBehavior()
    {
        var defaultList = HashList.Create<int>(optimizeForRemove: false);
        var linkedList = HashList.Create<int>(optimizeForRemove: true);

        for (var i = 0; i < 50; i++)
        {
            Assert.Equal(defaultList.Add(i), linkedList.Add(i));
        }

        // Duplicate adds
        for (var i = 0; i < 50; i++)
        {
            Assert.Equal(defaultList.Add(i), linkedList.Add(i));
        }

        Assert.Equal(defaultList.Count, linkedList.Count);

        // Remove some items
        for (var i = 0; i < 25; i++)
        {
            Assert.Equal(defaultList.Remove(i), linkedList.Remove(i));
        }

        Assert.Equal(defaultList.Count, linkedList.Count);

        // Check contents and order
        for (var i = 0; i < defaultList.Count; i++)
        {
            Assert.Equal(defaultList[i], linkedList[i]);
        }
    }

    [Fact]
    public void BothStrategies_SameContainsBehavior()
    {
        var defaultList = HashList.Create<int>(optimizeForRemove: false);
        var linkedList = HashList.Create<int>(optimizeForRemove: true);

        for (var i = 0; i < 50; i++)
        {
            defaultList.Add(i);
            linkedList.Add(i);
        }

        for (var i = -10; i < 60; i++)
        {
            Assert.Equal(defaultList.Contains(i), linkedList.Contains(i));
        }
    }

    [Fact]
    public void BothStrategies_SameCopyToBehavior()
    {
        var defaultList = HashList.Create<int>(optimizeForRemove: false);
        var linkedList = HashList.Create<int>(optimizeForRemove: true);

        for (var i = 0; i < 10; i++)
        {
            defaultList.Add(i);
            linkedList.Add(i);
        }

        var defaultArray = new int[10];
        var linkedArray = new int[10];
        defaultList.CopyTo(defaultArray, 0);
        linkedList.CopyTo(linkedArray, 0);

        Assert.Equal(defaultArray, linkedArray);
    }

    [Fact]
    public void BothStrategies_SameEnumerationOrder()
    {
        var defaultList = HashList.Create<int>(optimizeForRemove: false);
        var linkedList = HashList.Create<int>(optimizeForRemove: true);

        for (var i = 0; i < 50; i++)
        {
            defaultList.Add(i);
            linkedList.Add(i);
        }

        var defaultItems = new List<int>();
        var linkedItems = new List<int>();
        foreach (var item in defaultList)
            defaultItems.Add(item);
        foreach (var item in linkedList)
            linkedItems.Add(item);

        Assert.Equal(defaultItems, linkedItems);
    }
    #endregion

    #region Options default capacity matches library behavior
    [Fact]
    public void Options_DefaultCapacity_MatchesLibraryDefault()
    {
        var options = new HashListOptions<int>();
        Assert.Equal(HashList.DefaultCapacity, options.Capacity);
    }
    #endregion

    #region Null comparer fallback
    [Fact]
    public void Create_NullComparer_FallsBackToDefault()
    {
        // Passing null comparer should not throw and should use default
        var list = HashList.Create<int>(capacity: 4, comparer: null);
        list.Add(1);
        Assert.True(list.Contains(1));
    }

    [Fact]
    public void Create_NullComparer_OptimizeForRemove_FallsBackToDefault()
    {
        var list = HashList.Create<int>(capacity: 4, comparer: null, optimizeForRemove: true);
        list.Add(1);
        Assert.True(list.Contains(1));
    }
    #endregion
}
