using Units.Core.Parser.State;

namespace Units.Core.Parser.Semantic.Rules
{
    [Semantic("Real")]
    public class RealSematic : SemanticAbstract
    {
        public RealSematic(ParserState state) : base(state) { }

        [Semantic("Types")]
        public void Handle(string clrTypeName, string wrapperName)
        {
            var t = new RealDef
            {
                ClrName = clrTypeName,
                WrapName = wrapperName
            };
            _state.RealDefs.Add(t);
        }
    }
}
