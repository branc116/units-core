using System.Linq;
using System.Text.RegularExpressions;
using Units.Core.Parser.State;
using static Units.Core.Parser.Handlers.Constants;

namespace Units.Core.Parser.Handlers
{
    /// <summary>
    /// Operator(/) := a = b / c | c = b / a | b = a * c | b = c * a
    /// </summary>
    public class HandleOperatorDefinition : IHandler
    {
        private static readonly Regex _regex = new Regex($@"^Operator\(({OperatorReg})\)");
        public bool Handle(ParserState parserState, string input)
        {
            var op = Regex.Matches(input, $@"Operator\((?<operator>{OperatorReg})\)")[0].Groups["operator"].Value;
            var ops = parserState.Operators.FirstOrDefault(i => i.Symbol == op);
            var difs = Regex.Matches(input, $@"(?<res>\w) = (?<op1>\w) (?<op>(\*|/)) (?<op2>\w)").ToLinq().Select(i => i.Groups)
                .Select(i => (res: i["res"].Value, op1: i["op1"].Value, op: parserState.Operators.First(g => g.Symbol == i["op"].Value), op2: i["op2"].Value))
                .ToList();
            var first = difs.First();
            var order = new[] { first.op1, first.op2, first.res }.ToIndexed().ToDictionary(i => i.value, i => i.index);
            ops.InferedOperations = difs.Select(i => (order[i.op1], i.op, order[i.op2], order[i.res])).ToList();
            return true;
        }

        public Regex MatchRegex(ParserState parserState) =>
            _regex;
    }
}
