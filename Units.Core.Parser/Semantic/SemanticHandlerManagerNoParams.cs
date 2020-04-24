using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Units.Core.Parser.Semantic
{
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
