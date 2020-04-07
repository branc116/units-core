using System.Linq;
using System.Text.RegularExpressions;
using Units.Core.Parser.State;

namespace Units.Core.Parser.Handlers
{
    /// <summary>
    /// Handle lines like:
    /// <code>Real(Types) := (System.Single, RealFloat) | (System.Double, RealDouble) | (Godot.Vector3, Vec3)</code>
    /// </summary>
    /// <remarks>
    /// Defnine what will be the base types underlying units.
    /// This makes it possible to add unit definitions for custom types, so that you can use existion data structures, such as Godot.Vector3
    /// Adds new elements to <see cref="State.ParserState.RealDefs"/>
    /// </remarks>
    public class HandleReals : IHandler
    {
        public static readonly Regex Match = new Regex($@"Real\(Types\) *:=");
        private static readonly Regex _match = new Regex(@"\((?<num>\w+), *(?<clr>\w+)\)");
        /// <inheritdoc/>
        public bool Handle(ParserState parserState, string s)
        {
            var reals = _match.Matches(s).ToLinq()
                    .Select(i => (i.Groups["num"].Value, i.Groups["clr"].Value))
                    .ToList()
                    .Select(i => new RealDef { WrapName = i.Item2, ClrName = i.Item1 });
            parserState.RealDefs.UnionWith(reals);
            return true;
        }
        /// <inheritdoc/>
        public Regex MatchRegex(ParserState parserState) =>
            Match;
    }

}
