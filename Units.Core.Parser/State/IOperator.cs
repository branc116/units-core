using System.Collections.Generic;

namespace Units.Core.Parser.State
{
    public interface IOperator
    {
        string Name { get; }
        string Symbol { get; }
        IEnumerable<(IUnit[] @params, IOperator @operator, IUnit res)> GetInferedOperations(IUnit res, params IUnit[] @params);
        void AddInferedOperatorion(int res, IOperator @operator, params int[] @params);
    }
}
