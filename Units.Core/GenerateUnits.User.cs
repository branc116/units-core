namespace Units.Core
{
    /// <summary>
    /// Generate struct definitions from <see cref="Lines"/>
    /// </summary>
    public partial class GenerateUnits
    {
        /// <summary>
        /// Every line should be handle by one implementation of <see cref="Parser.Handlers.IHandler" />
        ///This shuld be something like:
        ///</summary>
        ///<remarks>
        ///
        ///<h1>This is what the code for defining units</h1>
        ///
        ///<code>
        ///Base(Unit) := Mass | Length | Time | Temperature | ElectricCurent | AmountOfSubstance | LuninusIntensity
        ///Operations := (*, Times) | (/, Per)
        ///SelfOps := (==, Eq, bool) | (!=, Ne, bool) | (+, Plus, null) | (-, Minus, null) | (*, Times, null) | (/, Per, null)
        ///Real(Types) := (float, RealFloat)
        ///
        ///Operator(*) := a = b * c | a = c * b | c = a / b | b = a / c
        ///Operator(/) := a = b / c | c = b / a | b = a * c | b = c * a
        ///
        ///Speed := Length / Time
        ///Acceleration := Speed / Time
        ///Area := Length * Length
        ///Volume := Area * Length
        ///Force := Mass * Acceleration
        ///Pressure := Force / Area
        ///Energy := Force * Length
        ///
        ///Infer
        ///
        ///Unit(Mass) := gram(i, g)
        ///Unit(Length) := meter(i, m)
        ///Unit(Time) := second(i, s) | hour(i/3600, h) | minute(i/60, min)
        ///Unit(Temperature) := kelvin(i, K) | celsius(i + 273, ˙C)
        ///Unit(Force) := newton(i, N)
        ///</code>
        ///
        ///<h1>This is what the code for defining units will look like</h1>
        ///
        ///<code>
        ///Units(Base) := Mass | Length | Time
        ///Units(Prefixes) := (mili, i/1000) | (micro, i/10e6) | (kilo, i*1000) | (mega, i*1e6)
        ///Operators(Binary) := (*, Times, 1, 1) | (/, Per, 1, -1)
        ///Operators(Unary) := (Square, 2) | (SquareRoot, 0.5)
        ///Operators(Self) := (==, Eq, bool) | (+, Plus, null) | (-, Minus, null) | (*, Times, null) | (/, Per, null)
        ///
        ///Real(Types) := (System.Single, RealFloat) | (Godot.Vector3, Vec3)
        ///
        ///Operator(*) := a = b * c | a = c * b | c = a / b | b = a / c
        ///Operator(/) := a = b / c | c = b / a | b = a * c | b = c * a
        ///Operator(Square) := a = Square b | b = SquareRoot a | a = b * b
        ///Operator(SquareRoot) := a = SquareRoot b | b = Square a | b = a * a
        ///
        ///Speed := Length / Time
        ///Acceleration := Speed / Time
        ///Area := Length * Length
        ///Volume := Area * Length
        ///Force := Mass * Acceleration
        ///Pressure := Force / Area
        ///Energy := Force * Length
        ///
        ///!Infer
        ///
        ///Unit(Mass) := gram(i, g)
        ///Unit(Length) := meter(i, m)
        ///Unit(Time) := second(i, s) | hour(i/3600, h) | minute(i/60, min)
        ///Unit(Temperature) := kelvin(i, K) | celsius(i + 273, ˙C)
        ///Unit(Force) := newton(i, N)
        ///
        ///!Export Units ./Units.cs
        ///!Export RealWrapers ./Wrapers.cs
        ///</code>
        ///</remarks>
        public string[] Lines { get; set; }
        public GenerateUnits(string[] lines)
        {
            Lines = lines;
        }
    }
}
