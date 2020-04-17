using System;
using System.Collections.Generic;

namespace Units.Core.Parser.State
{
    public class Operator : IEquatable<Operator>
    {
        public static Operator TIMES = new Operator("Times", "*") { CountLeft = (1, null), CountRight = (1, null) };
        public static Operator OVER = new Operator("Over", "/") { CountLeft = (1, null), CountRight = (-1, null) };
        public string Name { get; }
        public string Symbol { get; }
        public (double count, char? postfix) CountLeft { get; set; }
        public (double count, char? postfix) CountRight { get; set; }
        public List<(int op1, Operator symbol, int op2, int res)> InferedOperations { get; set; }
        public Operator(string name, string symbol)
        {
            Name = name;
            Symbol = symbol;
        }
        public IEnumerable<(IUnit op1, Operator @operator, IUnit op2, IUnit res)> GetEdges(IUnit op1, IUnit op2, IUnit res)
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
