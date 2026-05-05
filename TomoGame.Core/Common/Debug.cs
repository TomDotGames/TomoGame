using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace TomoGame.Core;

/// <summary>Debug assertion utilities.</summary>
public static class Dbg
{
    /// <summary>
    /// Asserts a condition in debug builds only. Logs an error and triggers a debug assertion if the condition is false.
    /// </summary>
    [DebuggerHidden, StackTraceHidden]
    [Conditional("DEBUG")]
    public static void Assert(bool condition,
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0)
    {
        if (!condition)
        {
            Log.Error("Assertion failed", file, line);
            if (Debugger.IsAttached) Debugger.Break();
        }
    }

    /// <summary>
    /// Asserts in debug builds if condition is not true. Returns the condition value in all builds.
    /// eg:
    /// if (!Verify(thingIExpectToBeTrue, "The thing is not true!"))
    ///     return;
    /// </summary>
    [DebuggerHidden, StackTraceHidden]
    public static bool Verify(bool condition, string? message = null)
    {
        #if DEBUG
        if (!condition)
        {
            if (message != null) Log.Error(message);
            if (Debugger.IsAttached) Debugger.Break();
        }
        #endif
        return condition;
    }
}