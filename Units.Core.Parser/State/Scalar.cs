namespace Units.Core.Parser.State
{
    public class Scalar : Unit
    {
        public Scalar()
        {
            Name = "Scalar";
        }
        public override Unit Clone()
        {
            return new Scalar();
        }
    }
}
