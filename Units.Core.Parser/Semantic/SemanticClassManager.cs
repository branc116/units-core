using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Units.Core.Parser.State;

namespace Units.Core.Parser.Semantic
{
    public class SemanticClassManager
    {
        private readonly ParserState _state;

        public SemanticMethodManager this[string className] => GetMethodManager(className);
        public SemanticClassManager(ParserState state)
        {
            _state = state;
        }

        private SemanticMethodManager GetMethodManager(string className)
        {
            var mmt = typeof(SemanticClassManager).Assembly
                .GetTypes()
                .FirstOrDefault(i => i.GetCustomAttribute<SemanticAttribute>()?.IsMatch(className) ?? false);
            if (mmt is null)
                throw new HandleException(@$"
'{className}' is not valid function.
Valid functions in this scope are {GetValidFunctionsStr()}.
", 2225);
            return new SemanticMethodManager(mmt, _state);
        }
        public List<string> GetValidFunctions()
        {
            var valid = typeof(SemanticClassManager).Assembly
                .GetTypes()
                .Select(i => i.GetCustomAttribute<SemanticAttribute>())
                .Where(i => i is { })
                .Select(i => i.Name)
                .ToList();
            return valid;
        }
        public string GetValidFunctionsStr()
        {
            var ret = GetValidFunctions() is { Count: { } c } valids && c > 0 ?
                valids.Select(i => $"'{i}'")
                    .Aggregate((i, j) => $"{i}, {j}") :
                string.Empty;
            return ret;
        }

    }
}
