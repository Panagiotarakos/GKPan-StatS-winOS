using System;
using System.IO;

namespace GKPanStats
{
    public static class Settings
    {
        private static string FilePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "GKPanStats", "settings.txt");

        public static void Save(string module)
        {
            try
            {
                var dir = Path.GetDirectoryName(FilePath)!;
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                File.WriteAllText(FilePath, module);
            }
            catch { }
        }

        public static string Load()
        {
            try
            {
                if (File.Exists(FilePath))
                    return File.ReadAllText(FilePath).Trim();
            }
            catch { }
            return "CPU";
        }
    }
}
