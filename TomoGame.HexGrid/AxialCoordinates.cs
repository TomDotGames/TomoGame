namespace TomoGame.HexGrid;

/// <summary>Axial (q, r) coordinates of a pointy-topped hex.</summary>
public readonly struct AxialCoordinates : IEquatable<AxialCoordinates>
{
    /// <summary>The six neighbour directions, in the order <see cref="EDirection"/> declares them.</summary>
    public enum EDirection
    {
        W,
        NW,
        NE,
        E,
        SE,
        SW
    }

    private static readonly (int Q, int R)[] DirectionOffsets =
    [
        (-1, 0),
        (0, -1),
        (1, -1),
        (1, 0),
        (0, 1),
        (-1, 1),
    ];

    public readonly int Q;
    public readonly int R;

    public AxialCoordinates(int q, int r)
    {
        Q = q;
        R = r;
    }

    /// <summary>The direction facing back the way you came.</summary>
    public static EDirection GetOppositeDirection(EDirection direction)
    {
        return (EDirection)(((int)direction + 3) % 6);
    }

    /// <summary>The six surrounding coordinates, paired with the direction each lies in.</summary>
    public IEnumerable<(AxialCoordinates, EDirection)> GetNeighbors()
    {
        for (int i = 0; i < DirectionOffsets.Length; ++i)
        {
            yield return (new AxialCoordinates(Q + DirectionOffsets[i].Q, R + DirectionOffsets[i].R), (EDirection)i);
        }
    }

    // Without IEquatable a Dictionary keyed on this falls back to ValueType's reflection-based comparison,
    // which the grid pays for on every cell and every neighbour lookup.
    public bool Equals(AxialCoordinates other) => Q == other.Q && R == other.R;

    public override bool Equals(object? obj) => obj is AxialCoordinates other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Q, R);

    public static bool operator ==(AxialCoordinates a, AxialCoordinates b) => a.Equals(b);

    public static bool operator !=(AxialCoordinates a, AxialCoordinates b) => !a.Equals(b);

    public override string ToString() => $"({Q}, {R})";
}
