using System;
using System.Linq;
using System.Text.RegularExpressions;
using Units.Core.Parser.State;
using static Units.Core.Parser.Handlers.Constants;

namespace Units.Core.Parser.Handlers
{
    public class HandleMesument : IHandler
    {
        private static readonly Regex _match = new Regex($@"Unit\({UnitName}\)");
        private static readonly Regex _match1 = new Regex($@"Unit\((?<for>{UnitName})\)");
        private static readonly Regex _match2 = new Regex($@"(?<name>{UnitName})\((?<expr>[^,]+), *(?<sym>[^\) ])\)");
        public bool Handle(ParserState parserState, string s)
        {
            var split = s.Split(new[] { ":=" }, StringSplitOptions.RemoveEmptyEntries);

            var a = parserState.Units.FirstOrDefault(i => i.Name == _match1
                .Matches(split[0])[0].Groups["for"].Value);
            var b = _match2.Matches(split[1]).ToLinq()
                .Select(i => (name: i.Groups["name"].Value, expr: i.Groups["expr"].Value, sym: i.Groups["sym"].Value))
                .Select(i => new MesurmentUnit { Derive = i.expr, For = a, Name = i.name, Postfix = i.sym });
            parserState.MesurmentUnits.UnionWith(b);
            return true;
        }
        public Regex MatchRegex(ParserState parserState) =>
            _match;
    }

}
