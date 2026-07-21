# PROJECT KNOWLEDGE BASE

**Generated:** 2026-03-04
**Commit:** 52a3fb6
**Branch:** main

## OVERVIEW

C# .NET library: ordered set collection (`HashList<T>`) combining `HashSet` uniqueness with `List` insertion order. Two internal strategies (index-optimized vs removal-optimized) behind factory API. Concurrent wrapper via `ReaderWriterLockSlim`. Targets netstandard2.0 + net5.0.

## STRUCTURE
```
./
├── LaquaiLib.Collections.HashList/       # Library (3 files, 816 LOC)
│   ├── HashList.cs                       # Factory methods, HashListOptions, abstract HashList<T>, DefaultListHashList<T>, LinkedListHashList<T>
│   └── ConcurrentHashList.cs             # ReaderWriterLockSlim wrapper with snapshot enumeration
├── LaquaiLib.Collections.HashList.Tests/  # xUnit v3 tests (4 files, 2661 LOC, 248 tests)
├── LaquaiLib.Collections.HashList.slnx    # Modern .slnx solution format
└── .github/workflows/dotnet.yml           # CI: restore → build → test on ubuntu-latest
```

## WHERE TO LOOK

| Task | Location | Notes |
|------|----------|-------|
| Add collection operation | `HashList.cs` L220-269 (abstract), then both impl classes | Must implement in both `DefaultListHashList<T>` (L271) and `LinkedListHashList<T>` (L360) |
| Change factory API | `HashList.cs` L10-193 | Static `HashList` class, mirror changes across `Create` and `CreateConcurrent` overloads |
| Concurrent behavior | `ConcurrentHashList.cs` | Delegates to inner `HashList<T>`, wraps with read/write locks |
| Configuration options | `HashList.cs` L199-214 | `HashListOptions<T>` class |
| Test a strategy | `DefaultListHashListTests.cs` or `LinkedListHashListTests.cs` | Private `Create<T>()` helper selects strategy |
| Test concurrent | `ConcurrentHashListTests.cs` | Async tests with `TestContext.Current.CancellationToken` |
| Test factory/options | `FactoryAndOptionsTests.cs` | Covers all overload combinations |

## CODE MAP

| Symbol | Type | Location | Role |
|--------|------|----------|------|
| `HashList` | static class | HashList.cs:10 | Factory: `Create<T>()`, `CreateConcurrent<T>()` overloads |
| `HashListOptions<T>` | sealed class | HashList.cs:199 | Config object: Capacity, EqualityComparer, OptimizeForRemove |
| `HashList<T>` | abstract class | HashList.cs:220 | Public API: ICollection\<T\>, IReadOnlyList\<T\>, IReadOnlySet\<T\> (NET5+) |
| `DefaultListHashList<T>` | internal sealed | HashList.cs:271 | HashSet + List — O(1) index, O(N) remove |
| `LinkedListHashList<T>` | internal sealed | HashList.cs:360 | Dictionary + LinkedList — O(1) remove, O(N/2) index |
| `ConcurrentHashList<T>` | public sealed | ConcurrentHashList.cs | ReaderWriterLockSlim wrapper, snapshot enumeration, `Mutate()` for atomic ops |

## CONVENTIONS

- **Namespace**: `LaquaiLib.Collections` (file-scoped)
- **Factory pattern**: Internal impls, public abstract base, static factory class. Never expose concrete types.
- **AggressiveInlining**: Applied to all factory method overloads
- **Dual-buffer consistency**: Both impls maintain parallel data structures (set+list or dict+linkedlist). Desync = `Debug.Fail` + `InvalidOperationException`
- **Null comparer**: Falls back to `EqualityComparer<T>.Default` silently
- **Capacity ≤ 0**: Falls back to `DefaultCapacity` (4) silently
- **Conditional compilation**: `#if NET5_0_OR_GREATER` for `IReadOnlySet<T>` support
- **Test naming**: `[Method]_[Condition]_[ExpectedResult]` (e.g. `Add_UniqueItem_ReturnsTrue`)
- **Test grouping**: `#region` directives by feature
- **InternalsVisibleTo**: Test assembly accesses internal types
- **No nullable**: Nullable reference types disabled globally
- **LangVersion**: latest

## ANTI-PATTERNS

- **Do NOT use ConcurrentHashList for frequent concurrent writes** — insertion order becomes non-deterministic with lock contention
- **Do NOT use ConcurrentHashList for build-once-read-many** — unnecessary locking overhead; use non-concurrent + `ReadOnlyCollection<>`
- **Do NOT instantiate impl classes directly** — always use `HashList.Create<T>()` / `HashList.CreateConcurrent<T>()` factory
- **Do NOT hold enumerator across mutations** — concurrent variant uses snapshot enumeration (allocation per enumeration); non-concurrent variant will throw/corrupt

## COMMANDS
```bash
dotnet restore
dotnet build
dotnet test
dotnet build -c Release    # Generates .nupkg + .snupkg
```

## NOTES

- `.slnx` format (not `.sln`) — requires VS 17.10+ or .NET 9+ SDK
- Test project targets net10.0 while library targets netstandard2.0 + net5.0
- LinkedListHashList indexer uses bidirectional traversal optimization (starts from nearest end)
- `InsertAt` in both impls has rollback logic: if secondary structure fails, primary structure change is reverted
- Set operations on LinkedListHashList are hand-rolled (not delegating to HashSet) — verify correctness when modifying
- CI runs on ubuntu-latest with .NET 10.0.x — no Windows/macOS matrix
