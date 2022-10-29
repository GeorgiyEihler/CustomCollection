using CustomCollecion.Collection;
using CustomCollecion.CollectionQuery;
using System.Collections;
using System.Text;
using Xunit.Abstractions;

namespace CustomCollection.Test;

public class CustomListTest
{
    #region Test settings
    private static Random _random = new Random();

    private void ListComparer<T>(ICustomList<T> source, T[] expected)
    {
        for (int i = 0; i < source.Count; i++)
        {
            Assert.Equal(expected[i], source[i]);
        }

        Assert.Equal(source.Count, expected.Length);

    }

    /// <summary>
    /// Получить рандомный уникальный лист со значениями
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public static CustomList<object> GetList(int count)
    {
        var list = new CustomList<object>();

        for (var i = 0; i < count; i++)
        {
            list.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(_random.Next(int.MinValue, int.MaxValue).ToString())));

        }

        return list;
    }

    /// <summary>
    /// Получить рандомный уникальный массив со значениями
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public static object[] GetItems(int count)
    {
        var items = new object[count];

        for (var i = 0; i < count; i++)
        {
            items[i] = (Convert.ToBase64String(Encoding.UTF8.GetBytes(_random.Next(int.MinValue, int.MaxValue).ToString())));
        }

        return items;
    }

    /// <summary>
    /// Получить рандомный уникальный лист со значениями
    /// </summary>
    /// <param name="count"></param>
    /// <param name="listsToTest"></param>
    /// <returns></returns>
    public static IEnumerable<object[]> GetRandomUniqueList(int count, int listsToTest)
    {
        for (int iList = 0; iList < listsToTest; iList++)
        {
            var prevValue = 0D;

            var list = new string[count];

            for (var i = 0; i < count; i++)
            {
                list[i] = Convert.ToBase64String(Encoding.UTF8.GetBytes(prevValue++.ToString()));
            }

            yield return new object[] {
                new CollectionTests<string>(list)
            };

        };
    }

    /// <summary>
    /// Коллекция для тестирования
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CollectionTests<T> : IXunitSerializable
    {
        private T[] _results;
        private ICustomList<T> _list;

        public CollectionTests()
        {

        }

        public CollectionTests(T[] results)
        {
            _results = results;
            _list = results.ToCustomList();
        }

        public ICustomList<T> Build()
        {
            return _list;
        }

        public void Deserialize(IXunitSerializationInfo info)
        {
            info.GetValue<T>("_source");

            _results = info.GetValue<T[]>("_results");

            _list = _results.ToCustomList();
        }

        public void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue("_results", _results, typeof(T[]));
        }
    } 

    #endregion

    [Theory]
    [InlineData(10)]
    [InlineData(16)]
    [InlineData(1000)]
    public void Add_AddingNulls(int listSize)
    {
        var list = new CustomList<object>();

        for (int i = 0; i < listSize; i++)
        {
            list.Add(null);
        }

        Assert.Equal(listSize, list.Count);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(16)]
    [InlineData(1000)]
    public void Add_AddingValues(int listSize)
    {
        var items = GetItems(listSize);

        var list = new CustomList<object>();

        for (int i = 0; i < listSize; i++)
        {
            list.Add(items[i]);
        }

        ListComparer(list, items);
    }

    [Fact]
    public void GetEnumerator_Foreach()
    {
        var list = GetList(100);

        var collIndex = 0;

        foreach (var item in list)
        {
            Assert.Equal(item, list[collIndex]);
            collIndex++;
        }
        Assert.Equal(100, collIndex);
    }

    [Theory]
    [InlineData(-100)]
    [InlineData(int.MinValue)]
    public void RemoveAt_RemoveAtInvalidValue_ExpectError(int index)
    {
        var list = GetList(250);
           
        Assert.Throws<InvalidOperationException>(() => list.RemoveAt(index));
    }

    [Theory]
    [MemberData(nameof(GetRandomUniqueList), 120, 12)]
    public void Remove_RemoveItem(CollectionTests<string> source)
    {
        var customList = source.Build().ToCustomList();

        var indexValue = customList.Count >> 1;

        var item = customList[indexValue];

        customList.Remove(item);

        Assert.NotEqual(item, customList[indexValue]);
        Assert.Equal(119, customList.Count);
        Assert.Equal(-1, customList.IndexOf(item));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(50)]
    [InlineData(900)]
    public void Index_GetIndexOutOfRange(int index)
    {
        var list = new CustomList<int>();

        Assert.Throws<ArgumentOutOfRangeException>(() => list[index]);

        ListComparer(list, Array.Empty<int>());
    }

    [Theory]
    [InlineData(50)]
    [InlineData(900)]
    public void Index_GetIndexSize(int size)
    {
        var items = GetItems(size);

        var list = new CustomList<object>(items);

        Assert.Throws<ArgumentOutOfRangeException>(() => list[size]);

        ListComparer(list, items);
    }

    [Theory]
    [MemberData(nameof(GetRandomUniqueList), 200, 12)]
    public void Index_SetItem(CollectionTests<string> source)
    {
        var customList = source.Build().ToCustomList();

        var item = GetItems(1)[0];

        var index = customList.Count >> 1;

        customList[index] = item.ToString();

        Assert.Equal(item.ToString(), customList[index]);

    }

    [Theory]
    [InlineData(50)]
    [InlineData(900)]
    public void Clear_AddAndClear(int size)
    {
        var list = GetList(size);

        list.Add(GetItems(1)[0]);

        list.Clear();

        Assert.Equal(0, list.Count);

        ListComparer(list, Array.Empty<object>());
    }

    [Theory]
    [InlineData(50)]
    [InlineData(900)]
    public void RemoveAt_AddAndRemove(int size)
    {
        var items = GetItems(size);

        var list = new CustomList<object>(items);

        var item = GetItems(1)[0];

        var index = size >> 1;

        list.Insert(index, item);

        Assert.Equal(size + 1, list.Count);

        Assert.Equal(item, list[index]);

        list.RemoveAt(index);

        Assert.Equal(size, list.Count);

        ListComparer(list, items);
    }

    [Fact]
    public void Clear_ClearEmpty()
    {
        var list = new CustomList<object>();

        list.Clear();

        list.Clear();

        list.Clear();

        list.Clear();

        Assert.Equal(0, list.Count);
    }

    [Theory]
    [InlineData(50)]
    [InlineData(900)]
    public void Count_AddClearAdd(int size)
    {
        var items = GetItems(size);

        var list = new CustomList<object>(items);

        var item = GetItems(1)[0];

        list.Add(item);

        var resizeArray = new object[size + 1];

        Array.Copy(items, resizeArray, size);

        resizeArray[size] = item;

        ListComparer(list, resizeArray);

        Assert.Equal(size + 1, list.Count);

        list.Clear();

        ListComparer(list, Array.Empty<object>());

        list.Add(item);

        ListComparer(list, new object[] { item });

        Assert.Equal(1, list.Count);
    }

    [Fact]
    public void Contains_EmptyList()
    {
        var list = new CustomList<object>();

        Assert.False(list.Contains(null));
    }

}
