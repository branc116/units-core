using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Units.Core.Tests
{
    public static class Helpers
    {
        public static IEnumerable<PortableExecutableReference> GetAllReferences()
        {
            return typeof(UnitTest1).Assembly.GetReferencedAssemblies()
                .Select(i => System.Reflection.Assembly.Load(i).Location)
                .Prepend(typeof(object).Assembly.Location)
                .Prepend(typeof(System.Console).Assembly.Location)
                .Select(i => MetadataReference.CreateFromFile(i));
        }
        public static Assembly ToAssembly(this string source, bool injectMain = true)
        {
            var refs = GetAllReferences().ToList();
            var arr = injectMain ? new[] { SyntaxFactory.ParseSyntaxTree(source), SyntaxFactory.ParseSyntaxTree(@"
namespace DontWorryAboutIt {
    public static class RealyDontWorryAboutIt {
        public static void Main(string[] args) { return; }
    }
}") } : new[] { SyntaxFactory.ParseSyntaxTree(source) };
            var comp = CSharpCompilation.Create(Guid.NewGuid().ToString(), arr, refs);
            using var ms = new MemoryStream();
            var er = comp.Emit(ms);
            if (!er.Success)
                throw new Exception(er.Diagnostics.Select(i => i.ToString()).Aggregate((i, j) => $"{i}{System.Environment.NewLine}"));
            var val = System.Reflection.Assembly.Load(ms.ToArray());
            return val;
        }
        public static Assembly UnitsToAssembly(this string source)
        {
            var lines = source.Split(System.Environment.NewLine);
            var generator = new Units.Core.GenerateUnits(lines);
            var csharpSource = generator.TransformText();
            var assembly = csharpSource.ToAssembly();
            return assembly;
        }
        public static dynamic DinamicCast(this object from, Type to)
        {
            var a = from.GetType();
            var meth = to.GetMethod("op_Explicit", new Type[] { a });
            Assert.IsNotNull(meth);
            dynamic din = meth.Invoke(null, new object[] { from });
            return din;
        }
        public static dynamic DinamicCtor(this object from, Type to)
        {
            var a = from.GetType();
            var meth = to.GetConstructor(new Type[] { a });
            Assert.IsNotNull(meth);
            dynamic din = meth.Invoke(new object[] { from });
            return din;
        }
        public static Type GetUnit(this Assembly from, string name)
        {
            return from.GetTypes().FirstOrDefault(i => i.Name == name);
        }
    }
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
Base(Unit) := Mass | Length
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
SelfOps := (*, Times, null)";
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
Base(Unit) := Mass | Length
Real(Types) := (float, RealFloat)
SelfOps := (*, Times, null)
Operators(Binary) := (*, Times, 1, 1)
Infer";
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
Base(Unit) := Mass | Length
Real(Types) := (float, RealFloat)
SelfOps := (*, Times, null) | (/, Per, null)
Operators(Binary) := (*, Times, 1, 1) | (/, Per, 1, -1)
Infer";
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
    }
}
