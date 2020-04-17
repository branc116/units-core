using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Units.Core.Parser.State;
namespace Units.Core.Parser.Tests
{
    [TestClass]
    public class ParserTests
    {
        private static Operator _times = new Operator("Times", "*")
        {
            CountLeft = (1, null),
            CountRight = (1, null)
        };
        private static Operator _over = new Operator("Over", "/")
        {
            CountLeft = (1, null),
            CountRight = (-1, null)
        };

        [TestMethod]
        public void ATimesB_Equals_BTimesA()
        {
            var u1 = new Unit("Lenght");
            var u2 = new Unit("Mass");
            var u3 = new CompositUnit(u1, _times, u2, null);
            var u4 = new CompositUnit(u2, _times, u1, null);
            Assert.AreEqual(u3, u4);
        }
        [TestMethod]
        public void APerA_Equals_Scalar()
        {
            var u1 = new Unit("Lenght");
            var u3 = new CompositUnit(u1, _over, u1);
            Assert.AreEqual(u3.SiName(), Scalar.Get.SiName());
            Assert.AreEqual(u3, Scalar.Get);
        }
        [TestMethod]
        public void ScalarTimesScalar_Eqals_Scalar()
        {
            var u1 = Scalar.Get;
            var u2 = Scalar.Get;
            var u3 = new CompositUnit(u1, _times, u2);
            Assert.AreEqual(u3, u1);
            Assert.AreEqual(u3, u2);
            Assert.AreEqual(u2, u3);
        }
        [TestMethod]
        public void CheckHandlersCount()
        {
            var handles = Helpers.GetHandlers().Count();
            Assert.AreEqual(8, handles);
        }
        [TestMethod]
        public void TestEverything()
        {
            var input = @"
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

Unit(Mass) := gram(i, g)
Unit(Length) := meter(i, m)
Unit(Time) := second(i, s) | hour(i/3600, h) | minute(i/60, min)
Unit(Temperature) := kelvin(i, K) | celzus(i + 273, ˙C)
Unit(Force) := newton(i, N)
";
            var res = Parser.Parse(input.Split(new[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
