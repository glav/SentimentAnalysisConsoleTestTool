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
                Console.WriteLine("Options: \n\t-k: Include Keyphrase Analysis\n\t-f {filename.ext} : Process contents of file.\n\t-t \"Text to analyse\" : Process text as entered.");
                Console.WriteLine("\nFormat: \n\tSentimentAnalysisConsoleTestTool -f {filename.txt}");
                Console.WriteLine("\tSentimentAnalysisConsoleTestTool -t \"Text to analyse\"");
                Console.WriteLine("\tSentimentAnalysisConsoleTestTool -f {filename.txt} -k\n");

                return;
            }

            if (string.IsNullOrWhiteSpace(Config.ConfigReader.ApiKey) || string.IsNullOrWhiteSpace(Config.ConfigReader.ApiLocation) )
            {
                Console.WriteLine("No API location or key is present. Cannot continue.");
                return;
            }


            string textToAnalyse = GetAnalysisInput(cliArguments);
            if (textToAnalyse == null) return;

            var location = (LocationKeyIdentifier)System.Enum.Parse(typeof(LocationKeyIdentifier), Config.ConfigReader.ApiLocation, true);

            try
            {
                Console.WriteLine("Sentiment Analysis: Submitting {0} characters of text for analysis.", textToAnalyse.Length);
                if (cliArguments.Options == ArgOptions.IncludeKeyphraseAnalysis)
                {
                    Console.WriteLine("Including Keyphrase analysis");
                }
                var analysis = TextAnalyticConfigurationSettings.CreateUsingConfigurationKeys(Config.ConfigReader.ApiKey, location)
                    .AddConsoleDiagnosticLogging()
                    .UsingHttpCommunication()
                    .WithTextAnalyticAnalysisActions()
                    .AddSentimentAnalysisSplitIntoSentences(textToAnalyse);
                if (cliArguments.Options == ArgOptions.IncludeKeyphraseAnalysis)
                {
                    analysis = analysis.AddKeyPhraseAnalysis(textToAnalyse);
                }
                var result = analysis.AnalyseAllAsync().Result;

                ProcessResults(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error calling the Cognitive service. [{0}]", ex.Message);
            }



        }

        private static void ProcessResults(TextAnalyticAnalysisResults result)
        {
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
                        Console.WriteLine(" {0}: {1} -> {2}", err.code, err.message, err.InnerError != null ? $"-> {err.InnerError.code}:{err.InnerError.message}" : string.Empty);
                    }
                }
                else
                {
                    Console.WriteLine("Sentiment Analysis: Action submitted successfully:");
                    var allResultItems = result.SentimentAnalysis.GetResults();
                    foreach (var resultItem in allResultItems)
                    {
                        Console.WriteLine(" {0}: {1} ({2})", resultItem.id, resultItem.score, result.SentimentAnalysis.ScoringEngine.EvaluateScore(resultItem.score).Name);
                    }
                }
            }
            else
            {
                var firstError = result.SentimentAnalysis.AnalysisResult.ResponseData.errors.First();
                Console.WriteLine("Sentiment Analysis: Unsuccessful. Reason: {0}:{1}", firstError.code, firstError.message, firstError.InnerError != null ? $" -> {firstError.InnerError.code}:{firstError.InnerError.message}" : string.Empty);
            }

            if (result.KeyPhraseAnalysis.AnalysisResult.ActionSubmittedSuccessfully)
            {
                var errors = result.KeyPhraseAnalysis.AnalysisResults
                    .Where(r => r.ResponseData.errors != null && r.ResponseData.errors.Length > 0)
                    .SelectMany(s => s.ResponseData.errors)
                    .ToList();
                if (errors.Count > 0)
                {
                    Console.WriteLine("Keyphrase Analysis: Action submitted but contained some errors:");
                    foreach (var err in errors)
                    {
                        Console.WriteLine(" {0}: {1} -> {2}", err.code, err.message, err.InnerError != null ? $"-> {err.InnerError.code}:{err.InnerError.message}" : string.Empty);
                    }
                }
                else
                {
                    Console.WriteLine("Keyphrase Analysis: Action submitted successfully:");
                    var allResultItems = result.KeyPhraseAnalysis.AnalysisResults.Select(r => r.ResponseData);
                    foreach (var resultItem in allResultItems)
                    {
                        var keyphraseList = resultItem.documents.SelectMany(k => k.keyPhrases);
                        Console.WriteLine(" {0}: {1}", resultItem.id, string.Join(",",keyphraseList));
                    }
                }
            }
            else
            {
                var firstError = result.SentimentAnalysis.AnalysisResult.ResponseData.errors.First();
                Console.WriteLine("Keyphrase Analysis: Unsuccessful. Reason: {0}:{1}", firstError.code, firstError.message, firstError.InnerError != null ? $" -> {firstError.InnerError.code}:{firstError.InnerError.message}" : string.Empty);
            }
        }

        private static string GetAnalysisInput(CliArguments cliArgs)
        {
            if (cliArgs.ArgumentType == ArgType.ManualText)
            {
                return cliArgs.TextToAnalyse;
            }
            else
            {
                if (System.IO.File.Exists(cliArgs.Filename))
                {
                    return System.IO.File.ReadAllText(cliArgs.Filename);
                }
                else
                {
                    Console.WriteLine("Error: File [{0}] does not exist", cliArgs.Filename);
                    return null;
                }
            }
        }
    }
}
