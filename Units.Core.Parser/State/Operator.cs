using System;
using System.Collections.Generic;

namespace Units.Core.Parser.State
{
    public class Operator : IEquatable<Operator>
    {
        public string Name { get; set; }
        public string Symbol { get; set; }
        public Operator Inverse { get; set; }
        public List<(int op1, Operator symbol, int op2, int res)> InferedOperations { get; set; }
        public IEnumerable<(Unit op1, Operator @operator, Unit op2, Unit res)> GetEdges(Unit op1, Unit op2, Unit res)
        {
            if (InferedOperations is null || InferedOperations.Count == 0)
            {
                yield return (op1, this, op2, res);
                yield break;
            }
            var arr = new[] { op1, op2, res };
            foreach (var a in InferedOperations)
            {
                yield return (arr[a.op1], a.symbol, arr[a.op2], arr[a.res]);
            }
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as Operator);
        }

        public bool Equals(Operator other)
        {
            return other != null &&
                   Symbol == other.Symbol;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Symbol);
        }
        public override string ToString()
        {
            return $"{Symbol}";
        }
    }
}
