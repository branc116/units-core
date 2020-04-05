using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Units.Core.Parser.State;

namespace Units.Core.Parser.Handlers
{
    public class HandleInfer : IHandler
    {
        private static readonly Regex _match = new Regex("^Infer$");
        public bool Handle(ParserState parserState, string input)
        {
            var newUnits = new HashSet<Unit>();
            foreach (var u1 in parserState.Units)
            {
                foreach (var u2 in parserState.Units)
                {
                    foreach (var op in parserState.Operators)
                    {
                        var un = new CompositUnit
                        {
                            Operator = op,
                            Unit1 = u1,
                            Unit2 = u2
                        };
                        var simp = un.Simplify();
                        var orig = parserState.Units.FirstOrDefault(i => i.SiName() == simp.Name);
                        foreach (var (op1, @operator, op2, res) in
                                op.GetEdges(u1, u2, orig ?? simp))
                        {

                            if (!parserState.GraphEdges.ContainsKey(op1))
                                parserState.GraphEdges.Add(op1, new HashSet<(Operator, Unit, Unit)>());
                            parserState.GraphEdges[op1].Add((@operator, op2, res));
                        }
                        if (orig is null)
                        {
                            newUnits.Add(simp);
                        }
                    }
                }
            }
            foreach (var unit in newUnits)
            {
                if (unit is CompositUnit cu)
                {
                    cu.Unit1 = parserState.Units.FirstOrDefault(i => i.SiName() == cu.Unit1.SiName()) ??
                        cu.Unit1;
                    cu.Unit2 = parserState.Units.FirstOrDefault(i => i.SiName() == cu.Unit2.SiName()) ??
                        cu.Unit2;
                    foreach (var (op1, @operator, op2, res) in
                        cu.Operator.GetEdges(cu.Unit1, cu.Unit2, cu))
                    {
                        if (!parserState.GraphEdges.ContainsKey(op1))
                            parserState.GraphEdges.Add(op1, new HashSet<(Operator, Unit, Unit)>());
                        parserState.GraphEdges[op1].Add((@operator, op2, res));
                    }
                }
                parserState.Units.Add(unit);
            }
            return true;
        }

        public Regex MatchRegex(ParserState parserState) =>
            _match;
    }
}
