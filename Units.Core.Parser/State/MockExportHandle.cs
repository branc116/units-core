namespace Units.Core.Parser.State
{
    public class MockExportHandle : IExportHandle
    {
        public bool ExportUnits(ParserState state, string location)
        {
            return true;
        }

        public bool ExportWrappers(ParserState state, string location)
        {
            return true;
        }
    }
}
