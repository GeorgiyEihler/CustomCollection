namespace CustomCollecion.Collection;

public interface ICustomQueue<T> : IEnumerable<T>
{
    void Enqueue(T item);
    T Dequeue();
    bool TryDequeue(out T? result);
    T Peek();
    bool TryPeek(out T? result);
}
