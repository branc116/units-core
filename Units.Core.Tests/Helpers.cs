using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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
    public static class ReallyDontWorryAboutIt {
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
            //var lines = source.Split(System.Environment.NewLine);
            var state = Parser.Parser.ParseGrammarString(source);
            var generator = new Units.Core.GenerateUnits(state);
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
}
