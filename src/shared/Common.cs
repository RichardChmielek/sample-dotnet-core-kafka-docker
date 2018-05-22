using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Shared
{
    public static class Common
    {
        static Common()
        {
            var environment = GetEnvironment();
            Configuration = BuildConfiguration(environment);
        }
        private static IConfiguration Configuration { get; set; }
        public static IConfiguration BuildConfiguration(string environment)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("config.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"config.{environment}.json", optional: true, reloadOnChange: true);
            return builder.Build();
        }

        public static string GetConfigValue(string settingName)
        {
            var environmentSettingName = settingName.ToUpper().Replace('.', '_');
            var environmentSetting = Environment.GetEnvironmentVariable(environmentSettingName);

            return String.IsNullOrWhiteSpace(environmentSetting) ? Configuration[settingName] : environmentSetting;
        }

        private static string GetEnvironment()
        {
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (String.IsNullOrWhiteSpace(environment))
            {
                environment = "development";
            }
            Console.WriteLine("Environment: {0}", environment);
            return environment;
        }
    }
}