using Units.Core.Parser.State;

namespace Units.Core.Parser.Semantic.Rules
{
    [Semantic("NewUnit")]
    public class NewUnitSemantic : SemanticAbstract
    {
        public NewUnitSemantic(ParserState state) : base(state) { }
        [Semantic("Binary")]
        public void Handle(string newUnitName, string leftUnit, string @operator, string rightUnit)
        {
            _state.AddNewCompositUnit(leftUnit, @operator, rightUnit, newUnitName);
        }
        [Semantic("Unary")]
        public void Handle(string newUnitName, string @operator, string rightUnit)
        {
            _state.AddNewCompositUnit(@operator, rightUnit, newUnitName);
        }
    }
}
