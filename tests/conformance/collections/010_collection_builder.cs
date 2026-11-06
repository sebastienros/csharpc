using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;

[CollectionBuilder(typeof(Bag), nameof(Bag.Create))]
sealed class Bag<T> : IEnumerable<T>
{
    private readonly T[] _items;
    public string Origin { get; }
    internal Bag(T[] items, string origin) { _items = items; Origin = origin; }
    public int Count => _items.Length;
    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)_items).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

static class Bag
{
    public static Bag<T> Create<T>(ReadOnlySpan<T> items)
    {
        Console.WriteLine($"Bag.Create({items.Length})");
        return new Bag<T>(items.ToArray(), "builder");
    }
}

[CollectionBuilder(typeof(IntRow), nameof(Build))]
readonly struct IntRow : IEnumerable<int>
{
    private readonly int[]? _values;
    private IntRow(int[] values) => _values = values;
    public static IntRow Build(ReadOnlySpan<int> values) => new(values.ToArray());
    public int Total { get { var t = 0; foreach (var v in _values ?? []) t += v; return t; } }
    public int Count => _values?.Length ?? -1;
    public IEnumerator<int> GetEnumerator() => ((IEnumerable<int>)(_values ?? [])).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

static class Program
{
    static int Sum(ReadOnlySpan<int> values)
    {
        var total = 0;
        foreach (var v in values) total += v;
        return total;
    }

    static ImmutableArray<T> Wrap<T>(T first, IEnumerable<T> rest) => [first, .. rest];

    static void Main()
    {
        ImmutableList<string> list = ["b", "a", "c"];
        Console.WriteLine($"ImmutableList: {list.Count} {string.Join(",", list.Sort())}");
        ImmutableList<string> emptyList = [];
        Console.WriteLine($"empty ImmutableList: {emptyList.Count} same={ReferenceEquals(emptyList, ImmutableList<string>.Empty)}");

        ImmutableHashSet<string> set = ["x", "y", "x"];
        Console.WriteLine($"ImmutableHashSet: {set.Count} x={set.Contains("x")} z={set.Contains("z")}");
        ImmutableHashSet<int> emptySet = [];
        Console.WriteLine($"empty ImmutableHashSet: {emptySet.Count}");

        Bag<int> bag = [1, 2, 3];
        Console.WriteLine($"Bag: {bag.Count} {bag.Origin} {string.Join(",", bag)}");
        int[] more = [4, 5];
        Bag<int> spreadBag = [.. bag, .. more];
        Console.WriteLine($"spread Bag: {string.Join(",", spreadBag)}");
        Bag<string> emptyBag = [];
        Console.WriteLine($"empty Bag: {emptyBag.Count}");

        IntRow row = [3, 4, 5];
        Console.WriteLine($"IntRow: {row.Count} total={row.Total}");
        IntRow emptyRow = [];
        Console.WriteLine($"empty IntRow: {emptyRow.Count}");

        var wrapped = Wrap("head", new[] { "t1", "t2" }.Select(s => s.ToUpperInvariant()));
        Console.WriteLine($"generic: {string.Join(",", wrapped)}");

        ImmutableArray<int> immutable = [7, 8];
        List<int> fromImmutable = [.. immutable, 9];
        Console.WriteLine($"List from ImmutableArray: {string.Join(",", fromImmutable)}");
        int[] arrayFromImmutable = [0, .. immutable];
        Console.WriteLine($"array from ImmutableArray: {string.Join(",", arrayFromImmutable)}");

        Console.WriteLine($"span with spreads: {Sum([1, .. more, .. immutable])}");

        JsonNode? one = JsonValue.Create(1), two = JsonValue.Create("two");
        JsonArray json = [one, two, null];
        Console.WriteLine($"JsonArray: {json.Count} {json.ToJsonString()}");
        JsonArray emptyJson = [];
        Console.WriteLine($"empty JsonArray: {emptyJson.Count} {emptyJson.ToJsonString()}");
    }
}
