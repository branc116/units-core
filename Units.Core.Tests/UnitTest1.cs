using Microsoft.CSharp.RuntimeBinder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Units.Core.Tests
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public void Testing_Test()
        {
            var assembly = @"
namespace test {
    public class t {
        public static void Main(string[] args) {
            System.Console.WriteLine(""Hello world"");
        }
        public static int Hey => 42;
    }
}".ToAssembly(false);
            var val = assembly.GetType("test.t").GetProperty("Hey").GetValue(null);
            Assert.AreEqual(42, val);
        }
        [TestMethod]
        public void TestBase()
        {
            var source = @"
Units(Base) := Mass | Length
Real(Types) := (float, RealFloat)";
            var assemby = source.UnitsToAssembly();
            var mass = assemby.GetUnit("Mass");
            Assert.IsNotNull(mass);
            var length = assemby.GetUnit("Length");
            Assert.IsNotNull(length);
        }
        [TestMethod]
        public void TestOperators()
        {

            var source = @"
Real(Types) := (float, RealFloat)
Operators(Self) := (*, Times, null)";
            var assemby = source.UnitsToAssembly();
            var real = assemby.GetUnit("RealFloat");
            var num = 3.1f;
            dynamic instance = num.DinamicCtor(real);
            Assert.AreEqual(num * num, (float)(instance * instance));
            Assert.AreEqual(num * num, (float)(instance.OpTimes(instance)));
            Assert.ThrowsException<RuntimeBinderException>(() =>
            {
                var a = instance / instance;
            });
        }
        [TestMethod]
        public void TestInfer_OnlyTimes()
        {
            var source = @"
Units(Base) := Mass | Length
Real(Types) := (float, RealFloat)
Operators(Self) := (*, Times, null)
Operators(Binary) := (*, Times, 1, 1)
!Infer Slow
".TrimStart();
            var assemby = source.UnitsToAssembly();
            foreach (var unit in new[] { "Mass", "Length" })
            {
                var un = assemby.GetUnit(unit);
                var num = 3.1f;
                dynamic instance = num.DinamicCast(un);
                Assert.AreEqual(num * num, (float)(instance * instance));
                Assert.IsInstanceOfType(instance * instance, assemby.GetUnit($"{unit}2"));
                Assert.ThrowsException<RuntimeBinderException>(() =>
                {
                    var a = instance / instance;
                });
            }
        }
        [TestMethod]
        public void TestInfer_OnlyTimesAndPer()
        {
            var num = 3.1f;
            var source = @"
Units(Base) := Mass | Length
Real(Types) := (float, RealFloat)
Operators(Self) := (*, Times, null) | (/, Per, null)
Operators(Binary) := (*, Times, 1, 1) | (/, Per, 1, -1)
!Infer Slow";
            var assemby = source.UnitsToAssembly();
            foreach (var unit in new[] { "Mass", "Length" })
            {
                var un = assemby.GetUnit(unit);
                dynamic instance = num.DinamicCast(un);
                Assert.AreEqual(num * num, (float)(instance * instance));
                Assert.IsInstanceOfType(instance * instance, assemby.GetUnit($"{unit}2"));
                Assert.AreEqual(num / num, (float)(instance / instance));
                Assert.IsInstanceOfType(instance / instance, assemby.GetUnit($"RealFloat"));
            }
        }
        [TestMethod]
        public void TestInitDefinitino()
        {
            var src = Units.Core.CommandLineOptions.Init.FullSiStandard;
            var assebly = src.UnitsToAssembly();
        }
        [TestMethod]
        public void TestUnitsNetAll()
        {
            var src = @"
Operators(Binary) := (*, Times, 1, 1) | (/, Per, 1, -1)
Operators(Self) := (*, Times, null) | (/, Per, null)
Operators(Self) := (<, Lt, bool) | (<=, Let, bool) | (>, Gt, bool) | (>=, Get, bool) | (==, Eq, bool) | (!=, Ne, bool) | (+, Plus, null) | (-, Minus, null)
Real(Types) := (float, RealFloat)

Operator(*) := a = b * c | a = c * b | c = a / b | b = a / c
Operator(/) := a = b / c | c = b / a | b = a * c | b = c * a

!UnitsNet All

!Infer Fast

!Export All, ./MyUnits".TrimStart();
            var assembly = src.UnitsToAssembly();
        }
    }
}
