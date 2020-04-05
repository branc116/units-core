using System.Collections.Generic;

namespace Units.Core.Parser.State
{
    public class ParserState
    {
        public HashSet<Unit> Units { get; set; } = new HashSet<Unit>();
        public HashSet<Operator> Operators { get; set; } = new HashSet<Operator>();
        public HashSet<MesurmentUnit> MesurmentUnits { get; set; } = new HashSet<MesurmentUnit>();
        public HashSet<SelfOp> SelfOps { get; set; } = new HashSet<SelfOp>();
        public HashSet<RealDef> RealDefs { get; set; } = new HashSet<RealDef>();
        public Dictionary<Unit, HashSet<(Operator op, Unit op2, Unit res)>> GraphEdges { get; set; } = new Dictionary<Unit, HashSet<(Operator, Unit, Unit)>>();
    }
}
