using System;
using System.Collections.Generic;

public static class EnumExtensions
{
    public static string FullName(this Enum source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        return $"{source.GetType().Name}.{source}";
    }
}

public static class EnumerateExtensions
{
    public static IEnumerable<(int Index, T item)> WithIndex<T>(this IEnumerable<T> source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return Iterator(source);
        
        static IEnumerable<(int Index, T item)> Iterator(IEnumerable<T> source)
        {
            int i = 0;
            foreach (var item in source)
                yield return (i++, item);
        }
    }

    public static ListIndexEnumerable<T> WithIndex<T>(this List<T> source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return new ListIndexEnumerable<T>(source);
    }
}

public readonly struct ListIndexEnumerable<T>
{
    private readonly List<T> _list;
    public ListIndexEnumerable(List<T> list) { _list = list; }
    public Enumerator GetEnumerator() => new Enumerator(_list);

    public struct Enumerator
    {
        private readonly List<T> _list;
        private int _index;
        public Enumerator(List<T> list) { _list = list; _index = -1; }
        public bool MoveNext() => ++_index < _list.Count;
        public (int Index, T item) Current => (_index, _list[_index]);
    }
}