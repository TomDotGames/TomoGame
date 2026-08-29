namespace TomoGame.Core.SceneGraph;

/// <summary>One node's place in a frame's draw order.</summary>
internal readonly struct DrawEntry(Node node, float zOrder, int index)
{
    public Node Node { get; } = node;

    /// <summary>The node's accumulated draw order.</summary>
    public float ZOrder { get; } = zOrder;

    /// <summary>Where the node fell in the graph walk, used to break ties.</summary>
    public int Index { get; } = index;
}

/// <summary>Orders by draw order, then by position in the graph walk. A total order, so the sort never has to
/// choose between equal entries.</summary>
internal sealed class DrawEntryComparer : IComparer<DrawEntry>
{
    public static readonly DrawEntryComparer Instance = new();

    public int Compare(DrawEntry a, DrawEntry b)
    {
        int byZOrder = a.ZOrder.CompareTo(b.ZOrder);
        return byZOrder != 0 ? byZOrder : a.Index.CompareTo(b.Index);
    }
}
