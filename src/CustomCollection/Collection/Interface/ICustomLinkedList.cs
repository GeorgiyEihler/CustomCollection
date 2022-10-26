namespace CustomCollecion.Collection;

public interface ICustomLinkedList<T> : ICollection<T>
{
    CusomNode<T>? Last { get; }
    CusomNode<T>? First { get; }
    void AddFirst(T value);
    void AddFirst(CusomNode<T> node);
    void AddLast(T value);
    void AddLast(CusomNode<T> node);
    void AddBefore(CusomNode<T> node, T value);
    void AddBefore(CusomNode<T> node, CusomNode<T> newNode);
    void AddAfter(CusomNode<T> node, T value);
    void AddAfter(CusomNode<T> node, CusomNode<T> newNode);
    CusomNode<T>? Find(T value);

}
