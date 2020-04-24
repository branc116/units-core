using System;
using System.Collections.Generic;

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
                   EqualityComparer<IOperator>.Default.Equals(Operator, other.Operator) &&
                   EqualityComparer<IUnit>.Default.Equals(Result, other.Result) &&
                   other.Parameters.Length == Parameters.Length;
            if (!res)
                return false;
            for (int i = 0; i < Parameters.Length; ++i)
            {
                res &= Parameters[i].Equals(other.Parameters[i]);
                if (!res)
                    return res;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Operator, Parameters, Result);
        }
    }
}
