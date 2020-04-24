using System;
using System.Collections.Generic;
using System.Linq;

namespace Units.Core.Parser.State
{
    public class UnaryOperator : IEquatable<UnaryOperator>, IOperator
    {
        public string Name { get; }
        public string Symbol { get; }
        public (double count, char? postfix) Count { get; set; }
        private List<(int res, IOperator @operator, int[] param)> Infers { get; set; }
        public UnaryOperator(string name, string symbol)
        {
            Name = name;
            Symbol = symbol;
        }
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
            return Equals(obj as UnaryOperator);
        }

        public bool Equals(UnaryOperator other)
        {
            return other is UnaryOperator @operator &&
                   Name == @operator.Name &&
                   Symbol == @operator.Symbol;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Symbol);
        }
    }
}
