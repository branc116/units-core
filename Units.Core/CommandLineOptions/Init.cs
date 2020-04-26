using System.IO;
using CommandLine;

namespace Units.Core.CommandLineOptions
{
    public class Init
    {
        [Verb("Init", HelpText = "Generate initial units file from witch you can generate classes")]
        public class InitOptions
        {
            [Option('p', "path", Default = "units.txt", HelpText = "Relative path where the units file will be generated")]
            public string Path { get; set; }
        }
        public enum InitType
        {
            FullSiStandardAllOperators,
            Empty
        }
        public InitOptions Options { get; }
        public Init(InitOptions options)
        {
            Options = options;
        }
        public bool DoIt()
        {
            var path = Options.Path;
            var text = FullSiStandard;
            File.WriteAllText(path, text);
            return true;
        }
        public const string FullSiStandard = @"
Operators(Binary) := (*, Times, 1, 1) | (/, Per, 1, -1)
Operators(Self) := (*, Times, null) | (/, Per, null)
Operators(Self) := (<, Lt, bool) | (<=, Let, bool) | (>, Gt, bool) | (>=, Get, bool) | (==, Eq, bool) | (!=, Ne, bool) | (+, Plus, null) | (-, Minus, null)
Real(Types) := (float, RealFloat)

Operator(*) := a = b * c | a = c * b | c = a / b | b = a / c
Operator(/) := a = b / c | c = b / a | b = a * c | b = c * a


Units(Base) := Mass | Length | Time | ThermodynamicTemperature | ElectricCurent | AmountOfSubstance | LuninusIntensity

Speed := Length / Time
Acceleration := Speed / Time
Area := Length * Length
Volume := Area * Length
Force := Mass * Acceleration
Pressure := Force / Area
Energy := Force * Length

!Infer

!Export All, ./MyUnits";
    }
}
