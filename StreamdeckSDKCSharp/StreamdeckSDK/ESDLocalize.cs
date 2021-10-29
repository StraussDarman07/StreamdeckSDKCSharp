using System.IO;
using System.Text.Json;

namespace Elgato.StreamdeckSDK
{
    public class ESDLocalize
    {
        public static void Initialize(string languageCode) => Initialize(languageCode, string.Empty);
        public static void Initialize(string languageCode, string subDir) => Instance ??= new ESDLocalize(languageCode, subDir);

        public static string GetLocalizedString(string defaultString) => Instance == null ? defaultString : Instance.GetLocalizedStringInternal(defaultString);

        private ESDLocalize(string languageCode, string subDir)
        {
            string pluginPath = ESDUtils.PluginPath();

            if (!string.IsNullOrWhiteSpace(subDir))
                pluginPath = Path.Combine(pluginPath, subDir);

            pluginPath = Path.Combine(pluginPath, $"{languageCode}.json");

            string localizeJson = File.ReadAllText(pluginPath);

            JsonDocument document = JsonDocument.Parse(localizeJson);

            LocalizationData = document.RootElement.GetProperty(LOCALIZATION_ROOT);
        }

        private string GetLocalizedStringInternal(string defaultString) => LocalizationData.GetProperty(defaultString).GetString();

        private const string LOCALIZATION_ROOT = "Localization";

        private JsonElement LocalizationData { get; }

        private static ESDLocalize Instance { get; set; }
    }
}
