using System.Text.RegularExpressions;
using Units.Core.Parser.State;

namespace Units.Core.Parser.Handlers
{
    public interface IHandler
    {
        bool Handle(ParserState parserState, string input);
        Regex MatchRegex(ParserState parserState);
    }
}
