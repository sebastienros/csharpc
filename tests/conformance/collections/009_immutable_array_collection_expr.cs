using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

record struct Point(int X, int Y);
record Person(string Name, int Age);

static class Program
{
    static IEnumerable<int> Evens(int n)
    {
        for (var i = 0; i < n; i++) yield return i * 2;
    }

    static ImmutableArray<string> Names() => ["alpha", "beta", "gamma"];

    static void Main()
    {
        ImmutableArray<int> empty = [];
        Console.WriteLine($"empty: Length={empty.Length} IsDefault={empty.IsDefault} IsEmpty={empty.IsEmpty}");

        var names = Names();
        Console.WriteLine($"names: {names.Length} {string.Join(",", names)} IsDefault={names.IsDefault}");
        foreach (var n in names) Console.WriteLine("  " + n);

        ImmutableArray<int> numbers = [1, 2, 3, 4, 5];
        Console.WriteLine($"sum={numbers.Sum()} evens={string.Join(",", numbers.Where(x => x % 2 == 0))} [2]={numbers[2]}");

        int[] arr = [10, 20];
        ImmutableArray<int> spreadArray = [0, .. arr, 30];
        Console.WriteLine("spreadArray: " + string.Join(",", spreadArray));

        ImmutableArray<int> spreadEnumerable = [.. Evens(4), -1];
        Console.WriteLine("spreadEnumerable: " + string.Join(",", spreadEnumerable));

        ImmutableArray<int> spreadImmutable = [.. numbers, .. spreadArray, 99];
        Console.WriteLine($"spreadImmutable: {spreadImmutable.Length} " + string.Join(",", spreadImmutable));

        ImmutableArray<int> onlySpread = [.. empty];
        Console.WriteLine($"onlySpread: Length={onlySpread.Length} IsDefault={onlySpread.IsDefault}");

        ImmutableArray<Point> points = [new Point(1, 2), new Point(3, 4)];
        Console.WriteLine($"points: {string.Join(" ", points)} sumX={points.Sum(p => p.X)}");

        ImmutableArray<Person> people = [new Person("Ann", 31), new("Bob", 42)];
        Console.WriteLine($"people: {people[1]} oldest={people.MaxBy(p => p.Age)!.Name}");

        ImmutableArray<(string Key, int Value)> pairs = [("a", 1), ("b", 2)];
        foreach (var (key, value) in pairs) Console.WriteLine($"  {key}={value}");
        Console.WriteLine($"pairs[1].Key={pairs[1].Key}");

        var list = new List<string> { "x" };
        ImmutableArray<string> mixed = [.. list, .. names, "z"];
        Console.WriteLine($"mixed: {string.Join("|", mixed)} contains beta: {mixed.Contains("beta")}");

        var built = numbers.Add(6);
        Console.WriteLine($"built: {built.Length} last={built[^1]} original={numbers.Length}");
        Console.WriteLine($"select: {string.Join(",", numbers.Select(x => x * x))}");
    }
}
