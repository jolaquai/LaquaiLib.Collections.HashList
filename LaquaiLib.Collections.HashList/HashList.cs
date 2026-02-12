using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace LaquaiLib.Collections;

/// <summary>
/// Contains factory methods for <see cref="HashList{T}"/> implementations.
/// </summary>
public static class HashList
{
    internal const int DefaultCapacity = 4;

    #region Non-concurrent
    /// <summary>
    /// Creates an implementation of <see cref="HashList{T}"/>.
    /// Optimizes for indexing and enumeration, whereas removal is O(N). Use <see cref="Create{T}(bool)"/> to specify whether to optimize for O(1) removal instead, which turns indexing into O(N).
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <returns>The created <see cref="HashList{T}"/> implementation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashList<T> Create<T>() => Create<T>(DefaultCapacity, EqualityComparer<T>.Default, false);
    /// <summary>
    /// Creates an implementation of <see cref="HashList{T}"/> with internal buffers pre-sized using <paramref name="capacity"/>.
    /// Optimizes for indexing and enumeration, whereas removal is O(N). Use <see cref="Create{T}(int, bool)"/> to specify whether to optimize for O(1) removal instead, which turns indexing into O(N).
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="capacity">The initial capacity of the internal buffers.</param>
    /// <returns>The created <see cref="HashList{T}"/> implementation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashList<T> Create<T>(int capacity) => Create<T>(capacity, EqualityComparer<T>.Default, false);
    /// <summary>
    /// Creates an implementation of <see cref="HashList{T}"/> using the specified <paramref name="comparer"/> for element equality.
    /// Optimizes for indexing and enumeration, whereas removal is O(N). Use <see cref="Create{T}(IEqualityComparer{T}, bool)"/> to specify whether to optimize for O(1) removal instead, which turns indexing into O(N).
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> implementation to use for comparing elements.</param>
    /// <returns>The created <see cref="HashList{T}"/> implementation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashList<T> Create<T>(IEqualityComparer<T> comparer) => Create<T>(DefaultCapacity, comparer, false);
    /// <summary>
    /// Creates an implementation of <see cref="HashList{T}"/> using the specified <paramref name="capacity"/> and <paramref name="comparer"/>.
    /// Optimizes for indexing and enumeration, whereas removal is O(N). Use <see cref="Create{T}(int, IEqualityComparer{T}, bool)"/> to specify whether to optimize for O(1) removal instead, which turns indexing into O(N).
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="capacity">The initial capacity of the internal buffers.</param>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> implementation to use for comparing elements.</param>
    /// <returns>The created <see cref="HashList{T}"/> implementation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashList<T> Create<T>(int capacity, IEqualityComparer<T> comparer) => Create<T>(capacity, comparer, false);
    /// <summary>
    /// Creates an implementation of <see cref="HashList{T}"/>, optionally optimizing for O(1) removal instead of indexing and enumeration, which become O(N) instead.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="optimizeForRemove">Whether to optimize for O(1) removal instead of indexing and enumeration.</param>
    /// <returns>The created <see cref="HashList{T}"/> implementation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashList<T> Create<T>(bool optimizeForRemove) => Create<T>(DefaultCapacity, EqualityComparer<T>.Default, optimizeForRemove);
    /// <summary>
    /// Creates an implementation of <see cref="HashList{T}"/> with internal buffers pre-sized using <paramref name="capacity"/>, optionally optimizing for O(1) removal instead of indexing and enumeration, which become O(N) instead.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="capacity">The initial capacity of the internal buffers.</param>
    /// <param name="optimizeForRemove">Whether to optimize for O(1) removal instead of indexing and enumeration.</param>
    /// <returns>The created <see cref="HashList{T}"/> implementation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashList<T> Create<T>(int capacity, bool optimizeForRemove) => Create(capacity, EqualityComparer<T>.Default, optimizeForRemove);
    /// <summary>
    /// Creates an implementation of <see cref="HashList{T}"/> using the specified <paramref name="comparer"/> for element equality, optionally optimizing for O(1) removal instead of indexing and enumeration, which become O(N) instead.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> implementation to use for comparing elements.</param>
    /// <param name="optimizeForRemove">Whether to optimize for O(1) removal instead of indexing and enumeration.</param>
    /// <returns>The created <see cref="HashList{T}"/> implementation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashList<T> Create<T>(IEqualityComparer<T> comparer, bool optimizeForRemove) => Create<T>(DefaultCapacity, comparer, optimizeForRemove);
    /// <summary>
    /// Creates an implementation of <see cref="HashList{T}"/> using the specified <paramref name="capacity"/> and <paramref name="comparer"/>, optionally optimizing for O(1) removal instead of indexing and enumeration, which become O(N) instead.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="capacity">The initial capacity of the internal buffers.</param>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> implementation to use for comparing elements.</param>
    /// <param name="optimizeForRemove">Whether to optimize for O(1) removal instead of indexing and enumeration.</param>
    /// <returns>The created <see cref="HashList{T}"/> implementation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashList<T> Create<T>(int capacity, IEqualityComparer<T> comparer, bool optimizeForRemove) => optimizeForRemove ? new LinkedListHashList<T>(capacity, comparer) : new DefaultListHashList<T>(capacity, comparer);

    /// <summary>
    /// Creates an implementation of <see cref="HashList{T}"/> using the specified <paramref name="options"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="options">An <see cref="HashListOptions{T}"/> instance containing options for the behavior of the created <see cref="HashList{T}"/> implementation.</param>
    /// <returns>The created <see cref="HashList{T}"/> implementation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
    public static HashList<T> Create<T>(HashListOptions<T> options)
    {
        if (options is null)
            throw new ArgumentNullException(nameof(options));

        return Create(options.Capacity, options.EqualityComparer, options.OptimizeForRemove);
    }
    #endregion

    #region Concurrent
    /// <summary>
    /// Creates an implementation of <see cref="HashList{T}"/>.
    /// Optimizes for indexing and enumeration, whereas removal is O(N). Use <see cref="CreateConcurrent{T}(bool)"/> to specify whether to optimize for O(1) removal instead, which turns indexing into O(N).
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <returns>The created <see cref="HashList{T}"/> implementation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashList<T> CreateConcurrent<T>() => CreateConcurrent<T>(DefaultCapacity, EqualityComparer<T>.Default, false);
    /// <summary>
    /// Creates an implementation of <see cref="HashList{T}"/> with internal buffers pre-sized using <paramref name="capacity"/>.
    /// Optimizes for indexing and enumeration, whereas removal is O(N). Use <see cref="CreateConcurrent{T}(int, bool)"/> to specify whether to optimize for O(1) removal instead, which turns indexing into O(N).
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="capacity">The initial capacity of the internal buffers.</param>
    /// <returns>The created <see cref="HashList{T}"/> implementation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashList<T> CreateConcurrent<T>(int capacity) => CreateConcurrent<T>(capacity, EqualityComparer<T>.Default, false);
    /// <summary>
    /// Creates an implementation of <see cref="HashList{T}"/> using the specified <paramref name="comparer"/> for element equality.
    /// Optimizes for indexing and enumeration, whereas removal is O(N). Use <see cref="CreateConcurrent{T}(IEqualityComparer{T}, bool)"/> to specify whether to optimize for O(1) removal instead, which turns indexing into O(N).
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> implementation to use for comparing elements.</param>
    /// <returns>The created <see cref="HashList{T}"/> implementation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashList<T> CreateConcurrent<T>(IEqualityComparer<T> comparer) => CreateConcurrent<T>(DefaultCapacity, comparer, false);
    /// <summary>
    /// Creates an implementation of <see cref="HashList{T}"/> using the specified <paramref name="capacity"/> and <paramref name="comparer"/>.
    /// Optimizes for indexing and enumeration, whereas removal is O(N). Use <see cref="CreateConcurrent{T}(int, IEqualityComparer{T}, bool)"/> to specify whether to optimize for O(1) removal instead, which turns indexing into O(N).
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="capacity">The initial capacity of the internal buffers.</param>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> implementation to use for comparing elements.</param>
    /// <returns>The created <see cref="HashList{T}"/> implementation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashList<T> CreateConcurrent<T>(int capacity, IEqualityComparer<T> comparer) => CreateConcurrent<T>(capacity, comparer, false);
    /// <summary>
    /// Creates an implementation of <see cref="HashList{T}"/>, optionally optimizing for O(1) removal instead of indexing and enumeration, which become O(N) instead.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="optimizeForRemove">Whether to optimize for O(1) removal instead of indexing and enumeration.</param>
    /// <returns>The created <see cref="HashList{T}"/> implementation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashList<T> CreateConcurrent<T>(bool optimizeForRemove) => CreateConcurrent<T>(DefaultCapacity, EqualityComparer<T>.Default, optimizeForRemove);
    /// <summary>
    /// Creates an implementation of <see cref="HashList{T}"/> with internal buffers pre-sized using <paramref name="capacity"/>, optionally optimizing for O(1) removal instead of indexing and enumeration, which become O(N) instead.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="capacity">The initial capacity of the internal buffers.</param>
    /// <param name="optimizeForRemove">Whether to optimize for O(1) removal instead of indexing and enumeration.</param>
    /// <returns>The created <see cref="HashList{T}"/> implementation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashList<T> CreateConcurrent<T>(int capacity, bool optimizeForRemove) => CreateConcurrent(capacity, EqualityComparer<T>.Default, optimizeForRemove);
    /// <summary>
    /// Creates an implementation of <see cref="HashList{T}"/> using the specified <paramref name="comparer"/> for element equality, optionally optimizing for O(1) removal instead of indexing and enumeration, which become O(N) instead.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> implementation to use for comparing elements.</param>
    /// <param name="optimizeForRemove">Whether to optimize for O(1) removal instead of indexing and enumeration.</param>
    /// <returns>The created <see cref="HashList{T}"/> implementation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashList<T> CreateConcurrent<T>(IEqualityComparer<T> comparer, bool optimizeForRemove) => CreateConcurrent<T>(DefaultCapacity, comparer, optimizeForRemove);
    /// <summary>
    /// Creates an implementation of <see cref="HashList{T}"/> using the specified <paramref name="capacity"/> and <paramref name="comparer"/>, optionally optimizing for O(1) removal instead of indexing and enumeration, which become O(N) instead.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="capacity">The initial capacity of the internal buffers.</param>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> implementation to use for comparing elements.</param>
    /// <param name="optimizeForRemove">Whether to optimize for O(1) removal instead of indexing and enumeration.</param>
    /// <returns>The created <see cref="HashList{T}"/> implementation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashList<T> CreateConcurrent<T>(int capacity, IEqualityComparer<T> comparer, bool optimizeForRemove) => new ConcurrentHashList<T>(optimizeForRemove ? new LinkedListHashList<T>(capacity, comparer) : new DefaultListHashList<T>(capacity, comparer));

    /// <summary>
    /// Creates an implementation of <see cref="HashList{T}"/> using the specified <paramref name="options"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="options">An <see cref="HashListOptions{T}"/> instance containing options for the behavior of the created <see cref="HashList{T}"/> implementation.</param>
    /// <returns>The created <see cref="HashList{T}"/> implementation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
    public static HashList<T> CreateConcurrent<T>(HashListOptions<T> options)
    {
        if (options is null)
            throw new ArgumentNullException(nameof(options));

        return CreateConcurrent(options.Capacity, options.EqualityComparer, options.OptimizeForRemove);
    }
    #endregion
}

/// <summary>
/// Encapsulates options for creating a <see cref="HashList{T}"/> using <see cref="HashList.Create{T}(HashListOptions{T})"/>.
/// </summary>
/// <typeparam name="T">The type of elements in the list.</typeparam>
public sealed class HashListOptions<T>
{
    /// <summary>
    /// The initial capacity of the internal buffers. Must be greater than or equal to 1.
    /// </summary>
    public int Capacity { get; set; } = HashList.DefaultCapacity;
    /// <summary>
    /// An <see cref="IEqualityComparer{T}"/> implementation to use for comparing elements.
    /// If <see langword="null"/>, the default equality comparer for <typeparamref name="T"/> is used.
    /// </summary>
    public IEqualityComparer<T> EqualityComparer { get; set; } = EqualityComparer<T>.Default;
    /// <summary>
    /// Whether to optimize for O(1) removal instead of indexing and enumeration, which become O(N) instead.
    /// </summary>
    public bool OptimizeForRemove { get; set; }
}

/// <summary>
/// Represents a collection of unique elements (similar to an <see cref="ISet{T}"/>) while maining insertion order (similar to a <see cref="IList{T}"/>).
/// </summary>
/// <typeparam name="T">The type of elements in the list.</typeparam>
public abstract class HashList<T> : ICollection<T>, IReadOnlyList<T>
{
    #region ICollection<> impl
    public abstract bool Add(T item);
    void ICollection<T>.Add(T item) => Add(item);
    public abstract void Clear();
    public abstract bool Contains(T item);
    public abstract void CopyTo(T[] array, int arrayIndex);
    public abstract bool Remove(T item);

    public abstract int Count { get; }
    public abstract bool IsReadOnly { get; }
    #endregion
    #region IReadOnlyList<> impl
    public abstract T this[int index] { get; }
    #endregion

    public abstract IEnumerator<T> GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

internal sealed class DefaultListHashList<T> : HashList<T>
{
    private readonly IEqualityComparer<T> _comparer;
    private readonly HashSet<T> _set;
    private readonly List<T> _list;

    public DefaultListHashList(int capacity, IEqualityComparer<T> equalityComparer)
    {
        _comparer = equalityComparer ?? EqualityComparer<T>.Default;
        _set = new HashSet<T>(_comparer);
        _list = new List<T>(capacity >= 1 ? capacity : HashList.DefaultCapacity);
    }

    public override int Count => _list.Count;
    public override bool IsReadOnly { get; }

    public override T this[int index] => _list[index];
    public override bool Add(T item)
    {
        var result = _set.Add(item);
        if (result)
            _list.Add(item);
        return result;
    }
    public override bool Remove(T item)
    {
        if (!_set.Remove(item))
            return false;

        for (var i = 0; i < _list.Count; i++)
            if (_comparer.Equals(_list[i], item))
            {
                _list.RemoveAt(i);
                return true;
            }

        Debug.Fail("Internal buffers were desynced");
        throw new InvalidOperationException("Internal buffers were desynced.");
    }
    public override void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);
    public override bool Contains(T item) => _set.Contains(item);
    public override void Clear()
    {
        _set.Clear();
        _list.Clear();
    }
    public override IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
}

internal sealed class LinkedListHashList<T>(int capacity, IEqualityComparer<T> equalityComparer) : HashList<T>
{
    private readonly Dictionary<T, LinkedListNode<T>> _map = new Dictionary<T, LinkedListNode<T>>(capacity >= 1 ? capacity : HashList.DefaultCapacity, equalityComparer ?? EqualityComparer<T>.Default);
    private readonly LinkedList<T> _list = [];

    public override int Count => _map.Count;
    public override bool IsReadOnly { get; }

    public override T this[int index]
    {
        get
        {
            if (index < 0 || index >= _map.Count)
                throw new ArgumentOutOfRangeException(nameof(index), index, "Index must be within the bounds of the list.");

            // Small optimization: find the "end" of the list to which the requested index is closer, and iterate from there instead of always iterating from the start
            // Logical shift avoids signed division shenanigans if the JIT isn't sure about the manual range check we did above
            if (index > _map.Count >>> 1)
            {
                var node = _list.Last;
                for (var i = _map.Count - 1; i > index; i--)
                    node = node.Previous;
                return node.Value;
            }
            else
            {
                var node = _list.First;
                for (var i = 0; i < index; i++)
                    node = node.Next;
                return node.Value;
            }
        }
    }
    public override bool Add(T item)
    {
        if (_map.TryGetValue(item, out var _))
            return false;

        _map[item] = _list.AddLast(item);
        return true;
    }
    public override void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);
    public override bool Remove(T item)
    {
        if (!_map.TryGetValue(item, out var node))
            return false;

        _map.Remove(item);
        _list.Remove(node);
        return true;
    }
    public override bool Contains(T item) => _map.ContainsKey(item);
    public override void Clear()
    {
        _map.Clear();
        _list.Clear();
    }
    public override IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
}