using System;
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

    public class SemanticHandlerManager
    {
        private readonly MethodInfo _meth;
        private readonly ISemantic _instance;
        private readonly SemanticAttribute _attribute;

        public SemanticHandlerManager(MethodInfo meth, ISemantic instance, SemanticAttribute attribute)
        {
            _meth = meth;
            _instance = instance;
            _attribute = attribute;
        }

        public void Invoke(params object[] arr)
        {
            _meth.Invoke(_instance, arr);
        }
        public void InvokeRaw(params List<object>[] arr)
        {
            if (_attribute.SemanticHandleKind == SemanticHandleKind.Bulk)
            {
                _meth.Invoke(_instance, arr);
                return;
            }
            foreach (var e in arr)
            {
                _meth.Invoke(_instance, e.ToArray());
            }
        }
    }
    public class SemanticHandlerManagerNoParams
    {
        private readonly List<(MethodInfo meth, SemanticAttribute attr)> _meths;
        private readonly ISemantic _instance;

        public SemanticHandlerManagerNoParams(List<(MethodInfo, SemanticAttribute)> meths, ISemantic instance)
        {
            _meths = meths;
            _instance = instance;
        }

        public void Invoke(params object[] arr)
        {
            var meth = _meths.FirstOrDefault(i => i.meth.GetParameters().Length == arr.Length);
            if (meth.meth is null)
            {
                throw new HandleException($"Can't handle function with {arr.Length} parameters.", 2333);
            }
            meth.meth.Invoke(_instance, arr);
        }
        public void InvokeRaw<T>(string methodName, params List<T>[] arr)
        {
            foreach (var a in arr)
            {
                var (meth, attr) = _meths.FirstOrDefault(i =>
                {
                    if (i.attr.SemanticMatch == SemanticMatch.All)
                    {
                        return i.meth.GetParameters().Length == a.Count + 1;
                    }
                    return i.meth.GetParameters().Length == a.Count;
                });
                if (meth is null)
                {
                    throw new HandleException($"Can't handle function with {a.Count} parameters.", 2339);
                }
                if (attr.SemanticMatch == SemanticMatch.All)
                {
                    meth.Invoke(_instance, a.Cast<object>().Prepend((object)methodName).ToArray());
                    return;
                }
                meth.Invoke(_instance, a.Cast<object>().ToArray());
            }
            //TODO bulk
        }
    }
}
