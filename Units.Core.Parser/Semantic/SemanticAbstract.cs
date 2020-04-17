using Units.Core.Parser.State;

namespace Units.Core.Parser.Semantic
{
    public abstract class SemanticAbstract : ISemantic
    {
        protected readonly ParserState _state;

        protected SemanticAbstract(ParserState state)
        {
            _state = state;
        }
    }
}
