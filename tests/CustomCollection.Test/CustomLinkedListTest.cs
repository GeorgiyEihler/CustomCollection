using CustomCollecion.Collection;
using CustomCollecion.CollectionQuery;
using System.Text;
using Xunit.Abstractions;

namespace CustomCollection.Test;

public class CustomLinkedListTest
{
    #region Tests Helpers

    private static Random _random = new Random();

    private static object GetItem()
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(_random.Next(int.MinValue, int.MaxValue).ToString()));
    }

    private void LinkedListComparer<T>(ICustomLinkedList<T> source, T[] expected)
    {
        var counter = 0;

        foreach (var nodeVal in source)
        {
            Assert.Equal(expected[counter], nodeVal);
            counter++;
        }

        Assert.Equal(source.Count, expected.Length);

    }

    /// <summary>
    /// Получить рандомный уникальный лист со значениями
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public static CustomLinkedList<object> GetList(int count)
    {
        var linkedList = new CustomLinkedList<object>();

        for (var i = 0; i < count; i++)
        {
            linkedList.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(_random.Next(int.MinValue, int.MaxValue).ToString())));
        }

        return linkedList;
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

    #endregion

    [Fact]
    public void Ctor_EmptyLinkedList()
    {
        var linkedList = new CustomLinkedList<object>();

        LinkedListComparer(linkedList, Array.Empty<object>());

        Assert.Equal(0, linkedList.Count);

    }

    [Theory]
    [InlineData(12)]
    [InlineData(16)]
    [InlineData(32)]
    public void Ctor_IEnumerable(int size)
    {
        var items = GetItems(size);

        var linkedList = new CustomLinkedList<object>(items);

        LinkedListComparer(linkedList, items);

        Assert.Equal(size, linkedList.Count);
    }

    [Fact]
    public void Ctor_IEnumerableEmpty()
    {
        Assert.Throws<ArgumentNullException>(() => new CustomLinkedList<object>(null));
    }

    [Fact]
    public void AddAfter_AddNullNode()
    {
        var linkedList = new CustomLinkedList<int>();

        Assert.Throws<ArgumentNullException>(() => linkedList.AddAfter(null, 1));
    }

    [Fact]
    public void AddAfter_AddAnotherListNode()
    {
        var linkedList = new CustomLinkedList<int>();

        var linListTemp = new CustomLinkedList<int>();

        linListTemp.Add(1);

        linkedList.Add(1);

        var listNode = linListTemp.First;

        Assert.Throws<InvalidOperationException>(() => linkedList.AddAfter(linkedList.First, listNode));
    }

    [Fact]
    public void AddAfter_AddNewNode()
    {
        var linkedList = new CustomLinkedList<int>();

        linkedList.Add(1);

        var node = new CusomNode<int>(15);

        linkedList.AddAfter(linkedList.First, node);

        Assert.Equal(1, linkedList.First.Value);

        Assert.Equal(15, linkedList.Last.Value);
    }

    [Theory]
    [InlineData(15, 5)]
    [InlineData(8, 4)]
    public void AddAfter_AddNewInMiddleNode(int size, int placeIn)
    {
        CusomNode<object> targetNode = null;

        var items = GetItems(size);

        var linkedList = new CustomLinkedList<object>();

        for (int i = 0; i < items.Length; i++)
        {
            linkedList.Add(items[i]);
            if (placeIn - 1 == i)
            {
                targetNode = linkedList.Last;
            }
            
        }

        var middleItem = GetItem();

        var targetArray = new object[items.Length + 1];

        Array.Copy(items, 0, targetArray, 0, placeIn);

        Array.Copy(items, placeIn, targetArray, placeIn + 1, items.Length - placeIn);

        targetArray[placeIn] = middleItem;

        var node = new CusomNode<int>(15);

        linkedList.AddAfter(targetNode, middleItem);

        LinkedListComparer(linkedList, targetArray);
    }

    [Fact]
    public void AddBefore_AddNull()
    {
        var list = new CustomLinkedList<int>();

        Assert.Throws<ArgumentNullException>(() => list.AddBefore(null, 10));
    }

    [Fact]
    public void AddBefore_Head()
    {
        var list = new CustomLinkedList<int>();

        list.Add(10);

        list.AddBefore(list.First, 1);

        Assert.Equal(1, list.First.Value);

        Assert.Equal(10, list.Last.Value);
    }

    [Theory]
    [InlineData(15, 5)]
    [InlineData(8, 4)]
    public void AddBefore_Middle(int size, int placeIn)
    {
        CusomNode<object> targetNode = null;

        var items = GetItems(size);

        var linkedList = new CustomLinkedList<object>();

        for (int i = 0; i < items.Length; i++)
        {
            linkedList.Add(items[i]);
            if (placeIn == i)
            {
                targetNode = linkedList.Last;
            }

        }

        var middleItem = GetItem();

        var targetArray = new object[items.Length + 1];

        Array.Copy(items, 0, targetArray, 0, placeIn);

        Array.Copy(items, placeIn, targetArray, placeIn + 1, items.Length - placeIn);

        targetArray[placeIn] = middleItem;


        linkedList.AddBefore(targetNode, middleItem);

        LinkedListComparer(linkedList, targetArray);
    }

    [Fact]
    public void RemoveFirst_EmptyList()
    {
        var linkedList = new CustomList<int>();

        linkedList.Remove(-1);

        Assert.Equal(0, linkedList.Count);
    }

}
