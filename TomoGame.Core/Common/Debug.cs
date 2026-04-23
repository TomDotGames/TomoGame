using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace TomoGame.Core;

/// <summary>Debug assertion utilities.</summary>
public static class Dbg
{
    /// <summary>
    /// Asserts a condition in debug builds only. Logs an error and triggers a debug assertion if the condition is false.
    /// </summary>
    [Conditional("DEBUG")]
    public static void Assert(bool condition,
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0)
    {
        if (!condition)
            Log.Error("Assertion failed", file, line);
        Debug.Assert(condition);
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