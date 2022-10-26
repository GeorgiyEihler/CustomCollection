using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;

namespace CustomCollecion.Collection;

public class CustomList<T> : ICustomList<T>
{
    private T[] _source;
    private int _size;
    private int _capacity;
    private int _baseCapacity = 8;

    private T[] _emptySource => new T[0];

    public CustomList()
    {
        CustomListInit(0);
    }

    public CustomList(IEnumerable<T> source)
    {
        if (source is null)
        {
            throw new ArgumentNullException("source");
        }
        CustomListInit(source.Count(), source);
    }

    public CustomList(int size)
    {
        CustomListInit(size);
    }


    public T this[int index]
    {
        get
        {
            if (index >= _size || index < 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            return _source[index];

        }
        set
        {
            if (index >= _size || index < 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            _source[index] = value;

        }
    }

    public int Count => _size;

    public bool IsReadOnly => false;

    public void Add(T item)
    {
        if (_size >= _capacity)
        {
            _source = WithResize();
        }

        _source[_size] = item;

        _size++;
    }

    public void Clear()
    {
        _size = 0;
        _source = new T[_baseCapacity];
        _capacity = _baseCapacity;
    }

    public bool Contains(T item)
    {
        return _source.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex) =>
        Array.Copy(_source, 0, array, arrayIndex, _size);

    public int IndexOf(T item) =>
        Array.IndexOf(_source, item);

    public void Insert(int index, T item)
    {
        if (index > _size)
        {
            throw new IndexOutOfRangeException("index");
        }

        if (_size == _source.Length)
        {
            _source = WithResize();
        }

        if (index < _size)
        {
            Array.Copy(_source, index, _source, index + 1, _size - index);
        }

        _source[index] = item;

        _size++;
    }

    public bool Remove(T item)
    {
        int index = IndexOf(item);

        if (index >= 0)
        {
            RemoveAt(index);
            return true;
        }

        return false;
    }

    public void RemoveAt(int index)
    {
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

    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _source.AsEnumerable().GetEnumerator();
    }

    public IEnumerator<T> GetEnumerator()
    {
        return ((IEnumerable<T>)_source).GetEnumerator(); ;
    }

    private void CustomListInit(int capacity, IEnumerable<T> source = null)
    {
        if (capacity < 0)
        {
            throw new ArgumentException("size");
        }

        if (capacity == 0)
        {
            _source = new T[_baseCapacity];
            _capacity = _baseCapacity;
            _size = 0;

        }

        if (capacity > 0)
        {
            _source = new T[capacity * 2];
            _capacity = capacity * 2;

            if (source is not null)
            {
                _size = source!.Count();
                Array.Copy(source!.ToArray(), _source, _size);
                return;

            }
            _size = capacity;
            return;
        }
    }

    private T[] Resize()
    {
        var newCapacity = _capacity * 2;

        var newSourse = new T[newCapacity];

        Array.Copy(_source, newSourse, _capacity);

        _capacity = newCapacity;

        return newSourse;
    }

    private T[] WithResize()
    {
        var newSourse = Resize();

        return newSourse;
    }


}

