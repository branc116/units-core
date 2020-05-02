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
                var sol = SolveSimpleRoot(X, expr.Expression);
                var retExpr = new Ex(sol);
                return retExpr.ToString();
                static Expr SolveSimpleRoot(Expr variable, Expr expr)
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
        public string Postfix { get; set; }

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
