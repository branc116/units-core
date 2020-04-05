using System;

namespace Units.Core.Parser.State
{
    public class Unit : IEquatable<Unit>
    {
        public string Name { get; set; }
        public virtual string SiName()
        {
            return Name;
        }
        public virtual Unit Clone()
        {
            return new Unit
            {
                Name = $"{Name}"
            };
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as Unit);
        }

        public bool Equals(Unit other)
        {
            return other != null &&
                   Name == other.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
