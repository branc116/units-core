using System;
using System.Linq;
using Units.Core.Generators;
using Units.Core.Parser.State;

namespace Units.Core
{
    public class ConsoleExporter : IExportHandle
    {
        public bool ExportUnits(ParserState state, string location)
        {
            foreach (var (unit, real) in state.Units.Join(state.RealDefs, i => true, i => true, (unit, real) => (unit, real)))
            {
                var inner = $"{unit.Name} - {real.WrapName}";
                if (inner.Length > Console.WindowWidth)
                {
                    inner = $"{unit.Name}";
                }
                if (inner.Length < Console.WindowWidth)
                {
                    var dif = Console.WindowWidth - inner.Length;
                    inner = new string('-', dif / 2 + dif % 2) + inner + new string('-', dif / 2);
                }
                Console.WriteLine($"{inner}");
                var text = new GenerateUnit(state, unit, real).TransformText();
                Console.WriteLine(text);
            }
            return true;
        }

        public bool ExportWrappers(ParserState state, string location)
        {
            var text = new GenerateWrappers(state).TransformText();
            Console.WriteLine(text);
            return true;
        }
    }
}
