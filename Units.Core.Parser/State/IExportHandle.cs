namespace Units.Core.Parser.State
{
    public interface IExportHandle
    {
        bool ExportAll(ParserState state, string location) =>
            ExportUnits(state, location) && ExportWrappers(state, location);
        bool ExportUnits(ParserState state, string location);
        bool ExportWrappers(ParserState state, string location);
    }
}
