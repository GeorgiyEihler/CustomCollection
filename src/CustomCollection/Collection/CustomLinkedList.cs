using System.Collections;

namespace CustomCollecion.Collection;

public class CustomLinkedList<T> : ICustomLinkedList<T>
{
    private int _count;

    private CusomNode<T>? _head;

    public CustomLinkedList()
    {

    }

    public CustomLinkedList(IEnumerable<T> collection)
    {
        if(collection is null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        foreach (T item in collection)
        {
            AddLast(item);
        }
    }

    public CusomNode<T>? Last => _head?.Previous;

    public CusomNode<T>? First => _head;

    public int Count => _count;

    public bool IsReadOnly => false;

    public void AddFirst(T value)
    {
        var node = new CusomNode<T>(this, value);
        if (_head is null)
        {
            InsertToNewList(node);
            return;
        }
        InsertBefore(_head, node);
        _head = node;
    }

    public void AddFirst(CusomNode<T> node)
    {
        ValidateNewNode(node);

        if (_head is null)
        {
            InsertToNewList(node);
            return;
        }
        InsertBefore(_head, node);
        _head = node;
    }

    public void AddLast(T value)
    {
        var node = new CusomNode<T>(this, value);
        if (_head is null)
        {
            InsertToNewList(node);
            return;
        }
        InsertBefore(_head, node);
    }

    public void AddLast(CusomNode<T> node)
    {
        ValidateNewNode(node);

        if (_head is null)
        {
            InsertToNewList(node);
            return;
        }
        InsertBefore(_head, node);
    }

    public void AddBefore(CusomNode<T> node, T value)
    {
        ValidateNode(node);

        var newNode = new CusomNode<T>(node.List!, value);

        InsertBefore(node, newNode);

        if (node == _head)
        {
            _head = newNode;
        }
    }

    public void AddBefore(CusomNode<T> node, CusomNode<T> newNode)
    {
        ValidateNode(node);
        ValidateNewNode(newNode);

        InsertBefore(node.Next!, newNode);

        newNode.List = this;

        if (node == _head)
        {
            _head = newNode;
        }
    }

    public void AddAfter(CusomNode<T> node, T value)
    {
        ValidateNode(node);

        var newNode = new CusomNode<T>(node.List!, value);

        InsertBefore(node.Next!, newNode);
    }

    public void AddAfter(CusomNode<T> node, CusomNode<T> newNode)
    {
        ValidateNode(node);
        ValidateNewNode(newNode);

        InsertBefore(node.Next!, newNode);
        newNode.List = this;
    }

    public void Add(T item) => AddLast(item);

    public void Clear()
    {
        var current = _head;

        while (current != null)
        {
            var temp = current;
            current = current.Next;
            temp.Clear();
        }

        _head = null;
        _count = 0;
    }

    public bool Contains(T item)
    {
        return Find(item) != null;
    }

    public bool Remove(T item)
    {
        var node = Find(item);
        if (node is not null)
        {
            RemoveNode(node);
            return true;
        }
        return false;
    }

    public CusomNode<T>? Find(T value)
    {
        var node = _head;

        EqualityComparer<T> c = EqualityComparer<T>.Default;

        if (node != null)
        {
            if (value != null)
            {
                do
                {
                    if (c.Equals(node!.Value, value))
                    {
                        return node;
                    }
                    node = node!.Next;
                } while (node != _head);
            }
            else
            {
                do
                {
                    if (node!.Value is null)
                    {
                        return node;
                    }
                    node = node.Next;
                } while (node != _head);
            }
        }
        return null;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        _ = array ?? throw new ArgumentNullException(nameof(array));

        if (arrayIndex < 0 || arrayIndex > array.Length || array.Length - arrayIndex < Count)
        {
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        }

        var node = _head;

        if (node is not null)
        {
            do
            {
                array[arrayIndex++] = node!.Value;
                node = node.Next;
            }
            while (node != _head);
        }
    }

    public IEnumerator<T> GetEnumerator() => new Enumerator(this);

    IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

    internal class Enumerator : IEnumerator<T>, IEnumerator
    {
        private readonly CustomLinkedList<T>? _list;
        private CusomNode<T>? _node;
        private T? _current;
        private int _index;

        internal Enumerator(CustomLinkedList<T> list)
        {
            _list = list;
            _node = list.First;
            _current = default;
            _index = 0;
        }

        public T Current => _current!;

        object? IEnumerator.Current
        {
            get
            {
                if (_index == 0 || (_index == _list!.Count + 1))
                {
                    throw new InvalidOperationException();
                }

                return Current;
            }
        }

        public bool MoveNext()
        {

            if (_node is null)
            {
                _index = _list!.Count + 1;
                return false;
            }

            ++_index;

            _current = _node.Value;

            _node = _node.Next;

            if (_node == _list!.First)
            {
                _node = null;
            }
            return true;
        }

        void IEnumerator.Reset()
        {
            _current = default;
            _node = _list!.First;
            _index = 0;
        }

        public void Dispose()
        {
        }

    }

    private void ValidateNode(CusomNode<T> node)
    {
        _ = node ?? throw new ArgumentNullException(nameof(node));

        if (node.List != this)
        {
            throw new InvalidOperationException("Node already attached");
        }
    }

    private void ValidateNewNode(CusomNode<T> node)
    {
        _ = node ?? throw new ArgumentNullException(nameof(node));

        if (node.List is not null)
        {
            throw new InvalidOperationException("Node already attached");
        }
    }

    private void InsertBefore(CusomNode<T> node, CusomNode<T> newNode)
    {
        newNode.Next = node;
        newNode.Previous = node.Previous;
        node.Previous!.Next = newNode;
        node.Previous = newNode;
        _count++;
    }

    private void InsertToNewList(CusomNode<T> newNode)
    {
        newNode.Next = newNode;
        newNode.Previous = newNode;
        _head = newNode;
        _count++;
    }

    private void RemoveNode(CusomNode<T> node)
    {
        if (node.Next == node)
        {
            _head = null;
        }
        else
        {
            node.Next!.Previous = node.Previous;
            node.Previous!.Next = node.Next;
            if (_head == node)
            {
                _head = node.Next;
            }
        }
        node.Clear();
        _count--;
    }
}

public sealed class CusomNode<T>
{
    private CustomLinkedList<T>? _list;
    private CusomNode<T>? _next;
    private CusomNode<T>? _prev;
    private T _value;

    public CusomNode(T value)
    {
        _value = value;
    }

    public CusomNode(CustomLinkedList<T> list, T value)
    {
        _value = value;
        _list = list;
    }

    public T Value => _value;

    public CusomNode<T>? Next
    {
        get => _next;
        internal set => _next = value;
    }

    public CusomNode<T>? Previous
    {
        get => _prev;
        internal set => _prev = value;
    }

    public CustomLinkedList<T>? List
    {
        get => _list;
        internal set => _list = value;
    }

    public void Clear()
    {
        _list = null;
        _next = null;
        _prev = null;
    }

}
