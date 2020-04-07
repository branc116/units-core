namespace Units.Core.Parser.State
{
    /// <summary>
    /// Unit that has not 
    /// </summary>
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
