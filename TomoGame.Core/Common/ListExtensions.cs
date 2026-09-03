namespace TomoGame.Core;

/// <summary>List helpers.</summary>
public static class ListExtensions
{
    /// <summary>Shuffles in place from a caller-supplied source, so the caller decides whether it repeats.</summary>
    public static void Shuffle<T>(this IList<T> list, Random random)
    {
        // Fisher-Yates. The original picked its swap partner from the whole list rather than the unshuffled
        // remainder, which biases the result.
        for (int i = 0; i < list.Count; ++i)
        {
            int swap = random.Next(i, list.Count);
            (list[i], list[swap]) = (list[swap], list[i]);
        }
    }
}
