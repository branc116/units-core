using System.Linq;
using System.Text.RegularExpressions;
using Units.Core.Parser.State;
using static Units.Core.Parser.Handlers.Constants;

namespace Units.Core.Parser.Handlers
{
    /// <summary>
    /// Handles lines like
    /// <code>Base(Unit) := Length | Mass | Time</code>
    /// Goal is to handle
    /// <code>Units(Base) := Length | Mass | Time</code>
    /// </summary>
    public class HandleBase : IHandler
    {
        private static readonly Regex _match = new Regex($@"Base\(Unit\) := ((?<unit>{UnitName}){EndOrMore})+$");
        /// <inheritdoc/>
        public bool Handle(ParserState parserState, string s)
        {
            var a = _match.Matches(s)
                    .ToLinq()
                    .FirstOrDefault()
                    .Groups["unit"]
                    .Captures.ToLinq()
                    .Select(i => i.Value)
                    .Select(i => new Unit(i));
            parserState.Units.UnionWith(a);
            return true;
        }
        /// <inheritdoc/>
        public Regex MatchRegex(ParserState parserState) =>
            new Regex($@"Base\(Unit\) *:= *({UnitName}{EndOrMore})+");
    }

}
