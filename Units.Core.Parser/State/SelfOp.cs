using System;

namespace Units.Core.Parser.State
{
    public class SelfOp : IEquatable<SelfOp>
    {
        public string Name { get; set; }
        public string Symbol { get; set; }
        public string RetType { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as SelfOp);
        }

        public bool Equals(SelfOp other)
        {
            return other != null &&
                   Name == other.Name &&
                   Symbol == other.Symbol;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Symbol);
        }
    }
}
