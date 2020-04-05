using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Units.Core.Parser.Handlers;
using Units.Core.Parser.State;

namespace Units.Core.Parser
{
    public static class Helpers
    {
        public static string ToRegexOr(this IEnumerable<Unit> units)
        {
            return '(' + (units.Any() ?
                units.Select(i => i.Name).Aggregate((i, j) => $"{i}|{j}") :
                string.Empty) + ')';
        }
        public static string ToRegexOr(this IEnumerable<Operator> operators)
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
        public static IEnumerable<IHandler> GetHandlers()
        {
            var instances = typeof(Helpers).Assembly.DefinedTypes.Where(i => i.GetInterface(nameof(IHandler)) != null)
                .Select(i => i.GetConstructor(new Type[] { }))
                .Select(i => i.Invoke(null))
                .Cast<IHandler>();
            return instances;
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
    }
}
