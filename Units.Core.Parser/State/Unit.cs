namespace Units.Core.Parser.State
{
    /// <summary>
    /// Base unit
    /// </summary>
    /// <remarks>
    /// Maybe should be called BaseeUnit or BaseDimension or SiUnit...
    /// </remarks>
    public class Unit : ReadonlyUnit<Unit>
    {
        public Unit(string name) : base(name) { }

        public override IUnit Rename(string newName)
        {
            return new Unit(newName);
        }

        public override IUnit Simplify() =>
            this;

        /// <inheritdoc/>
        public override string SiName() =>
            Name;
        /// <summary>
        /// Clone the unit
        /// </summary>
        /// <returns>The same unit but different reference</returns>
        public override IUnit WithSiName() =>
            this;
    }
}
