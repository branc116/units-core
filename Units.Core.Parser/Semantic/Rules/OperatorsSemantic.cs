using Units.Core.Parser.State;

namespace Units.Core.Parser.Semantic.Rules
{
    [Semantic("Operators")]
    public class OperatorsSemantic : SemanticAbstract
    {
        public OperatorsSemantic(ParserState state) : base(state) { }
        [Semantic("Binary")]
        public void HandleBinary(string symbol, string name, string leftCount, string rightCount)
        {
            var op = new Operator(name, symbol)
            {
                CountLeft = double.TryParse(leftCount, out var d) ? (d, default) : (double.Parse(leftCount[0..^1]), leftCount[^1]),
                CountRight = double.TryParse(rightCount, out var d1) ? (d1, default) : (double.Parse(rightCount[0..^1]), rightCount[^1])
            };
            _state.Operators.Add(op);
        }
        [Semantic("Unary")]
        public void HandleUnary(string name, string rightCount)
        {
            //TODO
        }
        [Semantic("Self")]
        public void Handle(string symbol, string name, string retName)
        {
            var selfOp = new SelfOp { Name = name, RetType = retName, Symbol = symbol };
            _state.SelfOps.Add(selfOp);
        }
    }
}
