using System;
using System.Collections.Generic;
using System.Linq;

namespace Units.Core.Parser.State
{
    public class BinaryOperator : IEquatable<BinaryOperator>, IOperator
    {
        public static BinaryOperator TIMES = new BinaryOperator("Times", "*") { CountLeft = (1, null), CountRight = (1, null) };
        public static BinaryOperator OVER = new BinaryOperator("Over", "/") { CountLeft = (1, null), CountRight = (-1, null) };
        public string Name { get; }
        public string Symbol { get; }
        public (double count, char? postfix) CountLeft { get; set; }
        public (double count, char? postfix) CountRight { get; set; }
        public BinaryOperator(string name, string symbol)
        {
            Name = name;
            Symbol = symbol;
        }
        private List<(int res, IOperator @operator, int[] param)> Infers { get; set; }

        public void AddInferedOperatorion(int res, IOperator @operator, params int[] @params)
        {
            Infers ??= new List<(int res, IOperator @operator, int[] param)>();
            Infers.Add((res, @operator, @params));
        }
        public IEnumerable<(IUnit[] @params, IOperator @operator, IUnit res)> GetInferedOperations(IUnit res, params IUnit[] @params)
        {
            if (Infers == null)
            {
                yield return (@params, this, res);
                yield break;
            }
            var ar = @params.Prepend(res).ToArray();
            foreach (var infers in Infers)
            {
                yield return (infers.param.Select(i => ar[i]).ToArray(), infers.@operator, ar[infers.res]);
            }
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as BinaryOperator);
        }

        public bool Equals(BinaryOperator other)
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
