using System.Collections.Generic;


namespace Units.Core.Generators
{
    using System;
    using System.Linq;
    using Units.Core.Parser.State;
    public partial class GenerateEdgeOperators
    {
        public IUnit Unit1 { get; set; }
        public IEnumerable<BinaryOperaorModel> Operators { get; }
        public List<(string returnType, string name)> UnaryEdges { get; }
        public bool Valid { get; } = false;
        public ParserState State { get; }
        public GenerateEdgeOperators(ParserState state, IUnit forUnit)
        {
            if (!state.GraphEdges.ContainsKey(forUnit))
                return;
            Unit1 = forUnit;
            Operators = state
                .GraphEdges[forUnit]
                .Where(i => i.Operator is BinaryOperator)
                .Where(i => i.Operator.Symbol == "*" || i.Operator.Symbol == "/")
                .Where(i => i.Parameters.All(j => state.Units.Contains(j)))
                .Where(i => state.Units.Contains(i.Result))
                .Where(i => !Unit1.Equals(Scalar.Get) || !i.Result.Equals(Scalar.Get) || i.Parameters.Any(i => !i.Equals(Scalar.Get)))
                .Select(i => new BinaryOperaorModel(i.Result.Name == "null" ? (Unit1.Name) : i.Result.Name,
                    Unit1.Name,
                    $"operator {i.Operator.Symbol}",
                    $"Op{i.Operator.Name}",
                    i.Parameters[0].Name))
                .ToList();
            UnaryEdges = state
                .GraphEdges[forUnit]
                .Where(i => i.Operator is UnaryOperator)
                .Where(i => i.Parameters.All(j => state.Units.Contains(j)))
                .Select(i => (i.Result.Name, i.Operator.Name))
                .ToList();
            Valid = true;
            State = state;
        }
        public string Parameters(OperatorNodeEdge i) => i.Parameters.Select((i, j) => $"{i} u{j}")
            .Aggregate((i, j) => $"{i}, {j}");
    }

    public struct BinaryOperaorModel
    {
        public string returnType;
        public string l;
        public string method;
        public string name;
        public string r;

        public BinaryOperaorModel(string returnType, string l, string method, string name, string r)
        {
            this.returnType = returnType;
            this.l = l;
            this.method = method;
            this.name = name;
            this.r = r;
        }
    }
}
