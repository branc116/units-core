using System;
using System.IO;
using System.Linq;
using Units.Core.Generators;
using Units.Core.Parser;
using Units.Core.Parser.State;

namespace Units.Core
{
    public class DefaultExporter : IExportHandle
    {
        public string BaseDir { get; }

        public DefaultExporter(string baseDir)
        {
            BaseDir = baseDir;
        }
        public bool ExportUnits(ParserState state, string location)
        {
            location = Path.Combine(BaseDir, location);
            if (File.Exists(location))
            {
                throw new HandleException($"Export requires directory, not a file. '{location}' is a file", 0717);
            }
            var dirsToCreate = Enumerable.Empty<string>();
            var unitsDirectory = Path.Combine(location, "Units");
            dirsToCreate = dirsToCreate.Append(location);
            dirsToCreate = dirsToCreate.Append(unitsDirectory);
            foreach (var real in state.RealDefs) {
                var realDir = Path.Combine(unitsDirectory, real.WrapName);
                var inferedUnitsDirectory = Path.Combine(realDir, "Infered");
                dirsToCreate = dirsToCreate.Append(realDir);
                dirsToCreate = dirsToCreate.Append(inferedUnitsDirectory);
            }
            dirsToCreate.CreateDirs();
            foreach(var (unit, real) in state.Units.Join(state.RealDefs, i => true, i => true, (unit, real) => (unit, real)))
            {
                if (unit is Scalar)
                    continue;
                var path = Path.Combine(unitsDirectory, $"{real.WrapName}");
                if (unit.IsInfered)
                    path = Path.Combine(path, "Infered");
                path = Path.Combine(path, $"{unit.Name}.cs");
                var text = new GenerateUnit(state, unit, real).TransformText()
                    .NonEmptyOrWhitespaceLines().ToArray();
                File.WriteAllLines(path, text);
            }
            return true;
        }

        public bool ExportWrappers(ParserState state, string location)
        {
            var numbersDirectory = Path.Combine(BaseDir, location, "Numbers");
            new[] { location, numbersDirectory }.CreateDirs();
            var path = Path.Combine(numbersDirectory, "Wrappers.cs");
            var text = new GenerateWrappers(state).TransformText()
                .NonEmptyOrWhitespaceLines().ToArray();
            File.WriteAllLines(path, text);
            return true;
        }
    }
}
