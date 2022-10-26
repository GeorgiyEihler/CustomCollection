using CustomCollecion.Collection;

namespace CustomCollecion.CollectionQuery;

public static class CustomCollectionQuery
{
    public static ICustomList<T> ToCustomList<T>(this IEnumerable<T> source)
    {
        if (source is null)
        {
            throw new NullReferenceException();
        }

        return new CustomList<T>(source);
    }

    public static IEnumerable<T> Filter<T>(this IEnumerable<T> source, Func<T, bool> query)
    {
        if (source is null || query is null)
        {
            throw new NullReferenceException();
        }

        foreach (var item in source)
        {
            if (query(item))
            {
                yield return item;
            }
        }
    }
    public static IEnumerable<TResult> Get<T, TResult>(this IEnumerable<T> source, Func<T, TResult> getFunc)
    {
        if (source is null || getFunc is null)
        {
            throw new NullReferenceException();
        }

        foreach (var item in source)
        {
            yield return getFunc(item);
        }
    }

    public static IEnumerable<T> SortQuery<T, TKey>(
        this IEnumerable<T> source,
        Func<T, TKey> selector,
        IComparer<TKey> comparer = null)
    {
        comparer ??= Comparer<TKey>.Default;


        if (source is null || selector is null)
        {
            throw new NullReferenceException();
        }

        var sotred = new Sotrer<T, TKey>(source, comparer, selector);

        foreach (var item in sotred._source)
        {
            yield return item;
        }

    }

}

internal class Sotrer<T, TKey>
{
    internal T[] _source;
    private IComparer<TKey> _comparer;
    private Func<T, TKey> _selector;
    internal Sotrer(IEnumerable<T> source, IComparer<TKey> comparer, Func<T, TKey> selector)
    {
        _source = source.ToArray();
        _comparer = comparer ?? Comparer<TKey>.Default;
        _selector = selector;
        QuickSort(0, _source.Length - 1);
        Console.WriteLine(123);
    }

    void QuickSort(int left, int right)
    {
        do
        {
            int i = left;
            int j = right;
            var pivot = _selector(_source[i + ((j - i) >> 1)]);
            do
            {
                while (i < _source.Length && _comparer.Compare(_selector(_source[i]), pivot) > 0)
                {
                    i++;
                }
                while (j >= 0 && _comparer.Compare(_selector(_source[j]), pivot) < 0)
                {
                    j--;
                }

                if (i > j) break;

                if (i < j)
                {
                    var temp = _source[i];
                    _source[i] = _source[j];
                    _source[j] = temp;
                }

                i++;
                j--;

            }
            while (i <= j);

            if (j - left <= right - i)
            {
                if (left < j) QuickSort(left, j);
                left = i;
            }
            else
            {
                if (i < right) QuickSort(i, right);
                right = j;
            }
        }
        while (left < right);
    }

}
