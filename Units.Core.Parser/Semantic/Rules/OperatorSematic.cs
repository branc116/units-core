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
            var infers = binaries
                .Select(i => (
                    res: order[i.Res],
                    op: _state.GetOperator(i.Symbol.ToString()),
                    operands: new[] { order[i.Left], order[i.Right] }
                )).Union(unaries.Select(i => (
                    res: order[i.Res],
                    op: _state.GetOperator(i.Operator),
                    operands: new[] { order[i.Res] }
                ))).ToList();
            foreach (var infer in infers)
            {
                op.AddInferedOperatorion(infer.res, infer.op, infer.operands);
            }
        }
    }
}
