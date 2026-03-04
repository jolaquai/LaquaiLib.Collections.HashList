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
        Assert.Single(list);
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
        Assert.Single(list);
    }

    [Fact]
    public void Remove_ItemNoLongerContained()
    {
        var list = Create<int>();
        list.Add(1);
        list.Remove(1);
        Assert.DoesNotContain(1, list);
    }

    [Fact]
    public void Remove_ThenReAdd_Succeeds()
    {
        var list = Create<int>();
        list.Add(1);
        list.Remove(1);
        Assert.True(list.Add(1));
        Assert.Single(list);
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
        Assert.Contains(42, list);
    }

    [Fact]
    public void Contains_NonExistingItem_ReturnsFalse()
    {
        var list = Create<int>();
        Assert.DoesNotContain(42, list);
    }

    [Fact]
    public void Contains_AfterClear_ReturnsFalse()
    {
        var list = Create<int>();
        list.Add(1);
        list.Clear();
        Assert.DoesNotContain(1, list);
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
        Assert.Empty(list);
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
        Assert.Empty(list);
    }

    [Fact]
    public void Count_DuplicateAdds_NotCounted()
    {
        var list = Create<int>();
        list.Add(1);
        list.Add(1);
        Assert.Single(list);
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
        Assert.Single(list);
    }

    [Fact]
    public void CustomComparer_Contains_UsesComparer()
    {
        var list = HashList.Create<string>(StringComparer.OrdinalIgnoreCase, optimizeForRemove: true);
        list.Add("Hello");
#pragma warning disable xUnit2017 // Intentionally testing HashList.Contains with custom comparer
        Assert.True(list.Contains("hello"));
#pragma warning restore xUnit2017
    }

    [Fact]
    public void CustomComparer_Remove_UsesComparer()
    {
        var list = HashList.Create<string>(StringComparer.OrdinalIgnoreCase, optimizeForRemove: true);
        list.Add("Hello");
        Assert.True(list.Remove("hello"));
        Assert.Empty(list);
    }
    #endregion

    #region InsertAt
    [Fact]
    public void InsertAt_Beginning_ShiftsElements()
    {
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        Assert.True(list.InsertAt(0, 0));
        Assert.Equal(0, list[0]);
        Assert.Equal(1, list[1]);
        Assert.Equal(2, list[2]);
        Assert.Equal(3, list.Count);
    }

    [Fact]
    public void InsertAt_Middle_ShiftsElements()
    {
        var list = Create<int>();
        list.Add(1);
        list.Add(3);
        Assert.True(list.InsertAt(1, 2));
        Assert.Equal(1, list[0]);
        Assert.Equal(2, list[1]);
        Assert.Equal(3, list[2]);
    }

    [Fact]
    public void InsertAt_End_AppendsElement()
    {
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        Assert.True(list.InsertAt(2, 3));
        Assert.Equal(1, list[0]);
        Assert.Equal(2, list[1]);
        Assert.Equal(3, list[2]);
    }

    [Fact]
    public void InsertAt_EmptyList_AtZero_Works()
    {
        var list = Create<int>();
        Assert.True(list.InsertAt(0, 42));
        Assert.Single(list);
        Assert.Equal(42, list[0]);
    }

    [Fact]
    public void InsertAt_DuplicateItem_ReturnsFalse()
    {
        var list = Create<int>();
        list.Add(1);
        Assert.False(list.InsertAt(0, 1));
        Assert.Single(list);
    }

    [Fact]
    public void InsertAt_NegativeIndex_Throws()
    {
        var list = Create<int>();
        Assert.Throws<ArgumentOutOfRangeException>(() => list.InsertAt(-1, 1));
    }

    [Fact]
    public void InsertAt_IndexGreaterThanCount_Throws()
    {
        var list = Create<int>();
        list.Add(1);
        Assert.Throws<ArgumentOutOfRangeException>(() => list.InsertAt(2, 2));
    }

    [Fact]
    public void InsertAt_DuplicateItem_ThrowsArgumentOutOfRangeForOutOfRangeIndex()
    {
        var list = Create<int>();
        list.Add(1);
        Assert.Throws<ArgumentOutOfRangeException>(() => list.InsertAt(5, 1));
    }

    [Fact]
    public void InsertAt_MaintainsContainsConsistency()
    {
        var list = Create<int>();
        list.Add(1);
        list.Add(3);
        list.InsertAt(1, 2);
        Assert.Contains(1, list);
        Assert.Contains(2, list);
        Assert.Contains(3, list);
    }

    [Fact]
    public void InsertAt_ThenRemove_Works()
    {
        var list = Create<int>();
        list.Add(1);
        list.Add(3);
        list.InsertAt(1, 2);
        list.Remove(2);
        Assert.Equal(2, list.Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(3, list[1]);
    }

    [Fact]
    public void InsertAt_NearEnd_UsesBackwardTraversal()
    {
        // Test insertion near the end to exercise the backward traversal optimization
        var list = Create<int>();
        for (var i = 0; i < 20; i++)
            list.Add(i * 2); // 0, 2, 4, ..., 38

        // Insert at index 18 (near the end of 20 items)
        Assert.True(list.InsertAt(18, 99));
        Assert.Equal(21, list.Count);
        Assert.Equal(34, list[17]);
        Assert.Equal(99, list[18]);
        Assert.Equal(36, list[19]);
        Assert.Equal(38, list[20]);
    }
    #endregion

    #region IndexOf
    [Fact]
    public void IndexOf_ExistingItem_ReturnsCorrectIndex()
    {
        var list = Create<int>();
        list.Add(10);
        list.Add(20);
        list.Add(30);
        Assert.Equal(0, list.IndexOf(10));
        Assert.Equal(1, list.IndexOf(20));
        Assert.Equal(2, list.IndexOf(30));
    }

    [Fact]
    public void IndexOf_NonExistingItem_ReturnsNegativeOne()
    {
        var list = Create<int>();
        list.Add(1);
        Assert.Equal(-1, list.IndexOf(99));
    }

    [Fact]
    public void IndexOf_EmptyList_ReturnsNegativeOne()
    {
        var list = Create<int>();
        Assert.Equal(-1, list.IndexOf(1));
    }

    [Fact]
    public void IndexOf_AfterRemoval_ReturnsNegativeOne()
    {
        var list = Create<int>();
        list.Add(1);
        list.Remove(1);
        Assert.Equal(-1, list.IndexOf(1));
    }

    [Fact]
    public void IndexOf_AfterRemoval_UpdatesIndices()
    {
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        list.Remove(1);
        Assert.Equal(0, list.IndexOf(2));
        Assert.Equal(1, list.IndexOf(3));
    }

    [Fact]
    public void IndexOf_WithCustomComparer_UsesComparer()
    {
        var list = HashList.Create<string>(StringComparer.OrdinalIgnoreCase, optimizeForRemove: true);
        list.Add("Hello");
        list.Add("World");
        Assert.Equal(0, list.IndexOf("hello"));
        Assert.Equal(1, list.IndexOf("WORLD"));
    }

    [Fact]
    public void IndexOf_AfterInsertAt_ReturnsCorrectIndex()
    {
        var list = Create<int>();
        list.Add(1);
        list.Add(3);
        list.InsertAt(1, 2);
        Assert.Equal(0, list.IndexOf(1));
        Assert.Equal(1, list.IndexOf(2));
        Assert.Equal(2, list.IndexOf(3));
    }

    [Fact]
    public void IndexOf_LargeList_ReturnsCorrectIndices()
    {
        var list = Create<int>();
        for (var i = 0; i < 100; i++)
            list.Add(i);
        for (var i = 0; i < 100; i++)
            Assert.Equal(i, list.IndexOf(i));
    }
    #endregion

    #region IReadOnlySet
    [Fact]
    public void IsSubsetOf_EmptySet_ReturnsTrueForAny()
    {
        var list = Create<int>();
        Assert.True(list.IsSubsetOf(new[] { 1, 2, 3 }));
        Assert.True(list.IsSubsetOf(Array.Empty<int>()));
    }

    [Fact]
    public void IsSubsetOf_ProperSubset_ReturnsTrue()
    {
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        Assert.True(list.IsSubsetOf(new[] { 1, 2, 3 }));
    }

    [Fact]
    public void IsSubsetOf_EqualSets_ReturnsTrue()
    {
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        Assert.True(list.IsSubsetOf(new[] { 1, 2 }));
    }

    [Fact]
    public void IsSubsetOf_Superset_ReturnsFalse()
    {
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        Assert.False(list.IsSubsetOf(new[] { 1, 2 }));
    }

    [Fact]
    public void IsSubsetOf_ViaIReadOnlySet_ReturnsTrue()
    {
        // Exercises the IReadOnlySet<T> fast path in LinkedListHashList.IsSubsetOf
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        IReadOnlySet<int> other = new HashSet<int>([1, 2, 3]);
        Assert.True(list.IsSubsetOf(other));
    }

    [Fact]
    public void IsSubsetOf_ViaISet_ReturnsTrue()
    {
        // Exercises the ISet<T> fast path in LinkedListHashList.IsSubsetOf
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        ISet<int> other = new SortedSet<int> { 1, 2, 3 };
        Assert.True(list.IsSubsetOf(other));
    }

    [Fact]
    public void IsSubsetOf_ViaEnumerable_ReturnsTrue()
    {
        // Exercises the IEnumerable<T> fallback path in LinkedListHashList.IsSubsetOf
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        IEnumerable<int> other = new List<int> { 1, 2, 3 };
        Assert.True(list.IsSubsetOf(other));
    }

    [Fact]
    public void IsSubsetOf_MismatchedComparer_UsesListComparer()
    {
        // HashList uses OrdinalIgnoreCase; other uses ordinal (case-sensitive).
        // "hello" and "Hello" are equal under OrdinalIgnoreCase, so IsSubsetOf must return true.
        // The ISet<T>/IReadOnlySet<T> fast paths previously called other.Contains(), using the
        // wrong comparer and returning false. Verify the fix enforces the list's own comparer.
        var list = HashList.Create<string>(StringComparer.OrdinalIgnoreCase, optimizeForRemove: true);
        list.Add("hello");
        ISet<string> other = new SortedSet<string>(StringComparer.Ordinal) { "Hello", "world" };
        Assert.True(list.IsSubsetOf(other));
    }

    [Fact]
    public void IsProperSubsetOf_EmptySet_NonEmptyOther_ReturnsTrue()
    {
        var list = Create<int>();
        Assert.True(list.IsProperSubsetOf(new[] { 1 }));
    }

    [Fact]
    public void IsProperSubsetOf_EmptySet_EmptyOther_ReturnsFalse()
    {
        var list = Create<int>();
        Assert.False(list.IsProperSubsetOf(Array.Empty<int>()));
    }

    [Fact]
    public void IsProperSubsetOf_ProperSubset_ReturnsTrue()
    {
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        Assert.True(list.IsProperSubsetOf(new[] { 1, 2, 3 }));
    }

    [Fact]
    public void IsProperSubsetOf_EqualSets_ReturnsFalse()
    {
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        Assert.False(list.IsProperSubsetOf(new[] { 1, 2 }));
    }

    [Fact]
    public void IsProperSubsetOf_Superset_ReturnsFalse()
    {
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        Assert.False(list.IsProperSubsetOf(new[] { 1, 2 }));
    }

    [Fact]
    public void IsProperSubsetOf_ViaIReadOnlySet_ReturnsTrue()
    {
        // Exercises the IReadOnlySet<T> fast path in LinkedListHashList.IsProperSubsetOf
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        IReadOnlySet<int> other = new HashSet<int>([1, 2, 3]);
        Assert.True(list.IsProperSubsetOf(other));
    }

    [Fact]
    public void IsProperSubsetOf_ViaISet_ReturnsTrue()
    {
        // Exercises the ISet<T> fast path in LinkedListHashList.IsProperSubsetOf
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        ISet<int> other = new SortedSet<int> { 1, 2, 3 };
        Assert.True(list.IsProperSubsetOf(other));
    }

    [Fact]
    public void IsProperSubsetOf_ViaEnumerable_ReturnsTrue()
    {
        // Exercises the IEnumerable<T> fallback path in LinkedListHashList.IsProperSubsetOf
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        IEnumerable<int> other = new List<int> { 1, 2, 3 };
        Assert.True(list.IsProperSubsetOf(other));
    }

    [Fact]
    public void IsProperSubsetOf_MismatchedComparer_UsesListComparer()
    {
        // HashList uses OrdinalIgnoreCase; other uses ordinal (case-sensitive).
        // "hello" ∈ other as "Hello" under OrdinalIgnoreCase, so IsProperSubsetOf must return true.
        var list = HashList.Create<string>(StringComparer.OrdinalIgnoreCase, optimizeForRemove: true);
        list.Add("hello");
        ISet<string> other = new SortedSet<string>(StringComparer.Ordinal) { "Hello", "world" };
        Assert.True(list.IsProperSubsetOf(other));
    }

    [Fact]
    public void IsSupersetOf_EmptyOther_ReturnsTrue()
    {
        var list = Create<int>();
        list.Add(1);
        Assert.True(list.IsSupersetOf(Array.Empty<int>()));
    }

    [Fact]
    public void IsSupersetOf_EqualSets_ReturnsTrue()
    {
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        Assert.True(list.IsSupersetOf(new[] { 1, 2 }));
    }

    [Fact]
    public void IsSupersetOf_ProperSuperset_ReturnsTrue()
    {
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        Assert.True(list.IsSupersetOf(new[] { 1, 2 }));
    }

    [Fact]
    public void IsSupersetOf_SubsetOfOther_ReturnsFalse()
    {
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        Assert.False(list.IsSupersetOf(new[] { 1, 2, 3 }));
    }

    [Fact]
    public void IsSupersetOf_ViaISet_ReturnsTrue()
    {
        // Exercises the ISet<T> fast path in LinkedListHashList.IsSupersetOf
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        ISet<int> other = new SortedSet<int> { 1, 2 };
        Assert.True(list.IsSupersetOf(other));
    }

    [Fact]
    public void IsSupersetOf_ViaISet_CountShortCircuit_ReturnsFalse()
    {
        // Exercises the ISet<T> count short-circuit in LinkedListHashList.IsSupersetOf
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        ISet<int> other = new SortedSet<int> { 1, 2, 3 };
        Assert.False(list.IsSupersetOf(other));
    }

    [Fact]
    public void IsProperSupersetOf_EmptySet_ReturnsFalse()
    {
        var list = Create<int>();
        Assert.False(list.IsProperSupersetOf(Array.Empty<int>()));
    }

    [Fact]
    public void IsProperSupersetOf_NonEmptySet_SupersetOfEmptyOther_ReturnsTrue()
    {
        var list = Create<int>();
        list.Add(1);
        Assert.True(list.IsProperSupersetOf(Array.Empty<int>()));
    }

    [Fact]
    public void IsProperSupersetOf_ProperSuperset_ReturnsTrue()
    {
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        Assert.True(list.IsProperSupersetOf(new[] { 1, 2 }));
    }

    [Fact]
    public void IsProperSupersetOf_EqualSets_ReturnsFalse()
    {
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        Assert.False(list.IsProperSupersetOf(new[] { 1, 2 }));
    }

    [Fact]
    public void IsProperSupersetOf_SubsetOfOther_ReturnsFalse()
    {
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        Assert.False(list.IsProperSupersetOf(new[] { 1, 2, 3 }));
    }

    [Fact]
    public void IsProperSupersetOf_ViaISet_ReturnsTrue()
    {
        // Exercises the ISet<T> fast path in LinkedListHashList.IsProperSupersetOf
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        ISet<int> other = new SortedSet<int> { 1, 2 };
        Assert.True(list.IsProperSupersetOf(other));
    }

    [Fact]
    public void IsProperSupersetOf_ViaIReadOnlySet_ReturnsTrue()
    {
        // Exercises the IReadOnlySet<T> fast path in LinkedListHashList.IsProperSupersetOf
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        IReadOnlySet<int> other = new HashSet<int>([1, 2]);
        Assert.True(list.IsProperSupersetOf(other));
    }

    [Fact]
    public void IsProperSupersetOf_ViaEnumerable_ReturnsTrue()
    {
        // Exercises the IEnumerable<T> fallback path in LinkedListHashList.IsProperSupersetOf
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        IEnumerable<int> other = new List<int> { 1, 2 };
        Assert.True(list.IsProperSupersetOf(other));
    }

    [Fact]
    public void IsProperSupersetOf_ViaEnumerable_WithNonMember_ReturnsFalse()
    {
        // Exercises the early-return branch in the IEnumerable<T> fallback of IsProperSupersetOf
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        IEnumerable<int> other = new List<int> { 1, 99 };
        Assert.False(list.IsProperSupersetOf(other));
    }

    [Fact]
    public void Overlaps_WithCommonElement_ReturnsTrue()
    {
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        Assert.True(list.Overlaps(new[] { 2, 3 }));
    }

    [Fact]
    public void Overlaps_NoCommonElements_ReturnsFalse()
    {
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        Assert.False(list.Overlaps(new[] { 3, 4 }));
    }

    [Fact]
    public void Overlaps_EmptyList_ReturnsFalse()
    {
        var list = Create<int>();
        Assert.False(list.Overlaps(new[] { 1, 2 }));
    }

    [Fact]
    public void Overlaps_EmptyOther_ReturnsFalse()
    {
        var list = Create<int>();
        list.Add(1);
        Assert.False(list.Overlaps(Array.Empty<int>()));
    }

    [Fact]
    public void SetEquals_EqualSets_ReturnsTrue()
    {
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        Assert.True(list.SetEquals(new[] { 1, 2, 3 }));
    }

    [Fact]
    public void SetEquals_DifferentOrder_ReturnsTrue()
    {
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        Assert.True(list.SetEquals(new[] { 3, 1, 2 }));
    }

    [Fact]
    public void SetEquals_DifferentElements_ReturnsFalse()
    {
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        Assert.False(list.SetEquals(new[] { 1, 3 }));
    }

    [Fact]
    public void SetEquals_DifferentSizes_ReturnsFalse()
    {
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        Assert.False(list.SetEquals(new[] { 1, 2, 3 }));
    }

    [Fact]
    public void SetEquals_EmptyBoth_ReturnsTrue()
    {
        var list = Create<int>();
        Assert.True(list.SetEquals(Array.Empty<int>()));
    }

    [Fact]
    public void SetEquals_WithDuplicatesInOther_ReturnsTrue()
    {
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        Assert.True(list.SetEquals(new[] { 1, 1, 2, 2 }));
    }

    [Fact]
    public void SetEquals_ViaIReadOnlySet_ReturnsTrue()
    {
        // Exercises the IReadOnlySet<T> fast path in LinkedListHashList.SetEquals
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        IReadOnlySet<int> other = new HashSet<int>([1, 2]);
        Assert.True(list.SetEquals(other));
    }

    [Fact]
    public void SetEquals_ViaISet_ReturnsTrue()
    {
        // Exercises the ISet<T> fast path in LinkedListHashList.SetEquals
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        ISet<int> other = new SortedSet<int> { 1, 2 };
        Assert.True(list.SetEquals(other));
    }

    [Fact]
    public void SetEquals_ViaEnumerable_ReturnsTrue()
    {
        // Exercises the IEnumerable<T> fallback path in LinkedListHashList.SetEquals
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        IEnumerable<int> other = new List<int> { 1, 2 };
        Assert.True(list.SetEquals(other));
    }

    [Fact]
    public void SetEquals_MismatchedComparer_UsesListComparer()
    {
        // HashList uses OrdinalIgnoreCase; other uses ordinal (case-sensitive).
        // "hello" == "Hello" under OrdinalIgnoreCase, so SetEquals must return true.
        var list = HashList.Create<string>(StringComparer.OrdinalIgnoreCase, optimizeForRemove: true);
        list.Add("hello");
        ISet<string> other = new SortedSet<string>(StringComparer.Ordinal) { "Hello" };
        Assert.True(list.SetEquals(other));
    }

    [Fact]
    public void IsSupersetOf_MismatchedComparer_CountInflatedByDuplicates_ReturnsTrue()
    {
        // OrdinalIgnoreCase list {"hello"} should be superset of Ordinal SortedSet {"hello","Hello"}
        // because under OrdinalIgnoreCase both "hello" and "Hello" are contained
        var hashList = HashList.Create<string>(StringComparer.OrdinalIgnoreCase, optimizeForRemove: true);
        hashList.Add("hello");
        // SortedSet uses default Ordinal comparer, so "hello" != "Hello" → Count=2
        var other = new SortedSet<string> { "hello", "Hello" };
        Assert.True(hashList.IsSupersetOf(other));
    }

    [Fact]
    public void SetEquals_MismatchedComparer_DuplicatesInOther_ReturnsTrue()
    {
        // OrdinalIgnoreCase list {"hello"} should equal Ordinal SortedSet {"hello","Hello"}
        // because under OrdinalIgnoreCase, both collapse to the same element
        var hashList = HashList.Create<string>(StringComparer.OrdinalIgnoreCase, optimizeForRemove: true);
        hashList.Add("hello");
        var other = new SortedSet<string> { "hello", "Hello" };
        Assert.True(hashList.SetEquals(other));
    }

    [Fact]
    public void IReadOnlySet_Interface_Works()
    {
        var list = Create<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        IReadOnlySet<int> roSet = list;
        Assert.True(roSet.IsSubsetOf(new[] { 1, 2, 3, 4 }));
        Assert.True(roSet.IsSupersetOf(new[] { 1, 2 }));
        Assert.True(roSet.SetEquals(new[] { 1, 2, 3 }));
        Assert.True(roSet.Overlaps(new[] { 3, 4 }));
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
