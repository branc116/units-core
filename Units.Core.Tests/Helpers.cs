using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Units.Core.Parser;

namespace Units.Core.Tests
{
    public sealed class CompileException : System.Exception
    {
        public int LineNum { get; set; }
        public CompileException(string message, int lineNum) : base(message)
        {
            LineNum = lineNum;
        }

        public CompileException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
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
            var arr = injectMain ? new[] { SyntaxFactory.ParseSyntaxTree(source + @"
namespace DontWorryAboutIt {
    public static class ReallyDontWorryAboutIt {
        public static void Main(string[] args) { return; }
    }
}") } : new[] { SyntaxFactory.ParseSyntaxTree(source) };
            var comp = CSharpCompilation.Create(Guid.NewGuid().ToString(), arr, refs);
            using var ms = new MemoryStream();
            var er = comp.Emit(ms);
            if (!er.Success)
                throw new CompileException(er.Diagnostics.Select(i => i.ToString())
                    .Aggregate((i, j) => $"{i}{System.Environment.NewLine}{j}"), 
                    er.Diagnostics
                        .Select(i => i.Location.SourceSpan.Start).FirstOrDefault());
            var val = System.Reflection.Assembly.Load(ms.ToArray());
            return val;
        }
        public static Assembly UnitsToAssembly(this string source)
        {
            //var lines = source.Split(System.Environment.NewLine);
            var state = Parser.Parser.ParseGrammarString(source);
            var generator = new Units.Core.Generators.GenerateUnits(state);
            var csharpSource = generator.TransformText();
            try
            {
                var assembly = csharpSource.ToAssembly();
                return assembly;
            }catch(CompileException ex)
            {
                throw new Exception(csharpSource.AddLineNumbers(ex.LineNum > 100 ? ex.LineNum - 100 : 0, 200), ex);
            }
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
        public static string AddLineNumbers(this string str, int skip, int take)
        {
            var text = str.Split(Environment.NewLine)
                .Take(take + 1)
                .ToIndexed()
                .Select(i => $"{i.index + skip}> {i.value}")
                .Aggregate((i, j) => $"{i}{Environment.NewLine}{j}");
            return text;
        }
    }
}
