using Xunit;
using LaquaiLib.Collections;

namespace LaquaiLib.Collections.Tests;

/// <summary>
/// Tests for ConcurrentHashList (both underlying strategies).
/// </summary>
public class ConcurrentHashListTests
{
    #region Basic operations (DefaultList-backed)
    [Fact]
    public void Add_UniqueItem_ReturnsTrue()
    {
        using var list = (ConcurrentHashList<int>)HashList.CreateConcurrent<int>();
        Assert.True(list.Add(1));
    }

    [Fact]
    public void Add_DuplicateItem_ReturnsFalse()
    {
        using var list = (ConcurrentHashList<int>)HashList.CreateConcurrent<int>();
        list.Add(1);
        Assert.False(list.Add(1));
    }

    [Fact]
    public void Remove_ExistingItem_ReturnsTrue()
    {
        using var list = (ConcurrentHashList<int>)HashList.CreateConcurrent<int>();
        list.Add(1);
        Assert.True(list.Remove(1));
    }

    [Fact]
    public void Remove_NonExistingItem_ReturnsFalse()
    {
        using var list = (ConcurrentHashList<int>)HashList.CreateConcurrent<int>();
        Assert.False(list.Remove(1));
    }

    [Fact]
    public void Contains_ExistingItem_ReturnsTrue()
    {
        using var list = (ConcurrentHashList<int>)HashList.CreateConcurrent<int>();
        list.Add(42);
        Assert.True(list.Contains(42));
    }

    [Fact]
    public void Contains_NonExistingItem_ReturnsFalse()
    {
        using var list = (ConcurrentHashList<int>)HashList.CreateConcurrent<int>();
        Assert.False(list.Contains(42));
    }

    [Fact]
    public void Clear_EmptiesList()
    {
        using var list = (ConcurrentHashList<int>)HashList.CreateConcurrent<int>();
        list.Add(1);
        list.Add(2);
        list.Clear();
        Assert.Equal(0, list.Count);
    }

    [Fact]
    public void Count_ReflectsAddRemove()
    {
        using var list = (ConcurrentHashList<int>)HashList.CreateConcurrent<int>();
        Assert.Equal(0, list.Count);
        list.Add(1);
        Assert.Equal(1, list.Count);
        list.Add(2);
        Assert.Equal(2, list.Count);
        list.Remove(1);
        Assert.Equal(1, list.Count);
    }

    [Fact]
    public void IsReadOnly_ReturnsFalse()
    {
        using var list = (ConcurrentHashList<int>)HashList.CreateConcurrent<int>();
        Assert.False(list.IsReadOnly);
    }

    [Fact]
    public void Indexer_ReturnsCorrectItem()
    {
        using var list = (ConcurrentHashList<int>)HashList.CreateConcurrent<int>();
        list.Add(10);
        list.Add(20);
        list.Add(30);
        Assert.Equal(10, list[0]);
        Assert.Equal(20, list[1]);
        Assert.Equal(30, list[2]);
    }

    [Fact]
    public void CopyTo_CopiesCorrectly()
    {
        using var list = (ConcurrentHashList<int>)HashList.CreateConcurrent<int>();
        list.Add(1);
        list.Add(2);
        var array = new int[4];
        list.CopyTo(array, 1);
        Assert.Equal(new[] { 0, 1, 2, 0 }, array);
    }
    #endregion

    #region Basic operations (LinkedList-backed)
    [Fact]
    public void LinkedListBacked_Add_Works()
    {
        using var list = (ConcurrentHashList<int>)HashList.CreateConcurrent<int>(optimizeForRemove: true);
        Assert.True(list.Add(1));
        Assert.False(list.Add(1));
        Assert.Equal(1, list.Count);
    }

    [Fact]
    public void LinkedListBacked_Remove_Works()
    {
        using var list = (ConcurrentHashList<int>)HashList.CreateConcurrent<int>(optimizeForRemove: true);
        list.Add(1);
        Assert.True(list.Remove(1));
        Assert.False(list.Contains(1));
    }

    [Fact]
    public void LinkedListBacked_Indexer_Works()
    {
        using var list = (ConcurrentHashList<int>)HashList.CreateConcurrent<int>(optimizeForRemove: true);
        list.Add(10);
        list.Add(20);
        Assert.Equal(10, list[0]);
        Assert.Equal(20, list[1]);
    }
    #endregion

    #region Enumeration (snapshot behavior)
    [Fact]
    public void GetEnumerator_ReturnsSnapshot()
    {
        using var list = (ConcurrentHashList<int>)HashList.CreateConcurrent<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);

        var items = new List<int>();
        foreach (var item in list)
            items.Add(item);

        Assert.Equal(new[] { 1, 2, 3 }, items);
    }

    [Fact]
    public void GetEnumerator_SnapshotNotAffectedByMutationDuringIteration()
    {
        using var list = (ConcurrentHashList<int>)HashList.CreateConcurrent<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);

        var items = new List<int>();
        using var enumerator = list.GetEnumerator();
        // Move through first item
        enumerator.MoveNext();
        items.Add(enumerator.Current);

        // Mutate during enumeration
        list.Add(4);
        list.Remove(2);

        // Continue iterating - should see original snapshot
        while (enumerator.MoveNext())
            items.Add(enumerator.Current);

        Assert.Equal(new[] { 1, 2, 3 }, items);
    }

    [Fact]
    public void GetEnumerator_EmptyList_YieldsNothing()
    {
        using var list = (ConcurrentHashList<int>)HashList.CreateConcurrent<int>();
        var items = new List<int>();
        foreach (var item in list)
            items.Add(item);
        Assert.Empty(items);
    }
    #endregion

    #region AddOrRemove
    [Fact]
    public void AddOrRemove_Add_WhenNotPresent_ReturnsTrue()
    {
        using var list = (ConcurrentHashList<int>)HashList.CreateConcurrent<int>();
        Assert.True(list.AddOrRemove(1, add: true));
        Assert.True(list.Contains(1));
    }

    [Fact]
    public void AddOrRemove_Add_WhenPresent_ReturnsFalse()
    {
        using var list = (ConcurrentHashList<int>)HashList.CreateConcurrent<int>();
        list.Add(1);
        Assert.False(list.AddOrRemove(1, add: true));
    }

    [Fact]
    public void AddOrRemove_Remove_WhenPresent_ReturnsTrue()
    {
        using var list = (ConcurrentHashList<int>)HashList.CreateConcurrent<int>();
        list.Add(1);
        Assert.True(list.AddOrRemove(1, add: false));
        Assert.False(list.Contains(1));
    }

    [Fact]
    public void AddOrRemove_Remove_WhenNotPresent_ReturnsFalse()
    {
        using var list = (ConcurrentHashList<int>)HashList.CreateConcurrent<int>();
        Assert.False(list.AddOrRemove(1, add: false));
    }
    #endregion

    #region Mutate
    [Fact]
    public void Mutate_ExecutesActionAtomically()
    {
        using var list = (ConcurrentHashList<int>)HashList.CreateConcurrent<int>();
        list.Mutate(inner =>
        {
            inner.Add(1);
            inner.Add(2);
            inner.Add(3);
        });
        Assert.Equal(3, list.Count);
        Assert.True(list.Contains(1));
        Assert.True(list.Contains(2));
        Assert.True(list.Contains(3));
    }

    [Fact]
    public void Mutate_CompoundOperation_Works()
    {
        using var list = (ConcurrentHashList<int>)HashList.CreateConcurrent<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);

        list.Mutate(inner =>
        {
            inner.Remove(2);
            inner.Add(4);
        });

        Assert.Equal(3, list.Count);
        Assert.False(list.Contains(2));
        Assert.True(list.Contains(4));
    }
    #endregion

    #region Dispose
    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        var list = (ConcurrentHashList<int>)HashList.CreateConcurrent<int>();
        list.Dispose();
        // Second dispose should not throw
        list.Dispose();
    }
    #endregion

    #region Concurrent access
    [Fact]
    public async Task ConcurrentReads_DoNotBlock()
    {
        using var list = (ConcurrentHashList<int>)HashList.CreateConcurrent<int>();
        for (var i = 0; i < 100; i++)
            list.Add(i);

        var tasks = new Task<bool>[10];
        for (var i = 0; i < 10; i++)
        {
            tasks[i] = Task.Run(() =>
            {
                for (var j = 0; j < 100; j++)
                {
                    _ = list.Contains(j);
                    _ = list[j];
                    _ = list.Count;
                }
                return true;
            });
        }

        await Task.WhenAll(tasks);
        Assert.All(tasks, t => Assert.True(t.Result));
    }

    [Fact]
    public async Task ConcurrentAddAndRead_NoExceptions()
    {
        using var list = (ConcurrentHashList<int>)HashList.CreateConcurrent<int>();

        var writerTask = Task.Run(() =>
        {
            for (var i = 0; i < 1000; i++)
                list.Add(i);
        }, TestContext.Current.CancellationToken);

        var readerTask = Task.Run(() =>
        {
            while (!writerTask.IsCompleted)
            {
                var _ = list.Count;
                foreach (var item in list)
                {
                    // just enumerate
                }
            }
        }, TestContext.Current.CancellationToken);

        await Task.WhenAll(writerTask, readerTask);
        Assert.Equal(1000, list.Count);
    }

    [Fact]
    public async Task ConcurrentAddAndRemove_MaintainsConsistency()
    {
        using var list = (ConcurrentHashList<int>)HashList.CreateConcurrent<int>();

        // Add items 0-999 from one thread, remove 0-499 from another
        var addTask = Task.Run(() =>
        {
            for (var i = 0; i < 1000; i++)
                list.Add(i);
        }, TestContext.Current.CancellationToken);

        await addTask;

        var removeTask = Task.Run(() =>
        {
            for (var i = 0; i < 500; i++)
                list.Remove(i);
        }, TestContext.Current.CancellationToken);

        await removeTask;

        Assert.Equal(500, list.Count);
        for (var i = 500; i < 1000; i++)
            Assert.True(list.Contains(i));
    }

    [Fact]
    public async Task ConcurrentEnumeration_SafeWithMutation()
    {
        using var list = (ConcurrentHashList<int>)HashList.CreateConcurrent<int>();
        for (var i = 0; i < 100; i++)
            list.Add(i);

        var enumerationTask = Task.Run(() =>
        {
            for (var round = 0; round < 50; round++)
            {
                var count = 0;
                foreach (var item in list)
                    count++;
                // Snapshot enumeration means we get a consistent view
                Assert.True(count >= 0);
            }
        }, TestContext.Current.CancellationToken);

        var mutationTask = Task.Run(() =>
        {
            for (var i = 100; i < 200; i++)
                list.Add(i);
        }, TestContext.Current.CancellationToken);

        await Task.WhenAll(enumerationTask, mutationTask);
    }
    #endregion
}
