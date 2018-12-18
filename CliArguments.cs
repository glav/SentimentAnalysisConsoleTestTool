using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SentimentAnalysisConsoleTestTool
{
    internal class CliArguments
    {
        private string[] _args;
        public CliArguments(string[] args)
        {
            _args = args;
            ArgumentType = ArgType.Invalid;
            Options = ArgOptions.None;
            ParseArguments();
        }

        private void ParseArguments()
        {
            if (_args == null || _args.Length == 0)
            {
                return;
            }

            if (_args.Any(a => a.ToLowerInvariant() == "-?" || _args.Any(b => b.ToLowerInvariant() == "-help")))
            {
                return;
            }

            if (_args.Any(a => a.ToLowerInvariant() == "-k"))
            {
                Options = ArgOptions.IncludeKeyphraseAnalysis;
            }

            if (_args.Any(a => a.ToLowerInvariant() == "-f"))
            {
                Filename = _args[1];
                ArgumentType = ArgType.Filename;
                return;
            }

            if (_args.Any(a => a.ToLowerInvariant() == "-t"))
            {
                TextToAnalyse = _args[1];
                ArgumentType = ArgType.ManualText;
                return;
            }
        }

        public string Filename { get; private set; }
        public string TextToAnalyse { get; private set; }

        public ArgType ArgumentType { get; private set; }
        public ArgOptions Options { get; private set; }


    }

    public enum ArgType
    {
        Invalid,
        Filename,
        ManualText
    }

    public enum ArgOptions
    {
        None,
        IncludeKeyphraseAnalysis
    }
}
