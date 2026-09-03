using System.Diagnostics;

namespace TomoGame.HexGrid;

/// <summary>A cell in a <see cref="HexGrid{T}"/>. Derive from it to hang game data off a cell.</summary>
public class HexCell
{
    public AxialCoordinates Coords;

    /// <summary>Neighbouring cells indexed by <see cref="AxialCoordinates.EDirection"/>; null off the edge.</summary>
    public HexCell?[] Neighbors = new HexCell?[6];
}

/// <summary>A hexagonal grid of cells filling the given radius around the origin.</summary>
public class HexGrid<T> where T : HexCell, new()
{
    private readonly Dictionary<AxialCoordinates, T> _cells;
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
}
