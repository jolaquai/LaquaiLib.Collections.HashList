namespace LaquaiLib.Collections;

public sealed class ConcurrentHashList<T> : HashList<T>, IDisposable
{
    #region Concurrent wrapper
    private readonly ReaderWriterLockSlim _rwls = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
    internal ConcurrentHashList(HashList<T> inner)
    {
        _inner = inner;
    }
    #endregion

    #region Satisfy abstract impl
    private readonly HashList<T> _inner;

    public override bool Add(T item)
    {
        _rwls.EnterWriteLock();
        try
        {
            return _inner.Add(item);
        }
        finally
        {
            _rwls.ExitWriteLock();
        }
    }
    public override void Clear()
    {
        _rwls.EnterWriteLock();
        try
        {
            _inner.Clear();
        }
        finally
        {
            _rwls.ExitWriteLock();
        }
    }
    public override bool Contains(T item)
    {
        _rwls.EnterReadLock();
        try
        {
            return _inner.Contains(item);
        }
        finally
        {
            _rwls.ExitReadLock();
        }
    }
    public override void CopyTo(T[] array, int arrayIndex)
    {
        _rwls.EnterReadLock();
        try
        {
            _inner.CopyTo(array, arrayIndex);
        }
        finally
        {
            _rwls.ExitReadLock();
        }
    }
    public override bool Remove(T item)
    {
        _rwls.EnterWriteLock();
        try
        {
            return _inner.Remove(item);

        }
        finally
        {
            _rwls.ExitWriteLock();
        }
    }
    public override int Count
    {
        get
        {
            _rwls.EnterReadLock();
            try
            {
                return _inner.Count;
            }
            finally
            {
                _rwls.ExitReadLock();
            }
        }
    }
    public override bool IsReadOnly { get; }

    public override T this[int index]
    {
        get
        {
            _rwls.EnterReadLock();
            try
            {
                return _inner[index];
            }
            finally
            {
                _rwls.ExitReadLock();
            }
        }
    }

    public override IEnumerator<T> GetEnumerator()
    {
        T[] snapshot;
        _rwls.EnterReadLock();
        try
        {
            snapshot = new T[_inner.Count];
            _inner.CopyTo(snapshot, 0);
        }
        finally
        {
            _rwls.ExitReadLock();
        }
        return ((IEnumerable<T>)snapshot).GetEnumerator();
    }
    #endregion

    #region Extra functionality
    /// <summary>
    /// Atomically adds or removes <paramref name="item"/> based on <paramref name="add"/>, avoiding a double lock acquire from calling <see cref="Add"/>/<see cref="Remove"/> separately after a <see cref="Contains"/> check.
    /// </summary>
    /// <param name="add"><see langword="true"/> to attempt to add, <see langword="false"/> to attempt to remove <paramref name="item"/>.</param>
    /// <returns>If <paramref name="add"/> was <see langword="true"/>, returns <see langword="true"/> if <paramref name="item"/> was not present and added successfully. If <see langword="false"/>, returns <see langword="true"/> if <paramref name="item"/> was present and removed successfully.</returns>
    public bool AddOrRemove(T item, bool add)
    {
        _rwls.EnterWriteLock();
        try
        {
            return add ? _inner.Add(item) : _inner.Remove(item);
        }
        finally
        {
            _rwls.ExitWriteLock();
        }
    }
    /// <summary>
    /// Executes <paramref name="action"/> under the write lock for compound operations that need atomicity.
    /// </summary>
    public void Mutate(Action<HashList<T>> action)
    {
        _rwls.EnterWriteLock();
        try
        {
            action(_inner);
        }
        finally
        {
            _rwls.ExitWriteLock();
        }
    }
    #endregion

    #region IDisposable impl
    public void Dispose() => _rwls.Dispose();
    #endregion
}