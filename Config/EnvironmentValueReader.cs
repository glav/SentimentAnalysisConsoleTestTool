using System;

namespace SentimentAnalysisConsoleTestTool.Config
{
    public class EnvironmentValueReader
    {
        public EnvironmentValueReader()
        {
        }

        public string GetEnvironmentValueThatIsNotEmpty(string[] environmentVariables, string defaultValue = null)
        {
            if (environmentVariables == null || environmentVariables.Length == 0)
            {
                return null;
            }
            foreach (var ev in environmentVariables)
            {
                var val = Environment.GetEnvironmentVariable(ev);
                if (!string.IsNullOrWhiteSpace(val))
                {
                    return val;
                }
            }
            return defaultValue;
        }

    }
}
