using System.IO;

namespace Elgato.StreamdeckSDK
{
    public static class ESDUtils
    {
        public static string PluginPath()
        {
            FileInfo file = new FileInfo(System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName ?? string.Empty);

            if (!file.Exists) throw new FileNotFoundException("Current running executable not found!");

            return file.Directory?.FullName;

        }
    }
}
