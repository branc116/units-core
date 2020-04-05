using System;
using System.Linq;
using Units.Core.Parser.State;

namespace Units.Core.Parser
{
    public static class Parser
    {
        public static ParserState Parse(string[] lines)
        {
            var state = new ParserState();
            var handlers = Helpers.GetHandlers().ToList();
            var i = 0;
            foreach (var line in lines)
            {
                i++;
                var s = line;
                var handler = handlers.FirstOrDefault(j => j.MatchRegex(state).IsMatch(s));
                if (handler is null)
                {
                    Console.WriteLine($"Error on line {i}: {s} is not valid");
                    continue;
                }
                try
                {
                    handler.Handle(state, s);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error on line {i}: {ex}");
                }
            }
            state.Units.Remove(new Scalar());
            return state;
        }

    }
}
