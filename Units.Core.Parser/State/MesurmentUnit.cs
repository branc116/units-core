using System;
using System.Collections.Generic;
using MathNet.Symbolics;
using Ex = MathNet.Symbolics.SymbolicExpression;
using Expr = MathNet.Symbolics.Expression;
namespace Units.Core.Parser.State
{
    public class MesurmentUnit : IEquatable<MesurmentUnit>
    {
        public IUnit For { get; set; }
        public string Name { get; set; }
        public string ConvertTo { get; set; }
        public string ConvertFrom { get
            {
                var expr = Ex.Parse($"{ConvertTo.Replace('x', 'X')} - x");
                var X = Expr.Symbol("X");
                var sol = Helpers.SolveSimpleRoot(X, expr.Expression);
                var retExpr = new Ex(sol);
                return retExpr.ToString();
            }
        }
        public string Postfix { get; set; }
        public string Summary { get; set; }
        public string Remarks { get; set; }
        public override bool Equals(object obj)
        {
            return Equals(obj as MesurmentUnit);
        }

        public bool Equals(MesurmentUnit other)
        {
            return other != null &&
                   EqualityComparer<IUnit>.Default.Equals(For, other.For) &&
                   Name == other.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(For, Name);
        }
    }
}
