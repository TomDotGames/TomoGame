using System.Runtime.CompilerServices;

namespace TomoGame.Core;

/// <summary>Static logging utilities with colored console output and optional file output.</summary>
public static class Log
{
    private static StreamWriter? _logFile;

    /// <summary>Logs all output to a file at the given path.</summary>
    public static void InitOutputFile(string path)
    {
        _logFile = new StreamWriter(path, append: true) { AutoFlush = true };
    }

    /// <summary>Logs an informational message.</summary>
    public static void Info(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
    {
        string formatted = $"[{Path.GetFileName(file)}:{line}] {message}";
        Console.WriteLine(formatted);
        _logFile?.WriteLine(formatted);
    }

    /// <summary>Logs a warning message. Outputs in yellow.</summary>
    public static void Warning(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
    {
        string formatted = $"[{Path.GetFileName(file)}:{line}] {message}";
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(formatted);
        Console.ResetColor();
        _logFile?.WriteLine($"[WARN] {formatted}");
    }

    /// <summary>Logs an error message. Outputs in red.</summary>
    public static void Error(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
    {
        string formatted = $"[{Path.GetFileName(file)}:{line}] {message}";
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(formatted);
        Console.ResetColor();
        _logFile?.WriteLine($"[ERROR] {formatted}");
    }
}
