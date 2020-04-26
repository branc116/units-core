using System;
using System.Collections;
using System.IO;
using CommandLine;
using Units.Core.Parser;
using Units.Core.Parser.State;

namespace Units.Core.CommandLineOptions
{
    public class Run
    {
        [Verb("Run", HelpText = "Generate units and wrapper types defined in the units file")]
        public class RunOptions
        {
            [Option('f', "file", Required = false, Default = "units.txt", HelpText = "In what file is the definition located")]
            public string File { get; set; }
            [Option('e', "exporter", Required = false, Default = ExporterType.Default,
                HelpText = "If you are debugging the unit file, set this to console or null and you will not get any output if you use '!Export' commands")]
            public ExporterType ExportType { get; set; }
        }
        public RunOptions Options { get; set; }
        public Run(RunOptions options)
        {
            Options = options;
        }
        public bool DoIt()
        {
            IExportHandle exporter = Options.ExportType switch
            {
                ExporterType.Console => new ConsoleExporter(),
                ExporterType.Default => new DefaultExporter(Path.Combine(Environment.CurrentDirectory, Path.GetDirectoryName(Options.File))),
                ExporterType.Null => default,
                _ => throw new HandleException($"Invalid Exporter type. Please select one of the valid exporters", 0832)
            };
            var state = Parser.Parser.PaseGramarFile(Options.File, exporter);
            return true;
        }
    }
}
