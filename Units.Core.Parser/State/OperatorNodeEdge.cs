using System;
using System.Collections.Generic;
using System.Linq;

namespace Units.Core.Parser.State
{
    public class OperatorNodeEdge : IEquatable<OperatorNodeEdge>
    {
        public IOperator Operator { get; }
        public IUnit[] Parameters { get; }
        public IUnit Result { get; set; }
        public OperatorNodeEdge(IOperator @operator, IUnit resount, params IUnit[] units)
        {
            Operator = @operator;
            Result = resount;
            Parameters = units;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as OperatorNodeEdge);
        }

        public bool Equals(OperatorNodeEdge other)
        {
            var res = other != null &&
                   Operator.Equals(other.Operator) &&
                   Result.Equals(other.Result) &&
                   other.Parameters.Length == Parameters.Length;
            if (!res)
                return false;
            for (int i = 0; i < Parameters.Length; ++i)
            {
                res = Parameters[i].Equals(other.Parameters[i]);
                if (!res)
                    return res;
            }
            return true;
        }
        private int? _hashCode = null;
        public override int GetHashCode()
        {

            return _hashCode ??= Parameters.Any(i => i is IUnit) ?
                 HashCode.Combine(Operator, Parameters.Where(i => i is IUnit).Select(i => i.GetHashCode()).Aggregate((i, j) => i ^ j), Result) :
                 HashCode.Combine(Operator, Result)
                 ;

        }
    }
}
