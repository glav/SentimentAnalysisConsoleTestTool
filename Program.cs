using Glav.CognitiveServices.FluentApi.Core;
using Glav.CognitiveServices.FluentApi.TextAnalytic;
using System;
using System.Linq;

namespace SentimentAnalysisConsoleTestTool
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Sentiment Analysis CLI tool.");
            Console.WriteLine("API Location: {0}", Config.ConfigReader.ApiLocation);
            Console.WriteLine("API Key: {0}", string.IsNullOrWhiteSpace(Config.ConfigReader.ApiKey) ? "Not set!" : "Set");
            var cliArguments = new CliArguments(args);

            if (cliArguments.ArgumentType == ArgType.Invalid)
            {
                Console.WriteLine("\nFormat: \n\tSentimentAnalysisConsoleTestTool -f {filename.txt}");
                Console.WriteLine("\tSentimentAnalysisConsoleTestTool -t \"Text to analyse\"");

                return;
            }

            string textToAnalyse;
            if (cliArguments.ArgumentType == ArgType.ManualText)
            {
                textToAnalyse = cliArguments.TextToAnalyse;
            }
            else
            {
                textToAnalyse = System.IO.File.ReadAllText(cliArguments.Filename);
            }

            var location = (LocationKeyIdentifier)System.Enum.Parse(typeof(LocationKeyIdentifier), Config.ConfigReader.ApiLocation, true);

            try
            {
                var result = TextAnalyticConfigurationSettings.CreateUsingConfigurationKeys(Config.ConfigReader.ApiKey, location)
                    .AddConsoleDiagnosticLogging()
                    .UsingHttpCommunication()
                    .WithTextAnalyticAnalysisActions()
                    .AddSentimentAnalysisSplitIntoSentences(textToAnalyse)
                    .AddKeyPhraseAnalysisSplitIntoSentences(textToAnalyse)
                    .AnalyseAllAsync().Result;

                if (result.SentimentAnalysis.AnalysisResult.ActionSubmittedSuccessfully)
                {
                    var errors = result.SentimentAnalysis.AnalysisResults
                        .Where(r => r.ResponseData.errors != null && r.ResponseData.errors.Length > 0)
                        .SelectMany(s => s.ResponseData.errors)
                        .ToList();
                    if (errors.Count > 0)
                    {
                        Console.WriteLine("Sentiment Analysis: Action submitted but contained some errors:");
                        foreach (var err in errors)
                        {
                            Console.WriteLine(" >> {0}:{1} -> {2}", err.code, err.message, err.InnerError != null ? $"-> {err.InnerError.code}:{err.InnerError.message}" : string.Empty);
                        }
                    } else
                    {
                        Console.WriteLine("Sentiment Analysis: Action submitted successfully:");
                        var allResultItems = result.SentimentAnalysis.GetResults();
                        foreach (var resultItem in allResultItems)
                        {
                            Console.WriteLine(" >> {0}:{1} ({2})",resultItem.id, resultItem.score, result.SentimentAnalysis.ScoringEngine.EvaluateScore(resultItem.score).Name);
                        }
                    }
                } else
                {
                    Console.WriteLine("Sentiment Analysis: Unsuccessful. Reason: {0}", result.SentimentAnalysis.AnalysisResult.ApiCallResult.ErrorMessage);
                }
            } catch (Exception ex)
            {
                Console.WriteLine("Error calling the Cognitive service. [{0}]", ex.Message);
            }



        }
    }
}
