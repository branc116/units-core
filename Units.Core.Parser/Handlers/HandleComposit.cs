using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Units.Core.Parser.State;
using static Units.Core.Parser.Handlers.Constants;

namespace Units.Core.Parser.Handlers
{
    /// <summary>
    /// Handles lines like:
    /// <code>Velocity := Length / Time</code>
    /// </summary>
    public class HandleComposit : IHandler
    {
        public static readonly Regex Match = new Regex($@"{UnitName} *:= {UnitName} {OperatorReg} {UnitName}$");
        private static readonly Regex _match = new Regex($@"(?<new>{UnitName}) := (?<op1>{UnitName}) *(?<operator>{OperatorReg}) *(?<op2>{UnitName})$");
        /// <inheritdoc/>
        public bool Handle(ParserState parserState, string s)
        {
            var groups = _match.Matches(s)[0]
                    .Groups;
            var op = parserState.Operators.FirstOrDefault(i => i.Symbol == groups["operator"].Value);
            var u1 = parserState.Units.FirstOrDefault(i => i.Name == groups["op1"].Value);
            var u2 = parserState.Units.FirstOrDefault(i => i.Name == groups["op2"].Value);
            var newName = groups["new"].Value;
            var unit = new CompositUnit
            {
                Name = newName,
                Operator = op,
                Unit1 = u1,
                Unit2 = u2
            };
            parserState.Units.Add(unit);
            foreach (var (op1, @operator, op2, res) in op.GetEdges(u1, u2, unit))
            {
                if (!parserState.GraphEdges.ContainsKey(op1))
                    parserState.GraphEdges.Add(op1, new HashSet<(Operator, Unit, Unit)>());
                parserState.GraphEdges[op1].Add((@operator, op2, res));
            };
            return true;
        }
        /// <inheritdoc/>
        public Regex MatchRegex(ParserState parserState) =>
            Match;
    }

}
