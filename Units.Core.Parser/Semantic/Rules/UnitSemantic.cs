using Units.Core.Parser.State;

namespace Units.Core.Parser.Semantic.Rules
{
    [Semantic("Unit")]
    public class UnitSemantic : SemanticAbstract
    {
        public UnitSemantic(ParserState state) : base(state) { }
        [Semantic(SemanticMatch.All)]
        public void Handle(string @for, string name, string expr, string sym)
        {
            var unit = _state.GetUnit(@for);
            _state.MesurmentUnits.Add(new MesurmentUnit()
            {
                ConvertTo = expr,
                For = unit,
                Name = name,
                Postfix = sym
            });
        }
    }
}
