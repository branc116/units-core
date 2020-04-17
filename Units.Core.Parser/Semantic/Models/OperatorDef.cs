using System.Collections.Generic;
using System.Linq;

namespace Units.Core.Parser.Semantic.Models
{
    public class OperatorDef
    {
        public List<OperatorDef_Binary> Binaries { get; } = new List<OperatorDef_Binary>();
        public List<OperatorDef_Unary> Unaries { get; } = new List<OperatorDef_Unary>();
        public Dictionary<char, int> GetOrder()
        {
            if (Binaries.Any())
            {
                var first = Binaries.First();
                var order = new[] { first.Res, first.Left, first.Right }.Distinct().ToIndexed().ToDictionary(i => i.value, i => i.index);
                return order;
            }
            else if (Unaries.Any())
            {
                var first = Unaries.First();
                var order = new[] { first.Res, first.Right }.ToIndexed().ToDictionary(i => i.value, i => i.index);
                return order;
            }
            else
            {
                return new Dictionary<char, int>();
            }
        }
    }
}
