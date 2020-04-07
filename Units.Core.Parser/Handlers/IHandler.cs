using System.Text.RegularExpressions;
using Units.Core.Parser.State;

namespace Units.Core.Parser.Handlers
{
    /// <summary>
    /// Interface every Handler shuld implement.
    /// Handlers job is to provide the <see cref="System.Text.RegularExpressions.Regex"/> definition for the inputs it can handle <see cref="MatchRegex(ParserState)"/>.
    /// When Handler can handle the line, <see cref="Handle(ParserState, string)"/> will be called by <see cref="Parser.Parse(string[])"/>
    /// </summary>
    public interface IHandler
    {
        /// <summary>
        /// Defines how the <paramref name="input"/> will change the <paramref name="parserState"/>
        /// </summary>
        /// <param name="parserState">Current parser state</param>
        /// <param name="input">Current line</param>
        /// <returns><see cref="true"/> if the handling was succesful, else <see cref="false"/></returns>
        bool Handle(ParserState parserState, string input);
        /// <summary>
        /// Based on current <paramref name="parserState"/> generate <see cref="Regex"/> that checks if <see cref="this"/> can handle the input.
        /// </summary>
        /// <param name="parserState">Current parser state</param>
        /// <returns>Regex that cheks if the line is something <see cref="this"/> can handle.</returns>
        Regex MatchRegex(ParserState parserState);
    }
}
