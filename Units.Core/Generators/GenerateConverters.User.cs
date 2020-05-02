using System.Collections.Generic;
using System.Linq;
using Units.Core.Parser.State;

namespace Units.Core.Generators
{
    public partial class GenerateConverters
    {
        public ParserState State { get; }
        public IUnit Unit { get; }
        public GenerateConverters(ParserState state, IUnit unit)
        {
            State = state;
            Unit = unit;
        }
        public List<(string name, string expr)> GetToConverts()
        {
            var a = State.Descriptions[Unit].SelectMany(i => i.Units.Select(j => (name: j.SingularName, expr: j.FromBaseToUnitFunc.Replace("x", "_rawValue"))))
                .ToList();
            return a;
        }
    }
}
