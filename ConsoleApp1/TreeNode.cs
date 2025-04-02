using System.Collections.ObjectModel;

namespace QuadtreeCompressor;

class TreeNode<T>
{
    private readonly T _value;
    private readonly List<TreeNode<T>> _children = new List<TreeNode<T>>();

    public TreeNode(T value)
    {
        _value = value;
    }

    public TreeNode<T> this[int i]  // children access by index
    {
        get { return _children[i]; }
    }

    public T Value { get { return _value; } }

    public ReadOnlyCollection<TreeNode<T>> Children
    {
        get { return _children.AsReadOnly(); }
    }

    public TreeNode<T> AddChild(T value)
    {
        var node = new TreeNode<T>(value);
        _children.Add(node);
        return node;
    }

    public bool RemoveChild(TreeNode<T> node)
    {
        return _children.Remove(node);
    }

    public void Traverse(Action<T> action)
    {
        action(Value);
        foreach (var child in _children){
            child.Traverse(action);
        }
    }

    public IEnumerable<T> Flatten()
    {
        return new[] {Value}.Concat(_children.SelectMany(x => x.Flatten()));
    }
}