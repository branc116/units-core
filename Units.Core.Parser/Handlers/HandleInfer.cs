using System.Collections.Generic;
using System.Text.RegularExpressions;
using Units.Core.Parser.State;

namespace Units.Core.Parser.Handlers
{
    /// <summary>
    /// Handle lines:
    /// <code>Infer</code>
    /// Goal is:
    /// <code>!Infer</code>
    /// </summary>
    /// <remarks>
    /// Based on currently defined <see cref="State.Unit"/>, <see cref="State.CompositUnit"/> and <see cref="State.Operator"/> create new <see cref="CompositUnit"/>.
    /// Create edges between them and store them in <see cref="State.ParserState.GraphEdges"/>
    /// </remarks>
    public class HandleInfer : IHandler
    {
        private static readonly Regex _match = new Regex("^Infer$");
        /// <inheritdoc/>
        public bool Handle(ParserState parserState, string input)
        {
            var newUnits = new HashSet<IUnit>();
            foreach (var u1 in parserState.Units)
            {
                foreach (var u2 in parserState.Units)
                {
                    foreach (var op in parserState.Operators)
                    {
                        var un = new CompositUnit(u1, op, u2, null);
                        var simp = un.Simplify();
                        parserState.Units.TryGetValue(simp, out var orig);
                        foreach (var (op1, @operator, op2, res) in
                                op.GetEdges(u1, u2, orig ?? simp))
                        {

                            if (!parserState.GraphEdges.ContainsKey(op1))
                                parserState.GraphEdges.Add(op1, new HashSet<(Operator, IUnit, IUnit)>());
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
                    var u1 = parserState.Units.TryGetValue(cu.Unit1, out var unit1);
                    var u2 = parserState.Units.TryGetValue(cu.Unit2, out var unit2);
                    if (u1 || u2)
                        cu = new CompositUnit(unit1 ?? cu.Unit1, cu.Operator, unit2 ?? cu.Unit2, cu.SiName());
                    foreach (var (op1, @operator, op2, res) in
                        cu.Operator.GetEdges(cu.Unit1, cu.Unit2, cu))
                    {
                        if (!parserState.GraphEdges.ContainsKey(op1))
                            parserState.GraphEdges.Add(op1, new HashSet<(Operator, IUnit, IUnit)>());
                        parserState.GraphEdges[op1].Add((@operator, op2, res));
                    }
                }
                parserState.Units.Add(unit);
            }
            return true;
        }
        /// <inheritdoc/>
        public Regex MatchRegex(ParserState parserState) =>
            _match;
    }
}
