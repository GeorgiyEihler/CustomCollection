using CustomCollecion.Collection;
using System.Text;

namespace CustomCollection.Test;

public class CustomQueueTest
{
    private static Random _random = new Random();

    /// <summary>
    /// Получить рандомный уникальный лист со значениями
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public static CustomQueue<object> GetQueue(int count)
    {
        var queue = new CustomQueue<object>();
        var baseQueue = new Queue<object>();

        for (var i = 0; i < count; i++)
        {
            queue.Enqueue(GetItem());
        }

        return queue;
    }
    private static object GetItem()
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(_random.Next(int.MinValue, int.MaxValue).ToString()));
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

    [Fact]
    public void Enqueue_AddNull()
    {
        var queue = new CustomQueue<object>();

        queue.Enqueue(null);

        Assert.Equal(1, queue.Count);

        Assert.Null(queue.Dequeue());
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    public void Enqueue_AddItem(int size)
    {
        var queue = GetQueue(size);

        var item = GetItem();
        
        queue.Enqueue(item);

        Assert.Equal(size + 1, queue.Count);

        for (int i = 0; i < size; i++)
        {
            queue.Dequeue();
        }

        Assert.Equal(item, queue.Dequeue());
    }

    [Fact]
    public void Dequeue_DequeueEmpty()
    {
        var queue = new CustomQueue<int>();

        Assert.Throws<NullReferenceException>(() => queue.Dequeue());
    }
    [Fact]
    public void TryDequeue_GetEmptyQueue()
    {
        var queue = new CustomQueue<int>();

        Assert.False(queue.TryDequeue(out _));
    }

    [Fact]
    public void TryDequeue_GetFIrstElement()
    {
        var queue = new CustomQueue<object>();

        var item = GetItem();

        queue.Enqueue(item);

        var idQuqueItem = queue.TryDequeue(out var ququeItem);

        Assert.True(idQuqueItem);

        Assert.Equal(item, ququeItem);
    }

    [Fact]
    public void TryDequeue_EnqueueDequeueTwice()
    {
        var queue = new CustomQueue<object>();

        var item = GetItem();
        var item2 = GetItem();

        queue.Enqueue(item);

        var idQuqueItem = queue.TryDequeue(out var ququeItem);

        Assert.True(idQuqueItem);

        Assert.Equal(item, ququeItem);

        queue.Enqueue(item2);

        idQuqueItem = queue.TryDequeue(out ququeItem);

        Assert.True(idQuqueItem);

        Assert.Equal(item2, ququeItem);
    }

    [Fact]
    public void TryDequeue_TryeDequeueAfterEmpty()
    {
        var queue = new CustomQueue<object>();

        var item = GetItem();
        var item2 = GetItem();

        queue.Enqueue(item);

        var idQuqueItem = queue.TryDequeue(out var ququeItem);

        Assert.True(idQuqueItem);

        Assert.Equal(item, ququeItem);

        idQuqueItem = queue.TryDequeue(out ququeItem);

        Assert.False(idQuqueItem);

    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    public void Peek_EnqueueItems(int size)
    {
        var items = GetItems(size);

        var quque = new CustomQueue<object>();

        foreach (var item in items)
        {
            quque.Enqueue(item);
        }

        foreach (var item in items)
        {
            var queueItem = quque.Peek();
            Assert.Equal(item, queueItem);
            _ = quque.Dequeue();
        }
    }

    [Fact]
    public void TryPeek_PeekEmpty()
    {
        var quque = new CustomQueue<object>();

        Assert.False(quque.TryPeek(out _));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    public void TryPeek_EnqueueItems(int size)
    {
        var items = GetItems(size);

        var quque = new CustomQueue<object>();

        foreach (var item in items)
        {
            quque.Enqueue(item);
        }

        foreach (var item in items)
        {
            Assert.True(quque.TryPeek(out var queueItem));

            Assert.Equal(item, queueItem);
            _ = quque.Dequeue();
        }
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    public void Contains_DequeueItems(int size)
    {
        var items = GetItems(size);

        var quque = new CustomQueue<object>();

        foreach (var item in items)
        {
            quque.Enqueue(item);
        }

        foreach (var item in items)
        {
            _ = quque.Dequeue();

            Assert.False(quque.Contains(item));
        }

    }

    [Fact]
    public void Contains_ContainsNull()
    {
        var quque = new CustomQueue<object>();

        quque.Enqueue(GetItem());

        Assert.False(quque.Contains(null));
        
    }

    [Fact]
    public void Contains_ContainsValue()
    {
        var quque = new CustomQueue<object>();

        var item = GetItem();

        quque.Enqueue(item);

        Assert.True(quque.Contains(item));
    }

    [Fact]
    public void Contains_ContainsValueAfterPeek()
    {
        var quque = new CustomQueue<object>();

        var item = GetItem();

        quque.Enqueue(item);

        _ = quque.Peek();

        Assert.True(quque.Contains(item));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    public void GetEnumerator_GetAllItems(int size)
    {
        var items = GetItems(size);

        var quque = new CustomQueue<object>();

        foreach (var item in items)
        {
            quque.Enqueue(item);
        }
        var itemIndex = 0;

        foreach (var item in quque)
        {
            Assert.Equal(items[itemIndex], item);
            itemIndex++;
        }
        Assert.Equal(size, itemIndex);
    }

    [Fact]
    public void GetEnumerator_GetEmpty()
    {

        var queue = new CustomQueue<object>();

        var countIteration = 0;

        foreach (var item in queue)
        {
            countIteration++;
        }

        Assert.Equal(0, countIteration);
    }

}
