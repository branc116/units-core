using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Units.Core.Parser.State;

namespace Units.Core.Parser.Tests
{
    [TestClass]
    public class ParserTestsSemantic
    {
        [TestMethod]
        public void TestEverything()
        {
            var input = @"
Units(Base) := Mass | Length | Time | Temperature | ElectricCurent | AmountOfSubstance | LuninusIntensity
Operators(Binary) := (*, Times, 1, 1) | (/, Per, 1, -1)
Operators(Unary) := (Sq, 2) | (Sqrt, 0.5)
Real(Types) := (float, RealFloat)

Operator(*) := a = b * c | a = c * b | c = a / b | b = a / c
Operator(/) := a = b / c | c = b / a | b = a * c | b = c * a
Operator(Sq) := a = Sq b | a = b * b | b = Sqrt a

Speed := Length / Time
Acceleration := Speed / Time
Area := Sq Length
Volume := Area * Length
Force := Mass * Acceleration
Pressure := Force / Area
Energy := Force * Length

Infer

Unit(Mass) := (gram, i, g)
Unit(Length) := (meter, i, m)
Unit(Time) := (second, i, s) | (hour, i/3600, h) | (minute, i/60, min)
Unit(Temperature) := (kelvin, i, K) | (celzus, i + 273, ˙C)
Unit(Force) := (newton, i, N)
";
            var res = Parser.ParseGrammarString(input);
        }
        [TestMethod]
        public void TestOperatorInfer()
        {
            var input = @"
Operators(Binary) := (*, Times, 1, 1) | (/, Per, 1, -1)
Operator(*) := a = b * c | a = c * b | c = a / b | b = a / c";
            var l1 = new Unit("1");
            var l2 = new Unit("2");
            var l3 = new Unit("3");
            var res = Parser.ParseGrammarString(input);
            var times = res.GetOperator("*");
            Assert.IsNotNull(times);
            var per = res.GetOperator("/");
            var infersTimes = times.GetInferedOperations(l1, l2, l3).ToList();
            var infersPer = per.GetInferedOperations(l1, l2, l3).ToList();

            Assert.IsNotNull(per);
            Assert.IsNotNull(infersTimes);
            Assert.IsNotNull(infersPer);
            Assert.AreEqual(4, infersTimes.Count);
            Assert.AreEqual(1, infersPer.Count);

            Assert.IsTrue(infersTimes.Any(i => i.res.Equals(l1) && i.@params[0].Equals(l2) && i.@params[1].Equals(l3) && i.@operator.Equals(times)));
            Assert.IsTrue(infersTimes.Any(i => i.res.Equals(l1) && i.@params[0].Equals(l3) && i.@params[1].Equals(l2) && i.@operator.Equals(times)));
            Assert.IsTrue(infersTimes.Any(i => i.res.Equals(l3) && i.@params[0].Equals(l1) && i.@params[1].Equals(l2) && i.@operator.Equals(per)));
            Assert.IsTrue(infersTimes.Any(i => i.res.Equals(l2) && i.@params[0].Equals(l1) && i.@params[1].Equals(l3) && i.@operator.Equals(per)));

            Assert.IsTrue(infersPer.Any(i => i.res.Equals(l1) && i.@params[0].Equals(l2) && i.@params[1].Equals(l3) && i.@operator.Equals(per)));
        }
        [TestMethod]
        public void TestUnaryOperator()
        {
            var input = @"
Units(Base) := Mass | Length | Time | Temperature | ElectricCurent | AmountOfSubstance | LuninusIntensity
Operators(Binary) := (*, Times, 1, 1) | (/, Per, 1, -1)
Operators(Unary) := (Sq, 2) | (Sqrt, 0.5)
Real(Types) := (float, RealFloat)

Operator(*) := a = b * c | a = c * b | c = a / b | b = a / c
Operator(/) := a = b / c | c = b / a | b = a * c | b = c * a
Operator(Sq) := a = Sq b | a = b * b | b = Sqrt a

Speed := Length / Time
Acceleration := Speed / Time
Area := Sq Length
Volume := Area * Length
Force := Mass * Acceleration
Pressure := Force / Area
Energy := Force * Length

Infer

Unit(Mass) := (gram, i, g)
Unit(Length) := (meter, i, m)
Unit(Time) := (second, i, s) | (hour, i/3600, h) | (minute, i/60, min)
Unit(Temperature) := (kelvin, i, K) | (celzus, i + 273, ˙C)
Unit(Force) := (newton, i, N)
";
            var res = Parser.ParseGrammarString(input);
            var area = new BinaryCompositUnit(new Unit("Length"), BinaryOperator.TIMES, new Unit("Length"));
            Assert.AreEqual(area.SiName(), res.GetUnit("Area").SiName());
        }
    }
}
