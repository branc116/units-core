using System.Linq;
using System.Text.RegularExpressions;
using Units.Core.Parser.State;

namespace Units.Core.Parser.Handlers
{
    public class HandleReals : IHandler
    {
        public static readonly Regex Match = new Regex($@"Real\(Types\) *:=");
        private static readonly Regex _match = new Regex(@"\((?<num>\w+), *(?<clr>\w+)\)");
        public bool Handle(ParserState parserState, string s)
        {
            var reals = _match.Matches(s).ToLinq()
                    .Select(i => (i.Groups["num"].Value, i.Groups["clr"].Value))
                    .ToList()
                    .Select(i => new RealDef { WrapName = i.Item2, ClrName = i.Item1 });
            parserState.RealDefs.UnionWith(reals);
            return true;
        }
        public Regex MatchRegex(ParserState parserState) =>
            Match;
    }

}
