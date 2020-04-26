using System.Collections.Generic;


namespace Units.Core.Generators
{
    using System.Linq;
    using Units.Core.Parser.State;
    public partial class GenerateEdgeOperators
    {
        public IUnit Unit1 { get; set; }
        public IEnumerable<(string returnType, string l, string method, string name, string r)> Operators { get; }
        public List<(string returnType, string name)> UnaryEdges { get; }
        public bool Valid { get; } = false;

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
                .Select(i => (i.Result.Name == "null" ? (Unit1.Name) : i.Result.Name,
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
        }
        public string Parameters(OperatorNodeEdge i) => i.Parameters.Select((i, j) => $"{i} u{j}")
            .Aggregate((i, j) => $"{i}, {j}");
    }

}
