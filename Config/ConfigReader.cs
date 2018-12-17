using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace SentimentAnalysisConsoleTestTool.Config
{
    public static class ConfigReader
    {
        public static IConfigurationRoot Configuration { get; set; }

        static ConfigReader()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            if (Environment.GetEnvironmentVariable("BuildConfiguration") == "development")
            {
                builder.AddUserSecrets<SentimentAnalysisConsoleTestTool.Program>();
            }

            Configuration = builder.Build();
        }

        public static string ApiKey
            => new string[]{
                "ApiKey",
                "TF_APIKEY"
            }.TryGetEnvironmentVariableElseUseConfig();

        public static string ApiLocation
            => new string[]{
                "ApiLocation",
                "TF_APILOCATION"
            }.TryGetEnvironmentVariableElseUseConfig();
    }
}
