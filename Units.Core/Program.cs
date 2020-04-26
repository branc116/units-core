using System;
using CommandLine;
using Units.Core.CommandLineOptions;

namespace Units.Core
{
    class Program
    {
        public static void Main(string[] args)
        {
            var res = CommandLine.Parser.Default.ParseArguments<Init.InitOptions, Run.RunOptions>(args).MapResult(
                (Init.InitOptions init) => new Init(init).DoIt(),
                (Run.RunOptions run) => new Run(run).DoIt(),
                i => false);
        }
    }
}
