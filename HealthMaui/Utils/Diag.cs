using System;
using System.IO;

namespace HealthMaui.Utils
{
    internal static class Diag
    {
        private static readonly string _path = Path.Combine(Path.GetTempPath(), "healthmaui-diag.log");

        public static void Log(string message)
        {
            try
            {
                var line = $"[{DateTime.Now:O}] {message}" + Environment.NewLine;
                File.AppendAllText(_path, line);
            }
            catch
            {
                // best-effort logging
            }
        }

        public static string FilePath => _path;
    }
}
