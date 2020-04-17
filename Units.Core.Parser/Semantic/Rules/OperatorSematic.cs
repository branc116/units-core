using System.Collections.Generic;
using System.Linq;
using Units.Core.Parser.Semantic.Models;
using Units.Core.Parser.State;

namespace Units.Core.Parser.Semantic.Rules
{
    [Semantic("Operator")]
    public class OperatorSematic : SemanticAbstract
    {
        public OperatorSematic(ParserState state) : base(state) { }
        [Semantic(SemanticMatch.All, SemanticHandleKind.Bulk)]
        public void Handle(string @for, Dictionary<char, int> order, List<OperatorDef_Binary> binaries, List<OperatorDef_Unary> unaries)
        {
            var op = _state.GetOperator(@for);
            op.InferedOperations = binaries
                .Select(i => (order[i.Left], _state.GetOperator(i.Symbol.ToString()), order[i.Right], order[i.Res])).ToList();
        }
    }
}
