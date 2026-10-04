using System.Diagnostics;

namespace TomoGame.HexGrid;

/// <summary>A cell in a <see cref="HexGrid{T}"/>. Derive from it to hang game data off a cell.</summary>
public class HexCell
{
    public AxialCoordinates Coords;

    /// <summary>Neighbouring cells indexed by <see cref="AxialCoordinates.EDirection"/>; null off the edge.</summary>
    public HexCell?[] Neighbors = new HexCell?[6];
}

/// <summary>A vertex of a <see cref="HexGrid{T}"/>, where up to three cells meet.</summary>
public class HexNode
{
    /// <summary>The cells touching this vertex; null for one or two of them at the edge of the grid.</summary>
    public HexCell?[] Hexes = new HexCell?[3];

    /// <summary>
    /// The coordinates of the three cells in <see cref="Hexes"/>, index-aligned with it. Unlike
    /// <see cref="Hexes"/>, always populated — a hex missing off the edge of the grid still has a
    /// coordinate, just no cell there.
    /// </summary>
    public AxialCoordinates[] Coords = new AxialCoordinates[3];

    /// <summary>True if this vertex is missing one or more of its three hexes, i.e. it sits on the grid's boundary.</summary>
    public bool IsOnEdge => Array.Exists(Hexes, hex => hex == null);
}

/// <summary>An edge of a <see cref="HexGrid{T}"/>, shared by up to two cells and bounded by two nodes.</summary>
public class HexEdge
{
    /// <summary>The three angles an edge of a pointy-topped hex can run at, as drawn on screen.</summary>
    public enum EOrientation
    {
        /// <summary>"|" — between W and E neighbours.</summary>
        Vertical,
        /// <summary>"/" — between NW and SE neighbours.</summary>
        Rising,
        /// <summary>"\" — between NE and SW neighbours.</summary>
        Falling
    }

    /// <summary>The angle this edge runs at.</summary>
    public EOrientation Orientation;

    /// <summary>The cells this edge borders; null on the side that falls off the edge of the grid.</summary>
    public HexCell?[] Hexes = new HexCell?[2];

    /// <summary>The coordinates of the two cells in <see cref="Hexes"/>, index-aligned with it, always populated.</summary>
    public AxialCoordinates[] Coords = new AxialCoordinates[2];

    /// <summary>The two vertices at the ends of this edge. Always present, unlike <see cref="Hexes"/>.</summary>
    public HexNode[] Nodes = new HexNode[2];

    /// <summary>True if this edge is missing one of its two hexes, i.e. it sits on the grid's boundary.</summary>
    public bool IsOnEdge => Array.Exists(Hexes, hex => hex == null);
}

/// <summary>A hexagonal grid of cells filling the given radius around the origin.</summary>
public class HexGrid<T> where T : HexCell, new()
{
    private readonly record struct NodeKey((int Q, int R) A, (int Q, int R) B, (int Q, int R) C);

    private readonly record struct EdgeKey((int Q, int R) A, (int Q, int R) B);

    private readonly Dictionary<AxialCoordinates, T> _cells;
    private readonly List<HexNode> _nodes;
    private readonly List<HexEdge> _edges;
    private readonly int _radius;

    public int Radius => _radius;

    public int Count => _cells.Count;

    public HexGrid(int radius)
    {
        _radius = radius;
        _cells = new Dictionary<AxialCoordinates, T>(1 + 3 * radius * (radius + 1));

        foreach (AxialCoordinates coords in GetCoords())
        {
            _cells.Add(coords, new T { Coords = coords });
        }

        foreach (T cell in GetCells())
        {
            foreach ((T neighbor, AxialCoordinates.EDirection direction) in GetNeighbors(cell))
            {
                Debug.Assert(cell.Neighbors[(int)direction] == null);
                cell.Neighbors[(int)direction] = neighbor;
            }
        }

        Dictionary<NodeKey, HexNode> nodesByKey = BuildNodes();
        _nodes = nodesByKey.Values.ToList();
        _edges = BuildEdges(nodesByKey);
    }

    /// <summary>Every coordinate inside the radius.</summary>
    public IEnumerable<AxialCoordinates> GetCoords()
    {
        for (int q = -_radius; q <= _radius; ++q)
        {
            int rMin = Math.Max(-_radius, -q - _radius);
            int rMax = Math.Min(_radius, -q + _radius);

            for (int r = rMin; r <= rMax; ++r)
            {
                yield return new AxialCoordinates(q, r);
            }
        }
    }

    public IEnumerable<T> GetCells() => _cells.Values;

    public T? GetCell(AxialCoordinates coords) => _cells.GetValueOrDefault(coords);

    public IEnumerable<HexNode> GetNodes() => _nodes;

    public IEnumerable<HexEdge> GetEdges() => _edges;

    /// <summary>The cell's existing neighbours, paired with their direction. Skips those off the edge.</summary>
    public IEnumerable<(T, AxialCoordinates.EDirection)> GetNeighbors(T cell)
    {
        foreach ((AxialCoordinates coords, AxialCoordinates.EDirection direction) in cell.Coords.GetNeighbors())
        {
            if (_cells.TryGetValue(coords, out T? neighbor))
            {
                yield return (neighbor, direction);
            }
        }
    }

    private static AxialCoordinates[] GetNeighborCoords(AxialCoordinates coords) =>
        coords.GetNeighbors().Select(n => n.Item1).ToArray();

    private static NodeKey MakeNodeKey(AxialCoordinates a, AxialCoordinates b, AxialCoordinates c)
    {
        var sorted = new[] { (a.Q, a.R), (b.Q, b.R), (c.Q, c.R) };
        Array.Sort(sorted);
        return new NodeKey(sorted[0], sorted[1], sorted[2]);
    }

    private static EdgeKey MakeEdgeKey(AxialCoordinates a, AxialCoordinates b)
    {
        var sorted = new[] { (a.Q, a.R), (b.Q, b.R) };
        Array.Sort(sorted);
        return new EdgeKey(sorted[0], sorted[1]);
    }

    /// <summary>
    /// Every vertex touches three mutually-adjacent hexes; walking each cell's corners visits
    /// every vertex three times (once, twice at the edge of the grid), so dedupe by the sorted
    /// coordinates rather than the cells themselves — two cells missing at the edge are still null,
    /// but the coordinates they'd have occupied differ, keeping the two vertices distinct.
    /// </summary>
    private Dictionary<NodeKey, HexNode> BuildNodes()
    {
        var nodes = new Dictionary<NodeKey, HexNode>();

        foreach (T cell in GetCells())
        {
            AxialCoordinates[] neighbors = GetNeighborCoords(cell.Coords);

            for (int i = 0; i < neighbors.Length; ++i)
            {
                AxialCoordinates a = cell.Coords;
                AxialCoordinates b = neighbors[i];
                AxialCoordinates c = neighbors[(i + 1) % neighbors.Length];

                NodeKey key = MakeNodeKey(a, b, c);
                if (nodes.ContainsKey(key))
                {
                    continue;
                }

                nodes.Add(key, new HexNode
                {
                    Hexes = [_cells.GetValueOrDefault(a), _cells.GetValueOrDefault(b), _cells.GetValueOrDefault(c)],
                    Coords = [a, b, c]
                });
            }
        }

        return nodes;
    }

    /// <summary>Every edge is shared by (at most) two cells, so dedupe by the unordered pair of coordinates.</summary>
    private List<HexEdge> BuildEdges(Dictionary<NodeKey, HexNode> nodesByKey)
    {
        var edges = new Dictionary<EdgeKey, HexEdge>();

        foreach (T cell in GetCells())
        {
            AxialCoordinates[] neighbors = GetNeighborCoords(cell.Coords);

            for (int i = 0; i < neighbors.Length; ++i)
            {
                AxialCoordinates a = cell.Coords;
                AxialCoordinates b = neighbors[i];

                EdgeKey key = MakeEdgeKey(a, b);
                if (edges.ContainsKey(key))
                {
                    continue;
                }

                AxialCoordinates prev = neighbors[(i - 1 + neighbors.Length) % neighbors.Length];
                AxialCoordinates next = neighbors[(i + 1) % neighbors.Length];

                edges.Add(key, new HexEdge
                {
                    // neighbors[i] lies in EDirection i, and opposite directions are 3 apart, so i % 3 pairs them up
                    Orientation = (HexEdge.EOrientation)(i % 3),
                    Hexes = [_cells.GetValueOrDefault(a), _cells.GetValueOrDefault(b)],
                    Coords = [a, b],
                    Nodes = [nodesByKey[MakeNodeKey(a, prev, b)], nodesByKey[MakeNodeKey(a, b, next)]]
                });
            }
        }

        return edges.Values.ToList();
    }
}
