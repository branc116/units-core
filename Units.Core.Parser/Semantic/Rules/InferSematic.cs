using System.Linq;
using Units.Core.Parser.State;

namespace Units.Core.Parser.Semantic.Rules
{
    [Semantic("Infer")]
    public class InferSematic : SemanticAbstract
    {
        public InferSematic(ParserState state) : base(state) { }

        [Semantic("Slow")]
        public void Handle()
        {
            var state = _state;
            var units = state.Units.ToList();
            var operators = state.Operators.Where(i => i is BinaryOperator).ToList();
            var newUnits = units.Join(units, i => true, i => true, (left, right) => (left, right))
                .Join(operators, i => true, i => true, (us, ops) => (us.left, ops, us.right));
            foreach (var (left, op, right) in newUnits)
            {
                state.AddNewCompositUnit(left, op, right, true);
            }
        }
        [Semantic("Fast")]
        public void HandleFast()
        {
            var state = _state;
            var units = state.Units.ToList();
            var operators = state.Operators.Where(i => i is BinaryOperator).ToList();
            var newUnits = units.Join(units, i => true, i => true, (left, right) => (left, right))
                .Join(operators, i => true, i => true, (us, ops) => (us.left, ops, us.right))
                .Where(i => !i.right.IsInfered);
            foreach (var (left, op, right) in newUnits)
            {
                state.AddNewCompositUnit(left, op, right, true, true);
            }
        }
    }
}
