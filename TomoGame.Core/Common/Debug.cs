using System.Diagnostics;

namespace TomoGame.Core;

/// <summary>Debug assertion utilities.</summary>
public static class Dbg
{
    /// <summary>
    /// Asserts a condition in debug builds only. Wraps the system debug, and is duplicated here for consistency.
    /// </summary>
    [Conditional("DEBUG")]
    public static void Assert(bool condition, string? message = null)
    {
        Debug.Assert(condition, message);
    }

    /// <summary>
    /// Asserts in debug builds if condition is not true. Returns the condition value in all builds.
    /// eg:
    /// if (!Verify(thingIExpectToBeTrue, "The thing is not true!"))
    ///     return;
    /// </summary>
    public static bool Verify(bool condition, string? message = null)
    {
        #if DEBUG
        Debug.Assert(condition, message);
        #endif
        return condition;
    }
}