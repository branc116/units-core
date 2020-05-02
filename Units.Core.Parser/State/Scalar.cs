namespace Units.Core.Parser.State
{
    /// <summary>
    /// Unit that has no dimension
    /// </summary>
    public class Scalar : IUnit<Scalar>
    {
        public static Scalar Get = new Scalar();
        private static int HashCode => Get.Name.GetHashCode();
        public string Name => "Scalar";

        public bool IsInfered => false;

        private Scalar() { }
        public IUnit Simplify() => this;
        public string SiName() => Name;
        public IUnit WithSiName() => this;
        public G WithSiName<G>(bool cache) where G : Scalar => (G)this;
        public override int GetHashCode() => HashCode;
        public override bool Equals(object obj) => Equals(obj as Scalar) || Equals(obj as IUnit);
        public static bool Equals(Scalar sc) => sc is { };
        public static bool Equals(IUnit unit) => unit?.Equals(Get) ?? false;

        public IUnit Rename(string newName) => this;
    }
}
