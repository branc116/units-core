using System.Collections.Generic;
using System.Reflection;

namespace Units.Core.Parser.Semantic
{
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
}
