using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

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
            SafeBreak();
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
            SafeBreak();
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

    /// <summary>
    /// Breaks into the debugger, first releasing any mouse or keyboard grab SDL holds. SDL grabs the pointer
    /// while a button is down, so breaking mid-click used to leave the whole desktop unresponsive.
    /// </summary>
    [DebuggerHidden, StackTraceHidden]
    private static void SafeBreak()
    {
        if (!Debugger.IsAttached)
            return;

        try
        {
            ReleaseInput();
        }
        catch
        {
            // releasing input is best effort; never let it stop the break itself
        }

        Debugger.Break();
    }

    private delegate int SdlIntFn(int value);
    private delegate IntPtr SdlGetWindowFn();
    private delegate void SdlSetWindowGrabFn(IntPtr window, int grabbed);
    private delegate int SdlSetHintWithPriorityFn([MarshalAs(UnmanagedType.LPUTF8Str)] string name,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string value, int priority);

    private const string AutoCaptureHint = "SDL_MOUSE_AUTO_CAPTURE";
    private const int SdlHintOverride = 2;

    private static IntPtr _sdl;

    private static void ReleaseInput()
    {
        if (_sdl == IntPtr.Zero && !TryLoadSdl(out _sdl))
            return;

        // each export is optional, as older SDL builds lack some of them
        if (TryGetFn(_sdl, "SDL_GetGrabbedWindow", out SdlGetWindowFn? getGrabbedWindow)
            && TryGetFn(_sdl, "SDL_SetWindowGrab", out SdlSetWindowGrabFn? setWindowGrab))
        {
            IntPtr window = getGrabbedWindow();
            if (window != IntPtr.Zero)
                setWindowGrab(window, 0);
        }

        if (TryGetFn(_sdl, "SDL_SetRelativeMouseMode", out SdlIntFn? setRelativeMouseMode))
            setRelativeMouseMode(0);
        // SDL grabs the pointer while a mouse button is held, and SDL_CaptureMouse(false) keeps that grab for as
        // long as the button stays down. Turning the hint off is what makes SDL ungrab, and it does so at once,
        // without needing the event loop the break is about to freeze. It is left off: restoring it after the break
        // regrabbed the pointer as soon as you stepped, since SDL still believes the button is down.
        if (TryGetFn(_sdl, "SDL_SetHintWithPriority", out SdlSetHintWithPriorityFn? setHint))
            setHint(AutoCaptureHint, "0", SdlHintOverride);
        if (TryGetFn(_sdl, "SDL_ShowCursor", out SdlIntFn? showCursor))
            showCursor(1);
    }

    private static bool TryLoadSdl(out IntPtr handle)
    {
        // MonoGame has already loaded SDL, so these resolve to the same library rather than a second copy
        string[] names = ["libSDL2-2.0.so.0", "libSDL2-2.0.0.dylib", "libSDL2.dylib", "SDL2.dll", "SDL2"];
        foreach (string name in names)
        {
            if (NativeLibrary.TryLoad(name, typeof(Game).Assembly, null, out handle))
                return true;
        }
        handle = IntPtr.Zero;
        return false;
    }

    private static bool TryGetFn<T>(IntPtr library, string name, [NotNullWhen(true)] out T? fn) where T : Delegate
    {
        fn = NativeLibrary.TryGetExport(library, name, out IntPtr address)
            ? Marshal.GetDelegateForFunctionPointer<T>(address)
            : null;
        return fn != null;
    }
}
