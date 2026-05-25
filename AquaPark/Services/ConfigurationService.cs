using System;
using System.IO;
using System.Text.Json;

namespace AquaPark.Services
{
    public static class ConfigurationService
    {
        private const string DefaultConnectionString = "Server=MAXIM;Database=AquaPark;Trusted_Connection=True;TrustServerCertificate=True;";

        public static string GetConnectionString()
        {
            string configPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

            if (!File.Exists(configPath))
            {
                return DefaultConnectionString;
            }

            try
            {
                using JsonDocument document = JsonDocument.Parse(File.ReadAllText(configPath));

                if (document.RootElement.TryGetProperty("ConnectionStrings", out JsonElement connectionStrings)
                    && connectionStrings.TryGetProperty("AquaPark", out JsonElement aquaParkConnection)
                    && !string.IsNullOrWhiteSpace(aquaParkConnection.GetString()))
                {
                    return aquaParkConnection.GetString()!;
                }
            }
            catch
            {
                return DefaultConnectionString;
            }

            return DefaultConnectionString;
        }
    }
}
