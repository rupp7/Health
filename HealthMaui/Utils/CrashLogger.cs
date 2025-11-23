using System.Diagnostics;

namespace HealthMaui.Utils;

public static class CrashLogger
{
    private static string LogDirectory => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        "Downloads");

    public static void LogError(string source, Exception ex)
    {
        try
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            var filename = $"maui-crash-{source}-{timestamp}.txt";
            var path = Path.Combine(LogDirectory, filename);

            var log = $"""
                Crash Report
                ===========
                Time: {DateTime.Now}
                Source: {source}
                Exception Type: {ex.GetType().FullName}
                Message: {ex.Message}
                Stack Trace:
                {ex.StackTrace}

                Inner Exception:
                {ex.InnerException}
                """;

            File.WriteAllText(path, log);
            Debug.WriteLine($"Crash log written to: {path}");
            Debug.WriteLine($"Exception: {ex}");
        }
        catch (Exception logEx)
        {
            Debug.WriteLine($"Failed to write crash log: {logEx}");
        }
    }
}