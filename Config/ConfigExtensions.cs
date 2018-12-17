namespace SentimentAnalysisConsoleTestTool.Config
{
    public static class ConfigExtensions
    {
        static ConfigExtensions()
        {
        }
        public static string TryGetEnvironmentVariableElseUseConfig(this string[] environmentVariables)
        {
            var reader = new EnvironmentValueReader();
            var val = reader.GetEnvironmentValueThatIsNotEmpty(environmentVariables);
            if (!string.IsNullOrWhiteSpace(val))
            {
                return val;
            }

            foreach (var ev in environmentVariables)
            {
                val = ConfigReader.Configuration[ev];
                if (!string.IsNullOrWhiteSpace(val))
                {
                    return val;
                }
            }
            return null;
        }

    }
}
