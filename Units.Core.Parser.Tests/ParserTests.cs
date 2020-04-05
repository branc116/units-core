using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Units.Core.Parser.State;
namespace Units.Core.Parser.Tests
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void ATimesB_Equals_BTimesA()
        {
            var u1 = new Unit { Name = "Lenght" };
            var u2 = new Unit { Name = "Mass" };
            var u3 = new CompositUnit { Operator = new Operator { Name = "Times", Symbol = "*" }, Unit1 = u1, Unit2 = u2 };
            var u4 = new CompositUnit { Operator = new Operator { Name = "Times", Symbol = "*" }, Unit1 = u2, Unit2 = u1 };
            Assert.AreEqual(u3, u4);
        }
        [TestMethod]
        public void APerA_Equals_Scalar()
        {
            var u1 = new Unit { Name = "Lenght" };
            var u3 = new CompositUnit { Operator = new Operator { Name = "Times", Symbol = "/" }, Unit1 = u1, Unit2 = u1 };
            Assert.AreEqual(u3, new Scalar());
        }
        [TestMethod]
        public void ScalarTimesScalar_Eqals_Scalar()
        {
            var u1 = new Scalar();
            var u2 = new Scalar();
            var u3 = new CompositUnit { Operator = new Operator { Name = "Times", Symbol = "*" }, Unit1 = u1, Unit2 = u2 };
            Assert.AreEqual(u3, u1);
            Assert.AreEqual(u3, u2);
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
Operations := (*, Times) | (/, Per)
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
