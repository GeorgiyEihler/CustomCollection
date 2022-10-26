using System.Collections;

namespace CustomCollecion.Collection;

public class CustomQueue<T> : ICollection<T>
{

    private T[] _source;

    private readonly int _baseCapacity = 4;

    private int _tail;
    private int _size;
    private int _head;

    public CustomQueue()
    {
        _source = new T[_baseCapacity];
    }


    public int Count => _size;

    public bool IsReadOnly => false;

    public void Add(T item)
    {
        throw new NotImplementedException();
    }

    public void Clear()
    {
        _source = new T[_baseCapacity];
        _head = 0;
        _tail = 0;
    }

    public void Enqueue(T item)
    {
        if (_size == _source.Length)
        {
            Grow();
        }

        _source[_tail] = item;

        MoveNext(ref _tail);

        _size++;
    }

    public T Dequeue()
    {
        int head = _head;
        T[] source = _source;

        if (_size == 0)
        {
            throw new NullReferenceException();
        }

        T removed = source[head];

        _source[head] = default;

        MoveNext(ref _head);

        _size--;

        return removed;
    }

    public bool TryDequeue(out T? result)
    {

        int head = _head;
        T[] source = _source;

        if (_size == 0)
        {
            result = default;
            return false;
        }

        if (_size == 0)
        {
            throw new NullReferenceException();
        }

        T removed = source[head];

        _source[head] = default;

        MoveNext(ref _head);

        _size--;

        result = removed;

        return true;
    }

    public T Peek()
    {
        if (_size == 0)
        {
            throw new NullReferenceException();
        }

        return _source[_head];
    }

    public bool TryPeek(out T? result)
    {
        if (_size == 0)
        {
            result = default;
            return false;
        }

        result = _source[_head];
        return true;
    }

    public bool Contains(T item)
    {
        if (_size == 0)
        {
            return false;
        }

        return Array.IndexOf(_source, item, _head, _source.Length - _head) >= 0;
    }

    public bool Remove(T item)
    {
        if (!Contains(item))
        {
            return false;
        }

        var index = Array.IndexOf(_source, item, _head, _source.Length - _head);

        if (index >= _size)
        {
            throw new IndexOutOfRangeException("index");
        }

        _size--;

        if (index < _size)
        {
            Array.Copy(_source, index + 1, _source, index, _size - index);
        }

        if (default(T) is not null)
        {
            _source[_size] = default!;
        }

        return true;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        _ = _source ?? throw new NullReferenceException();

        if (_size == 0) return;

        if (array.Rank != 1) throw new ArgumentException(nameof(array));

        var length = array.Length;

        if (arrayIndex < 0 || arrayIndex > length || length - arrayIndex < _size)
        {
            throw new ArgumentException(nameof(arrayIndex));
        }

        Array.Copy(_source, _head, array, arrayIndex, _size);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return new Enumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return new Enumerator(this);
    }

    private void MoveNext(ref int index) =>
        index = index + 1 == _source.Length ? 0 : index + 1;

    private void Grow()
    {

        int newcapacity = 2 * _source.Length;

        T[] newarray = new T[newcapacity];

        if (_size > 0)
        {
            if (_head < _tail)
            {
                Array.Copy(_source, _head, newarray, 0, _size);
            }
            else
            {
                Array.Copy(_source, _head, newarray, 0, _source.Length - _head);
                Array.Copy(_source, 0, newarray, _source.Length - _head, _tail);
            }
        }

        _source = newarray;
        _head = 0;
        _tail = (_size == newcapacity) ? 0 : _size;

    }

    internal class Enumerator : IEnumerator<T>
    {
        private readonly CustomQueue<T> _queue;
        private int _index;
        private int _queueIndex;
        private T? _currentElement;

        internal Enumerator(CustomQueue<T> queue)
        {
            _queue = queue;
            _index = -1;
            _currentElement = default;
        }

        public void Dispose()
        {
            _index = -2;
            _currentElement = default;
        }

        public bool MoveNext()
        {
            if (_index == -2)
                return false;

            _index++;

            if (_index == _queue._size)
            {
                _index = -2;
                _currentElement = default;
                return false;
            }

            _queueIndex = _queue._head + _index;

            _currentElement = _queue._source[_queueIndex];

            return true;
        }

        public T Current
        {
            get
            {
                if (_index < 0)
                    throw new ArgumentException(nameof(_index));
                return _currentElement!;
            }
        }

        object? IEnumerator.Current
        {
            get { return Current; }
        }

        void IEnumerator.Reset()
        {
            _index = -1;
            _currentElement = default;
        }
    }

}
