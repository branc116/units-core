using System.Linq;
using System.Text.RegularExpressions;
using Units.Core.Parser.State;
using static Units.Core.Parser.Handlers.Constants;

namespace Units.Core.Parser.Handlers
{
    public class HandleOperators : IHandler
    {
        public static Regex Match = new Regex($@"Operations := (\({OperatorReg}, *{UnitName}\){EndOrMore})+$");
        private static readonly Regex _match = new Regex($@"\((?<op>{OperatorReg}), *(?<name>{UnitName})\)");
        public bool Handle(ParserState parserState, string s)
        {
            var a = _match.Matches(s)
                    .ToLinq()
                    .Select(i => (i.Groups["op"], i.Groups["name"]))
                    .ToList()
                    .Select(i => new Operator
                    {
                        Name = i.Item2.Value,
                        Symbol = i.Item1.Value
                    });
            parserState.Operators.UnionWith(a);
            return true;
        }

        public Regex MatchRegex(ParserState parserState) =>
            Match;
    }

}
