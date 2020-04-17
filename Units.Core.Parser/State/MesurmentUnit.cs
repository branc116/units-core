using System;
using System.Collections.Generic;

namespace Units.Core.Parser.State
{
    public class MesurmentUnit : IEquatable<MesurmentUnit>
    {
        public IUnit For { get; set; }
        public string Name { get; set; }
        public string Derive { get; set; }
        public string Postfix { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as MesurmentUnit);
        }

        public bool Equals(MesurmentUnit other)
        {
            return other != null &&
                   EqualityComparer<IUnit>.Default.Equals(For, other.For) &&
                   Name == other.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(For, Name);
        }
    }
}
