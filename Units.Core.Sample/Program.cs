using System;
namespace Units.Core.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var definition = @"
Base(Unit) := Mass | Length | Time | Temperature | ElectricCurent | AmountOfSubstance | LuninusIntensity
Operators(Binary) := (*, Times, 1, 1) | (/, Per, 1, -1)
SelfOps := (<, Lt, bool) | (<=, Let, bool) | (>, Gt, bool) | (>=, Get, bool) | (==, Eq, bool) | (!=, Ne, bool) | (+, Plus, null) | (-, Minus, null) | (*, Times, null) | (/, Per, null)
Real(Types) := (float, RealFloat)

Operator(*) := a = b * c | a = c * b | c = a / b | b = a / c
Operator(/) := a = b / c | c = b / a | b = a * c | b = c * a

Speed := Length / Time
Acceleration := Speed / Time
Area := Length * Length
Volume := Area * Length
Force := Mass * Acceleration
Pressure := Force / Area
Energy := Force * Length

Infer
Infer

Unit(Mass) := gram(i, g)
Unit(Length) := meter(i, m)
Unit(Time) := second(i, s) | hour(i/3600, h) | minute(i/60, min)
Unit(Temperature) := kelvin(i, K) | celsius(i + 273, ˙C)
Unit(Force) := newton(i, N)
";
            var lines = definition.Split(Environment.NewLine);
            var state = Parser.Parser.Parse(lines);
            var generator = new GenerateUnits(lines);
            var res = generator.TransformText();
            System.IO.File.WriteAllText("Out_gen.cs", res);
        }
    }
}
