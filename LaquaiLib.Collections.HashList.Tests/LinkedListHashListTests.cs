using Xunit;
using LaquaiLib.Collections;

namespace LaquaiLib.Collections.Tests;

/// <summary>
/// Tests for HashList created with optimizeForRemove=true (LinkedListHashList).
/// </summary>
public class LinkedListHashListTests
{
    private static HashList<T> Create<T>() => HashList.Create<T>(optimizeForRemove: true);

    #region Add
    [Fact]
    public void Add_UniqueItem_ReturnsTrue()
    {
        var list = Create<int>();
        Assert.True(list.Add(1));
    }

    [Fact]
    public void Add_DuplicateItem_ReturnsFalse()
    {
        var list = Create<int>();
        list.Add(1);
        Assert.False(list.Add(1));
    }

    [Fact]
    public void Add_MultipleUniqueItems_AllReturnTrue()
    {
        var list = Create<int>();
        Assert.True(list.Add(1));
        Assert.True(list.Add(2));
        Assert.True(list.Add(3));
        Assert.Equal(3, list.Count);
    }

    [Fact]
    public void Add_ViaICollectionInterface_Works()
    {
        ICollection<int> list = Create<int>();
        list.Add(1);
        list.Add(1);
        Assert.Equal(1, list.Count);
    }
    #endregion

    #region Remove
    [Fact]
    public void Remove_ExistingItem_ReturnsTrue()
    {
        var list = Create<int>();
        list.Add(1);
        Assert.True(list.Remove(1));
    }

    [Fact]
    public void Remove_NonExistingItem_ReturnsFalse()
    {
        var list = Create<int>();
        Assert.False(list.Remove(1));
    }

    [Fact]
    public void Remove_DecreasesCount()
    {
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        list.Remove(1);
        Assert.Equal(1, list.Count);
    }

    [Fact]
    public void Remove_ItemNoLongerContained()
    {
        var list = Create<int>();
        list.Add(1);
        list.Remove(1);
        Assert.False(list.Contains(1));
    }

    [Fact]
    public void Remove_ThenReAdd_Succeeds()
    {
        var list = Create<int>();
        list.Add(1);
        list.Remove(1);
        Assert.True(list.Add(1));
        Assert.Equal(1, list.Count);
    }

    [Fact]
    public void Remove_PreservesOrderOfRemainingElements()
    {
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        list.Remove(2);
        Assert.Equal(1, list[0]);
        Assert.Equal(3, list[1]);
    }

    [Fact]
    public void Remove_FirstItem_PreservesOrder()
    {
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        list.Remove(1);
        Assert.Equal(2, list[0]);
        Assert.Equal(3, list[1]);
    }

    [Fact]
    public void Remove_LastItem_PreservesOrder()
    {
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        list.Remove(3);
        Assert.Equal(1, list[0]);
        Assert.Equal(2, list[1]);
    }
    #endregion

    #region Contains
    [Fact]
    public void Contains_ExistingItem_ReturnsTrue()
    {
        var list = Create<int>();
        list.Add(42);
        Assert.True(list.Contains(42));
    }

    [Fact]
    public void Contains_NonExistingItem_ReturnsFalse()
    {
        var list = Create<int>();
        Assert.False(list.Contains(42));
    }

    [Fact]
    public void Contains_AfterClear_ReturnsFalse()
    {
        var list = Create<int>();
        list.Add(1);
        list.Clear();
        Assert.False(list.Contains(1));
    }
    #endregion

    #region Clear
    [Fact]
    public void Clear_EmptiesList()
    {
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        list.Clear();
        Assert.Equal(0, list.Count);
    }

    [Fact]
    public void Clear_AllowsReAddingItems()
    {
        var list = Create<int>();
        list.Add(1);
        list.Clear();
        Assert.True(list.Add(1));
    }
    #endregion

    #region Count
    [Fact]
    public void Count_EmptyList_ReturnsZero()
    {
        var list = Create<int>();
        Assert.Equal(0, list.Count);
    }

    [Fact]
    public void Count_DuplicateAdds_NotCounted()
    {
        var list = Create<int>();
        list.Add(1);
        list.Add(1);
        Assert.Equal(1, list.Count);
    }
    #endregion

    #region IsReadOnly
    [Fact]
    public void IsReadOnly_ReturnsFalse()
    {
        var list = Create<int>();
        Assert.False(list.IsReadOnly);
    }
    #endregion

    #region Indexer
    [Fact]
    public void Indexer_ReturnsCorrectItem()
    {
        var list = Create<int>();
        list.Add(10);
        list.Add(20);
        list.Add(30);
        Assert.Equal(10, list[0]);
        Assert.Equal(20, list[1]);
        Assert.Equal(30, list[2]);
    }

    [Fact]
    public void Indexer_PreservesInsertionOrder()
    {
        var list = Create<string>();
        list.Add("c");
        list.Add("a");
        list.Add("b");
        Assert.Equal("c", list[0]);
        Assert.Equal("a", list[1]);
        Assert.Equal("b", list[2]);
    }

    [Fact]
    public void Indexer_NegativeIndex_Throws()
    {
        var list = Create<int>();
        list.Add(1);
        Assert.Throws<ArgumentOutOfRangeException>(() => list[-1]);
    }

    [Fact]
    public void Indexer_IndexEqualToCount_Throws()
    {
        var list = Create<int>();
        list.Add(1);
        Assert.Throws<ArgumentOutOfRangeException>(() => list[1]);
    }

    [Fact]
    public void Indexer_EmptyList_Throws()
    {
        var list = Create<int>();
        Assert.Throws<ArgumentOutOfRangeException>(() => list[0]);
    }

    [Fact]
    public void Indexer_AccessFromBothEnds_Correct()
    {
        // This tests the LinkedListHashList optimization that searches from
        // the nearest end of the list based on the index.
        var list = Create<int>();
        for (var i = 0; i < 20; i++)
            list.Add(i);

        // Access near the start (should traverse from front)
        Assert.Equal(0, list[0]);
        Assert.Equal(1, list[1]);
        Assert.Equal(2, list[2]);

        // Access near the end (should traverse from back)
        Assert.Equal(19, list[19]);
        Assert.Equal(18, list[18]);
        Assert.Equal(17, list[17]);

        // Access near the middle
        Assert.Equal(10, list[10]);
        Assert.Equal(9, list[9]);
    }
    #endregion

    #region CopyTo
    [Fact]
    public void CopyTo_CopiesToArrayAtIndex()
    {
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        var array = new int[5];
        list.CopyTo(array, 1);
        Assert.Equal(new[] { 0, 1, 2, 3, 0 }, array);
    }

    [Fact]
    public void CopyTo_EmptyList_NoOp()
    {
        var list = Create<int>();
        var array = new int[3];
        list.CopyTo(array, 0);
        Assert.Equal(new[] { 0, 0, 0 }, array);
    }
    #endregion

    #region Enumeration
    [Fact]
    public void GetEnumerator_ReturnsItemsInInsertionOrder()
    {
        var list = Create<int>();
        list.Add(3);
        list.Add(1);
        list.Add(2);
        var items = new List<int>();
        foreach (var item in list)
            items.Add(item);
        Assert.Equal(new[] { 3, 1, 2 }, items);
    }

    [Fact]
    public void GetEnumerator_EmptyList_YieldsNothing()
    {
        var list = Create<int>();
        var items = new List<int>();
        foreach (var item in list)
            items.Add(item);
        Assert.Empty(items);
    }

    [Fact]
    public void IEnumerable_NonGeneric_Works()
    {
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        var enumerable = (System.Collections.IEnumerable)list;
        var count = 0;
        foreach (var item in enumerable)
            count++;
        Assert.Equal(2, count);
    }
    #endregion

    #region Custom EqualityComparer
    [Fact]
    public void CustomComparer_CaseInsensitive_TreatsDuplicates()
    {
        var list = HashList.Create<string>(StringComparer.OrdinalIgnoreCase, optimizeForRemove: true);
        list.Add("Hello");
        Assert.False(list.Add("hello"));
        Assert.Equal(1, list.Count);
    }

    [Fact]
    public void CustomComparer_Contains_UsesComparer()
    {
        var list = HashList.Create<string>(StringComparer.OrdinalIgnoreCase, optimizeForRemove: true);
        list.Add("Hello");
        Assert.True(list.Contains("hello"));
    }

    [Fact]
    public void CustomComparer_Remove_UsesComparer()
    {
        var list = HashList.Create<string>(StringComparer.OrdinalIgnoreCase, optimizeForRemove: true);
        list.Add("Hello");
        Assert.True(list.Remove("hello"));
        Assert.Equal(0, list.Count);
    }
    #endregion

    #region Large collection
    [Fact]
    public void LargeCollection_MaintainsInsertionOrder()
    {
        var list = Create<int>();
        for (var i = 0; i < 1000; i++)
            list.Add(i);
        Assert.Equal(1000, list.Count);
        for (var i = 0; i < 1000; i++)
            Assert.Equal(i, list[i]);
    }

    [Fact]
    public void LargeCollection_RemoveAndVerifyOrder()
    {
        var list = Create<int>();
        for (var i = 0; i < 100; i++)
            list.Add(i);

        // Remove every other element
        for (var i = 0; i < 100; i += 2)
            list.Remove(i);

        Assert.Equal(50, list.Count);
        for (var i = 0; i < 50; i++)
            Assert.Equal(i * 2 + 1, list[i]);
    }
    #endregion
}
