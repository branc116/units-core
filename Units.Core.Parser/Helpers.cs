using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Units.Core.Parser.State;

namespace Units.Core.Parser
{
    /// <summary>
    /// Helper class
    /// </summary>
    public static class Helpers
    {
        public static string ToRegexOr(this IEnumerable<Unit> units)
        {
            return '(' + (units.Any() ?
                units.Select(i => i.Name).Aggregate((i, j) => $"{i}|{j}") :
                string.Empty) + ')';
        }
        public static string ToRegexOr(this IEnumerable<BinaryOperator> operators)
        {
            return '(' + (operators.Any() ?
                operators.Select(i => i.Symbol.ToString()).Aggregate((i, j) => $"\\{i}|\\{j}") :
                string.Empty) + ')';
        }
        public static IEnumerable<Match> ToLinq(this MatchCollection mc)
        {
            for (var i = 0; i < mc.Count; i++)
            {
                yield return mc[i];
            }
        }
        public static IEnumerable<Capture> ToLinq(this CaptureCollection cc)
        {
            for (var i = 0; i < cc.Count; i++)
            {
                yield return cc[i];
            }
        }
        public static IEnumerable<(int index, T value)> ToIndexed<T>(this IEnumerable<T> ts)
        {
            var i = 0;
            foreach (var t in ts)
            {
                yield return (i, t);
                i++;
            }
        }
        public static (IUnit unit, int depth) ShortestUnit(BinaryCompositUnit compositUnit, ParserState state)
        {
            //var dict = compositUnit.GetNamedBaseUnitsCount();
            var curString = string.Empty;
            var onlyBase = state.Units.Where(i => i is Unit).ToList();
            var start = onlyBase
                .Select(i => (i, 0))
                .Append((Scalar.Get, 0));
            var stack = new List<(IUnit unit, int depth)>(start);
            var cur = stack.First();
            var visitedB = new HashSet<(IUnit, IUnit, IOperator)>();
            var visitedU = new HashSet<(IUnit, IOperator)>();
            var visited = new HashSet<IUnit>();
            var uc = new UnitComparer(compositUnit);
            while (stack.Count > 0 && !compositUnit.Equals((cur = stack.OrderBy(uc.Dist).First()).unit))
            {
                stack.RemoveAll(i => i.unit.Equals(cur.unit));
                foreach (var op in state.Operators)
                {
                    if (op is UnaryOperator uo)
                    {
                        if (visitedU.Contains((cur.unit, uo)))
                            continue;
                        var newUnit = new UnaryCompositUnit(cur.unit, uo, null);
                        visitedU.Add((cur.unit, uo));
                        if (visited.Contains(newUnit))
                            continue;
                        visited.Add(newUnit);
                        stack.Add((newUnit, cur.depth + 1));
                    }
                    else if (op is BinaryOperator bo)
                    {
                        foreach (var unit in onlyBase)
                        {
                            if (visitedB.Contains((cur.unit, unit, op)))
                                continue;
                            var newUnit = new BinaryCompositUnit(cur.unit, bo, unit);
                            visitedB.Add((cur.unit, unit, bo));
                            if (visited.Contains(newUnit))
                                continue;
                            visited.Add(newUnit);
                            stack.Add((newUnit, cur.depth + 1));
                        }
                    }
                }
            }
            return cur;
        }
    }
}
