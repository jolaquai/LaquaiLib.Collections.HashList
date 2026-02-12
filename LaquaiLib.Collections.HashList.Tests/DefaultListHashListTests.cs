using Xunit;
using LaquaiLib.Collections;

namespace LaquaiLib.Collections.Tests;

/// <summary>
/// Tests for HashList created with optimizeForRemove=false (DefaultListHashList).
/// </summary>
public class DefaultListHashListTests
{
    private static HashList<T> Create<T>() => HashList.Create<T>();

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
    public void Add_NullItem_WorksForReferenceType()
    {
        var list = Create<string>();
        Assert.True(list.Add(null));
        Assert.False(list.Add(null));
        Assert.Equal(1, list.Count);
    }

    [Fact]
    public void Add_ViaICollectionInterface_Works()
    {
        ICollection<int> list = Create<int>();
        list.Add(1);
        list.Add(1); // duplicate via ICollection doesn't throw, just silently fails
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
    public void Clear_OnEmptyList_NoOp()
    {
        var list = Create<int>();
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
    public void Count_AfterAdds_ReturnsCorrectCount()
    {
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        Assert.Equal(3, list.Count);
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
    public void Indexer_OutOfRange_Throws()
    {
        var list = Create<int>();
        list.Add(1);
        Assert.Throws<ArgumentOutOfRangeException>(() => list[1]);
        Assert.Throws<ArgumentOutOfRangeException>(() => list[-1]);
    }

    [Fact]
    public void Indexer_EmptyList_Throws()
    {
        var list = Create<int>();
        Assert.Throws<ArgumentOutOfRangeException>(() => list[0]);
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

    [Fact]
    public void CopyTo_NullArray_ThrowsArgumentNullException()
    {
        var list = Create<int>();
        list.Add(1);
        Assert.Throws<ArgumentNullException>(() => list.CopyTo(null, 0));
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

    #region IReadOnlyList
    [Fact]
    public void IReadOnlyList_Count_Works()
    {
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        IReadOnlyList<int> roList = list;
        Assert.Equal(2, roList.Count);
    }

    [Fact]
    public void IReadOnlyList_Indexer_Works()
    {
        var list = Create<int>();
        list.Add(10);
        list.Add(20);
        IReadOnlyList<int> roList = list;
        Assert.Equal(10, roList[0]);
        Assert.Equal(20, roList[1]);
    }
    #endregion

    #region Custom EqualityComparer
    [Fact]
    public void CustomComparer_CaseInsensitive_TreatsDuplicates()
    {
        var list = HashList.Create<string>(StringComparer.OrdinalIgnoreCase);
        list.Add("Hello");
        Assert.False(list.Add("hello"));
        Assert.Equal(1, list.Count);
    }

    [Fact]
    public void CustomComparer_Contains_UsesComparer()
    {
        var list = HashList.Create<string>(StringComparer.OrdinalIgnoreCase);
        list.Add("Hello");
        Assert.True(list.Contains("hello"));
        Assert.True(list.Contains("HELLO"));
    }

    [Fact]
    public void CustomComparer_Remove_UsesComparer()
    {
        var list = HashList.Create<string>(StringComparer.OrdinalIgnoreCase);
        list.Add("Hello");
        Assert.True(list.Remove("hello"));
        Assert.Equal(0, list.Count);
    }
    #endregion

    #region Capacity
    [Fact]
    public void Create_WithCapacity_DoesNotAffectBehavior()
    {
        var list = HashList.Create<int>(100);
        Assert.Equal(0, list.Count);
        list.Add(1);
        Assert.Equal(1, list.Count);
    }

    [Fact]
    public void Create_WithZeroCapacity_FallsBackToDefault()
    {
        var list = HashList.Create<int>(0);
        list.Add(1);
        Assert.Equal(1, list.Count);
    }

    [Fact]
    public void Create_WithNegativeCapacity_FallsBackToDefault()
    {
        var list = HashList.Create<int>(-1);
        list.Add(1);
        Assert.Equal(1, list.Count);
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
    public void LargeCollection_NoDuplicatesAllowed()
    {
        var list = Create<int>();
        for (var i = 0; i < 100; i++)
            list.Add(i);
        for (var i = 0; i < 100; i++)
            Assert.False(list.Add(i));
        Assert.Equal(100, list.Count);
    }
    #endregion
}
