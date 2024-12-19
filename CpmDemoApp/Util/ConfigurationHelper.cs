using CpmDemoApp.Models;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace CpmDemoApp.Util
{
    public static class ConfigurationHelper
    {
        private static IConfigurationRoot configuration;

        static ConfigurationHelper()
        {
            var path = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
            var path2 = Directory.GetCurrentDirectory();
            var builder = new ConfigurationBuilder()
                .SetBasePath(path2)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            configuration = builder.Build();
        }

        public static string GetSetting(string key)
        {
            return configuration[key];
        }
        public static ClientOptions GetClientOptions()
        {
            var settings = new ClientOptions();
            configuration.GetSection("ClientOptions").Bind(settings);
            return settings;
        }
    }
}

