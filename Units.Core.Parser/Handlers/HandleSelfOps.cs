using System.Linq;
using System.Text.RegularExpressions;
using Units.Core.Parser.State;
using static Units.Core.Parser.Handlers.Constants;

namespace Units.Core.Parser.Handlers
{
    /// <summary>
    /// Handle lines like: 
    /// <code>
    /// SelfOps := (==, Eq, bool) | (!=, Ne, bool) | (+, Plus, null) | (-, Minus, null) | (*, Times, null) | (/, Per, null)
    /// </code>
    /// Goal is to unify it with <see cref="HandleOperators"/>
    /// </summary>
    public class HandleSelfOps : IHandler
    {
        public static readonly Regex Match = new Regex($@"SelfOps *:=");
        private static readonly Regex _match = new Regex($@"\((?<op>{OperatorReg}), *(?<name>{UnitName}), *(?<ret>{FullName})\)");
        /// <inheritdoc/>
        public bool Handle(ParserState parserState, string s)
        {
            var adds = _match.Matches(s).ToLinq()
                    .Select(i => (i.Groups["op"].Value, i.Groups["name"].Value, i.Groups["ret"].Value))
                    .ToList()
                    .Select(i => new SelfOp
                    {
                        Name = i.Item2,
                        Symbol = i.Item1,
                        RetType = i.Item3 == "null" ? null : i.Item3
                    });
            parserState.SelfOps.UnionWith(adds);
            return true;
        }
        /// <inheritdoc/>
        public Regex MatchRegex(ParserState parserState) =>
            Match;
    }

}
