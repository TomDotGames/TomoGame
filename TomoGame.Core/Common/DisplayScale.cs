using System.Globalization;
using System.Runtime.InteropServices;

namespace TomoGame.Core;

/// <summary>Works out how much to scale the window by on a high density display, so a game asking for a
/// window of a given size gets one that is the expected physical size rather than a tiny one.
/// <para>There is no portable way to ask for this, so it takes the first answer it can get: an explicit
/// override, then the desktop's own environment variables, then the X resource that X11 desktops set for
/// exactly this purpose. Anything it cannot work out is treated as an unscaled display.</para></summary>
public static class DisplayScale
{
    /// <summary>Environment variable that overrides the detected scale outright.</summary>
    public const string OverrideVariable = "TOMOGAME_DISPLAY_SCALE";

    private const float MinScale = 1f;
    private const float MaxScale = 4f;
    private const float BaseDpi = 96f;

    private static float? _scale;

    /// <summary>The display scale, resolved once on first use.</summary>
    public static float Get()
    {
        _scale ??= Resolve();
        return _scale.Value;
    }

    private static float Resolve()
    {
        // an explicit override wins, so there is always a way out when the guesses below are wrong
        if (TryReadScaleVariable(OverrideVariable, out float overridden))
            return Clamp(overridden);

        // set by several Linux desktops for exactly this
        if (TryReadScaleVariable("GDK_SCALE", out float gdkScale))
            return Clamp(gdkScale);

        if (TryReadXftDpi(out float dpi))
            return Clamp(dpi / BaseDpi);

        return 1f;
    }

    private static float Clamp(float scale)
    {
        if (float.IsNaN(scale) || float.IsInfinity(scale))
            return 1f;

        return Math.Clamp(scale, MinScale, MaxScale);
    }

    private static bool TryReadScaleVariable(string name, out float scale)
    {
        string? value = Environment.GetEnvironmentVariable(name);
        return float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out scale) && scale > 0f;
    }

    /// <summary>Reads Xft.dpi from the X resource database, which is what X11 desktops - including XWayland -
    /// set to tell applications how much to scale themselves by.</summary>
    private static bool TryReadXftDpi(out float dpi)
    {
        dpi = 0f;
        if (!OperatingSystem.IsLinux() && !OperatingSystem.IsFreeBSD())
            return false;

        IntPtr display = IntPtr.Zero;
        try
        {
            display = XOpenDisplay(null);
            if (display == IntPtr.Zero)
                return false;

            string? resources = Marshal.PtrToStringAnsi(XResourceManagerString(display));
            if (resources == null)
                return false;

            foreach (string line in resources.Split('\n'))
            {
                if (!line.StartsWith("Xft.dpi:", StringComparison.Ordinal))
                    continue;

                string value = line["Xft.dpi:".Length..].Trim();
                return float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out dpi) && dpi > 0f;
            }

            return false;
        }
        catch (DllNotFoundException)
        {
            return false; // no X11 to ask, which is normal on a pure Wayland or console session
        }
        catch (EntryPointNotFoundException)
        {
            return false;
        }
        finally
        {
            if (display != IntPtr.Zero)
                XCloseDisplay(display);
        }
    }

    [DllImport("libX11.so.6")]
    private static extern IntPtr XOpenDisplay(string? name);

    [DllImport("libX11.so.6")]
    private static extern IntPtr XResourceManagerString(IntPtr display);

    [DllImport("libX11.so.6")]
    private static extern int XCloseDisplay(IntPtr display);
}
