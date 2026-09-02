using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
    public static bool Verify(bool condition, string? message = null,
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0)
    {
        #if DEBUG
        if (!condition)
        {
            // a failure without a message used to log nothing at all, so it was invisible
            Log.Error(message ?? "Verify failed", file, line);
            if (Debugger.IsAttached) Debugger.Break();
        }
        #endif
        return condition;
    }

    /// <summary>
    /// Asserts in debug builds if <paramref name="value"/> is null. Returns true when non-null, narrowing
    /// nullability for the caller so no null-forgiving operator is needed:
    /// <code>if (!Verify(maybeNull)) return;  // maybeNull is non-null below</code>
    /// </summary>
    [DebuggerHidden, StackTraceHidden]
    public static bool Verify<T>([NotNullWhen(true)] T? value, string? message = null,
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0) where T : class
    {
        // passed through, so the error points at the caller rather than at this line
        return Verify(value != null, message, file, line);
    }
}