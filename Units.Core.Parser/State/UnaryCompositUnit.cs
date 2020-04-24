namespace Units.Core.Parser.State
{
    public class UnaryCompositUnit : ReadonlyUnit<UnaryCompositUnit>
    {
        public IUnit Unit { get; }
        public UnaryOperator UnaryOperator { get; }
        public UnaryCompositUnit(IUnit unit, UnaryOperator unaryOperator, string name) : base(name)
        {
            UnaryOperator = unaryOperator;
            Unit = unit.Simplify();
        }
        public override IUnit Simplify()
        {
            if (Unit is Scalar)
                return Scalar.Get;
            return this;
        }

        public override string SiName()
        {
            return BinaryCompositUnit.SiName(this);
        }

        public override IUnit WithSiName()
        {
            return new UnaryCompositUnit(Unit, UnaryOperator, SiName());
        }
    }
}
