using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Units.Core.Parser.Metadata;
using Units.Core.Parser.State;
using Expr = MathNet.Symbolics.Expression;
using Ex = MathNet.Symbolics.SymbolicExpression;
using MathNet.Symbolics;
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
        private static Dictionary<string, (IUnit unit, int depth)> shortest { get; } = new Dictionary<string, (IUnit, int)>();
        public static (IUnit unit, int depth) ShortestUnitFast(BinaryCompositUnit bcu, ParserState state)
        {
            IUnit uppers = null, downers = null;
            var dimensions = bcu.GetBaseUnitCount();
            foreach (var bd in dimensions.Where(i => i.Value < 0))
            {
                var a = bd.Key;
                var n = bd.Value * -1;
                while (--n >= 0)
                {
                    if (downers is null)
                        downers = a;
                    else
                        downers = new BinaryCompositUnit(downers, BinaryOperator.TIMES, a);
                }
            }
            foreach (var bd in dimensions.Where(i => i.Value > 0))
            {
                var a = bd.Key;
                var n = bd.Value;
                while (--n >= 0)
                {
                    if (uppers is null)
                        uppers = a;
                    else
                        uppers = new BinaryCompositUnit(uppers, BinaryOperator.TIMES, a);
                }
            }
            var ret = downers is null ?
                uppers ?? Scalar.Get :
                new BinaryCompositUnit(uppers ?? Scalar.Get, BinaryOperator.OVER, downers, null, true);
            return (MakeItNice(state, ret), dimensions.Select(i => i.Value).Select(Math.Abs).Sum() + (downers is null ? 0 : 1));
        }
        public static (IUnit unit, int depth) ShortestUnit(BinaryCompositUnit compositUnit, ParserState state)
        {
            //var dict = compositUnit.GetNamedBaseUnitsCount();
            var wantedName = compositUnit.SiName(true);
            if (shortest.ContainsKey(wantedName))
                return shortest[wantedName];
            var curString = string.Empty;
            var onlyBase = state.Units.Where(i => i is Unit)
                .Where(i => wantedName.Contains(i.Name))
                .ToList();
            var start = onlyBase
                .Select(i => (i, 0))
                .Append((Scalar.Get, 0))
                .Union(shortest.Values);
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
                            var newUnit = new BinaryCompositUnit(cur.unit, bo, unit, null, true);
                            visitedB.Add((cur.unit, unit, bo));
                            if (visited.Contains(newUnit))
                                continue;
                            visited.Add(newUnit);
                            var toAdd = (newUnit, cur.depth + 1);
                            if (!shortest.ContainsKey(newUnit.SiName(true)))
                                shortest.Add(newUnit.SiName(true), (MakeItNice(state, toAdd.newUnit), toAdd.Item2));
                            stack.Add(toAdd);
                        }
                    }
                }
            }
            var ret = (MakeItNice(state, cur.unit), cur.depth);
            return  ret;
        }
        public static IUnit MakeItNice(ParserState state, IUnit unit)
        {
            if (unit is BinaryCompositUnit bcu) {
                return new BinaryCompositUnit(bcu.Unit1, 
                    bcu.Operator,
                    bcu.Unit2,
                    bcu.SiName(),
                    bcu.IsInfered);
            }
            return unit;
        }
        public static IEnumerable<IEnumerable<T>> Batched<T>(this IEnumerable<T> col, int batchSize)
        {
            var i = 0;
            List<T> cur = null;

            do
            {
                yield return cur = col.Skip(i).Take(batchSize).ToList();
                i += batchSize;
            } while (cur.Any());
        }
        public static double GetHashSetHealth<T>(this HashSet<T> col)
        {
            int len = col.Count;
            var healths = col.Select(i => col
                    .Where(j => !j.Equals(i))
                    .Select(j => Math.Abs(i.GetHashCode() ^ j.GetHashCode()))
                    .Min())
                .Select(i => i == 0 ? 0 : 1 + Math.Log10(Math.Abs(i))/len);
            var health = healths.Aggregate((i, j) => i * j);
            return health;
        }
        public static IEnumerable<MesurmentUnit> ToMesurmentUnits(this IEnumerable<UnitsNetUnit> units, IUnit unit)
        {
            foreach(var u in units)
            {
                var clean = u.FromBaseToUnitFunc.CleanExpression();
                yield return new MesurmentUnit
                {
                    ConvertTo = clean,
                    For = unit,
                    Name = u.SingularName,
                    Postfix = u.Postfix,
                    Remarks = u.XmlDocRemarks,
                    Summary = u.XmlDocSummary
                };
            }
        }
        public static string CleanExpression(this string expr)
        {
            var rep1 = Regex.Replace(expr, @"Math.Pow\((?<base>[^,]*), *(?<exp>[^)]*) *\)", "$1^$2")
                    .Replace("Math.PI", Math.PI.ToString(CultureInfo.GetCultureInfo("en-US").NumberFormat));
            var rep2 = Regex.Replace(rep1, "(?<num>[0-9]+)(?<pf>[dlfumDLFU])", "$1");
            
            return rep2;
        }
        public static string AddExplicitConvertToNumbers(this string expr, string convertTo)
        {
            var rep3 = Regex.Replace(expr, @"(?<num>(([+\-]?\d)((\d|[eE\-+.]))*))", $"(({convertTo})($1))");
            return rep3;
        }
        public static string Aprox(this string expr)
        {
            var x = Expr.Symbol("X");
            var e = Ex.Parse($"{expr} - X");
            var solved = SolveSimpleRoot(x, e.Expression);
            var aprox = new Ex(solved).Approximate();
            return aprox.ToString();
        }
        public static Expr SolveSimpleRoot(Expr variable, Expr expr)
        {
            // try to bring expression into polynomial form
            Expr simple = Algebraic.Expand(Rational.Numerator(Rational.Simplify(variable, expr)));

            // extract coefficients, solve known forms of order up to 1
            Expr[] coeff = Polynomial.Coefficients(variable, simple);
            switch (coeff.Length)
            {
                case 1: return Expr.Zero.Equals(coeff[0]) ? variable : Expr.Undefined;
                case 2: return Rational.Simplify(variable, Algebraic.Expand(-coeff[0] / coeff[1]));
                default: return Expr.Undefined;
            }
        }
    }
}
