using System.Linq;
using System.Text.RegularExpressions;
using Units.Core.Parser.State;
using static Units.Core.Parser.Handlers.Constants;

namespace Units.Core.Parser.Handlers
{
    /// <summary>
    /// Handle lines like:
    /// <code>Operations := (*, Times) | (/, Per)</code>
    /// Goal is to unify all of the operator definitions (see remarks.)
    /// </summary>
    /// <remarks>
    /// This is the goal
    /// <code>
    /// Operators(Binary) := (*, Times, 1, 1) | (/, Per, 1, -1)
    /// Operators(Unary) := (Square, 2) | (SquareRoot, 0.5)
    /// Operators(Self) := (==, Eq, bool) | (!=, Ne, bool) | (+, Plus, null) | (-, Minus, null) | (*, Times, null) | (/, Per, null)
    /// </code>
    /// </remarks>
    public class HandleOperators : IHandler
    {
        public static Regex Match = new Regex($@"Operators\(Binary\) *:=");
        private static readonly Regex _match = new Regex($@"\((?<op>{OperatorReg}), *(?<name>{UnitName}), *((?<left>{FloatLike})(?<lp>[a-z])?), *(?<right>{FloatLike})(?<rp>[a-z])?\)");
        public bool Handle(ParserState parserState, string s)
        {
            var a = _match.Matches(s)
                    .ToLinq()
                    .Select(i => (i.Groups["op"], i.Groups["name"], i.Groups["left"], i.Groups["lp"], i.Groups["right"], i.Groups["rp"]))
                    .ToList()
                    .Select(i => new Operator(i.Item2.Value, i.Item1.Value)
                    {
                        CountLeft = (double.Parse(i.Item3.Value), i.Item4.Success ? i.Item4.Value[0] : default),
                        CountRight = (double.Parse(i.Item5.Value), i.Item6.Success ? i.Item6.Value[0] : default),
                    });
            if (parserState.Operators.Intersect(a).Any())
                throw new HandleException("Can't redefine operator", 1858);
            parserState.Operators.UnionWith(a);
            return true;
        }
        public Regex MatchRegex(ParserState parserState) =>
            Match;
    }

}
