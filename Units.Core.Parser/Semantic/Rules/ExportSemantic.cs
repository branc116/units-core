using Units.Core.Parser.State;

namespace Units.Core.Parser.Semantic.Rules
{
    [Semantic("Export")]
    public class ExportSemantic : SemanticAbstract
    {
        public ExportSemantic(ParserState state) : base(state) { }
        [Semantic("Units")]
        public void Handle(string outLoc)
        {
            _state.Exporter.ExportUnits(_state, outLoc);
        }
        [Semantic("RealWrapers")]
        public void HandleWrapers(string outLoc)
        {
            _state.Exporter.ExportWrappers(_state, outLoc);
        }
        [Semantic("All")]
        public void HandleAll(string outLoc)
        {
            _state.Exporter.ExportAll(_state, outLoc);
        }
    }
}
