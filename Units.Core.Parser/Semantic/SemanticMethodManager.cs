using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Units.Core.Parser.State;

namespace Units.Core.Parser.Semantic
{
    public class SemanticMethodManager
    {
        private readonly ParserState _state;
        public ISemantic Instance { get; }
        public Type Type { get; }
        public SemanticHandlerManager this[string name, int paramNum] => GetHandlerManager(name, paramNum);
        public SemanticHandlerManagerNoParams this[string name] => GetHandlerManager(name);
        public SemanticMethodManager(Type type, ParserState _state)
        {
            Type = type;
            Instance = type.GetConstructor(new[] { typeof(ParserState) }).Invoke(new[] { _state }) as ISemantic;
            this._state = _state;
        }
        public SemanticHandlerManager GetHandlerManager(string name, int paramNum)
        {
            var meth = Type.GetMethods().Where(i => i.GetCustomAttribute<SemanticAttribute>()?.IsMatch(name) ?? false)
                .FirstOrDefault(i => i.GetParameters().Length == paramNum);
            if (meth is null)
                throw new HandleException($@"
{name} with {paramNum} params is not valid function.
Valid functions in this scope are {GetValidFunctionsStr()}.
", 2232);
            return new SemanticHandlerManager(meth, Instance, meth.GetCustomAttribute<SemanticAttribute>());
        }
        public SemanticHandlerManagerNoParams GetHandlerManager(string name)
        {
            var meth = Type.GetMethods().Where(i => i.GetCustomAttribute<SemanticAttribute>()?.IsMatch(name) ?? false)
                .Select(i => (i, i.GetCustomAttribute<SemanticAttribute>()))
                .ToList();
            if (meth is null)
                throw new HandleException($@"
{name} is not valid function.
Valid functions in this scope are {GetValidFunctionsStr()}.
", 2343);
            return new SemanticHandlerManagerNoParams(meth, Instance);
        }
        public List<(string name, int paramNum)> GetValidFunctions()
        {
            var valids = Type.GetMethods()
                .Select(i => (meth: i, attr: i.GetCustomAttribute<SemanticAttribute>()))
                .Where(i => i.attr != null)
                .Select(i => (i.attr.Name, i.meth.GetParameters().Length))
                .ToList();
            return valids;
        }
        public string GetValidFunctionsStr()
        {
            var ret = GetValidFunctions() is { Count: { } c } valids && c > 0 ?
                valids.Select(i => $"{i.name}({i.paramNum})")
                .Aggregate((i, j) => $"{i}, {j}") :
                string.Empty;
            return ret;
        }
    }
}
