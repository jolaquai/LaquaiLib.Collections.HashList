# `LaquaiLib.HashList`

Implements a simple hash list, effectively a cross between some set data structure (to guarantee uniqueness of elements) and a list (to keep track of insertion order).

Right now, there's two strategies for the non-concurrent version which are not directly surfaced. Access them through the `HashList` static type which create instances of those strategies. They differ in which operation is more optimized: either indexing and enumeration or deletion. Insertion and `Contains` checks always offer set-like performance, but a choice must be made for `Remove`, `IndexOf` etc.

- Specifying `bool optimizeForRemove: true` returns an impl optimized for `O(1)` removals, but with `O(N / 2)` indexing and enumeration.
- Specifying `bool optimizeForRemove: false` (the default when using an overload of the factory method that doesn't have the parameter) returns an impl optimized for `O(1)` indexing and enumeration, but with `O(N)` removals.

Note that direct enumeration via `foreach` isn't affected as badly, merely differing by a few additional indirections through internal structures.

The **concurrent** variant is merely a wrapper for any of the above strategies since concurrency is orthogonal to the choice of optimization strategy. Reads never collide and are allowed concurrently.

- Snapshot enumeration: prevents holding the lock during iteration (which would cause deadlocks on any write attempts mid-enumeration), at the cost of an allocation per enumeration.
- `Mutate(Action<HashList<T>>)`: executes the passed delegate under a write lock, allowing compound operations that require atomicity.

You should, however, think really well of why you're reaching for the concurrent variant. Concurrent writes make keeping track of insertion order meaningless since the order in which writers acquire the internal lock is non-deterministic. If writes are sufficiently rare, you might be better off with the non-concurrent variant, locking around writes yourself and avoiding all locking overhead on read paths. Even better, if you're building once and then never modifying again, choose the non-concurrent variant, then copy and store that (perhaps using a `ReadOnlyCollection<>` over that copy if you're sharing to unknown consumers).

Contact me on Discord @ `eyeoftheenemy` or open an issue here if you have any questions, suggestions or want to contribute!