using System;
namespace Units.Core.Parser.State
{
    public abstract class ReadonlyUnit<T> : IUnit<T>, IEquatable<ReadonlyUnit<T>> where T : ReadonlyUnit<T>
    {
        private IUnit _simplifyed;
        private string _siName;
        private IUnit _si;
        /// <summary>
        /// Name of the unit
        /// </summary>
        public string Name { get; }
        protected ReadonlyUnit(string name)
        {
            Name = name;
        }
        public IUnit Simplify(bool cache) => cache ?
            _simplifyed ??= Simplify() :
            Simplify();

        public abstract IUnit Simplify();
        public string SiName(bool cache) => cache ?
            _siName ??= SiName() :
            SiName();
        public abstract string SiName();
        public G WithSiName<G>(bool cache) where G : T => (G)(cache ?
            _si ??= WithSiName() :
            WithSiName());
        public abstract IUnit WithSiName();

        public override bool Equals(object obj) =>
            Equals(obj as ReadonlyUnit<T>) ||
            Equals(obj as IUnit);

        public bool Equals(ReadonlyUnit<T> other) =>
            other != null && other.SiName(true) == SiName(true);
        public bool Equals(IUnit other) =>
            other != null && other.SiName() == SiName(true);
        private int? _hashCode;
        public override int GetHashCode() =>
            _hashCode ??= SiName(true).GetHashCode();
        public override string ToString() =>
            Name ?? SiName(true);
    }
}