using Units.Core.Parser.State;

namespace Units.Core.Parser.Semantic.Rules
{
    [Semantic("Units")]
    public class BaseSemantic : SemanticAbstract
    {
        public BaseSemantic(ParserState state) : base(state) { }
        [Semantic("Base")]
        public void HandleBase(string name)
        {
            _state.AddUnit(name);
        }
        [Semantic("Prefixes")]
        public void HandlePefixes(string name, string expression)
        {
            //todo
        }
    }
}
