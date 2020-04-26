using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Units.Core.Parser.State;

namespace Units.Core.Generators
{
    public partial class GenerateWrappers
    {
        public ParserState State { get; }
        public HashSet<RealDef> RealDefs { get; }
        public HashSet<SelfOp> SelfOps { get; }
        public IUnit Scalar { get; }
        public GenerateWrappers(ParserState state)
        {
            State = state;
            RealDefs = state.RealDefs;
            SelfOps = State.SelfOps.Select(i =>
            {
                i.RetType = i.RetType == "null" ? null : i.RetType;
                return i;
            }).ToHashSet(); ;
            Scalar = Parser.State.Scalar.Get;
        }
        public string EdgeOps(IUnit forUnit)
        {
            var text = new GenerateEdgeOperators(State, forUnit).TransformText();
            return text;
        }
    }
}
