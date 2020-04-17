using System.Collections.Generic;


namespace Units.Core
{
    using System.Linq;
    using Units.Core.Parser.State;
    public partial class GenerateEdgeOperators
    {
        public IUnit Unit1 { get; set; }
        public IEnumerable<(Operator Operator, IUnit Unit2, IUnit Res)> Edges { get; set; }
        public bool Valid { get; } = false;

        public GenerateEdgeOperators(ParserState state, IUnit forUnit)
        {
            if (!state.GraphEdges.ContainsKey(forUnit))
                return;
            Unit1 = forUnit;
            Edges = state
                .GraphEdges[forUnit]
                .Where(i => state.Units.Contains(i.op2))
                .Select(i => (Operator: i.op, Unit2: i.op2, Res: i.res)).ToList();
            Valid = true;
        }
    }
}
