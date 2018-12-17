using System;
using System.Collections.Generic;
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
            ParseArguments();
        }

        private void ParseArguments()
        {
            if (_args == null || _args.Length != 2)
            {
                return;
            }

            if (_args[0].ToLowerInvariant() == "-f")
            {
                Filename = _args[1];
                ArgumentType = ArgType.Filename;
                return;
            }

            if (_args[0].ToLowerInvariant() == "-t")
            {
                TextToAnalyse = _args[1];
                ArgumentType = ArgType.ManualText;
                return;
            }
        }

        public string Filename { get; private set; }
        public string TextToAnalyse { get; private set; }

        public ArgType ArgumentType { get; private set; }


    }

    public enum ArgType
    {
        Invalid,
        Filename,
        ManualText
    }
}
