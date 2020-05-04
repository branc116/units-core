using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Units.Core.Parser.State;

namespace Units.Core.Generators
{
    public partial class GenerateUnit
    {
        
        public RealDef Real { get; }
        public HashSet<SelfOp> SelfOps { get; }
        public IUnit Unit { get; }
        public ParserState State { get; }
        public GenerateUnit(ParserState state, IUnit unit, RealDef realDef)
        {
            State = state;
            Unit = unit;
            Real = realDef;
            SelfOps = State.SelfOps.Select(i =>
            {
                i.RetType = i.RetType == "null" ? null : i.RetType;
                return i;
            }).ToHashSet(); ;
        }
        public string Namespace()
        {
            return $"Units.{Real.WrapName}{(Unit.IsInfered ? ".Infers" : string.Empty)}";
        }
        public string RealWrapper()
        {
            var name = $"using Scalar = Numbers.{Real.WrapName}s.{Real.WrapName};";
            return name;
        }
        public string Usings()
        {
            var usings = new[] { $"using System;",
                $"using Numbers.{Real.WrapName}s;",
                State.Units.Any(i => i.IsInfered) ? $"using Units.{Real.WrapName}{(!Unit.IsInfered ? ".Infers" : string.Empty)};" : null,
                RealWrapper() 
            }
                .Where(i => i is string)
                .Aggregate((i, j) => $"{i}{System.Environment.NewLine}{j}");
            return usings;
        }

        public string EdgeOps()
        {
            var text = new GenerateEdgeOperators(State, Unit).TransformText();
            return text;
        }
        public string Converts()
        {
            if (!State.MesurmentUnits.Any(i => i.For.Equals(Unit)))
                return string.Empty;
            var text = new GenerateConverters(State, Unit).TransformText();
            return text;
        }
        public string Summary() {
            if (!State.Descriptions.ContainsKey(Unit) || !State.Descriptions[Unit].Any() || State.Descriptions[Unit].All(i => i.XmlDoc is null))
                return string.Empty;

            var summary = State.Descriptions[Unit]
                .Where(i => i.XmlDoc is string)
                .Select(i => "///" + i.XmlDoc)
                .Aggregate((i, j) => $"{i}{Environment.NewLine}///Or{Environment.NewLine}{j}");
            return summary;
        }
        public string Remarks()
        {
            if (!State.Descriptions.ContainsKey(Unit) || !State.Descriptions[Unit].Any() || State.Descriptions[Unit].All(i => i.XmlDocRemarks is null))
                return string.Empty;
            var remarks = State.Descriptions[Unit]
                .Where(i => i.XmlDocRemarks is string)
                .Select(i => "///" + i.XmlDocRemarks)
                .Aggregate((i, j) => $"{i}{Environment.NewLine}///Or{Environment.NewLine}{j}");
            return remarks;
        }
    }
}
