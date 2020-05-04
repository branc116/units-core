using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Units.Core.Parser.Tests
{
    [TestClass]
    public class RegexTests
    {
        [TestMethod]
        public void AddExplicitConvertToNumbers_Int()
        {
            var expr = "1";
            var expr2 = expr.AddExplicitConvertToNumbers("int");
            Assert.AreEqual("((int)(1))", expr2);
        }
        [TestMethod]
        public void AddExplicitConvertToNumbers_IntPlusInt()
        {
            var expr = "1 + 1";
            var expr2 = expr.AddExplicitConvertToNumbers("int");
            Assert.AreEqual("((int)(1)) + ((int)(1))", expr2);
        }
        [TestMethod]
        public void AddExplicitConvertToNumbers_Floatv1()
        {
            var expr = "1.1";
            var expr2 = expr.AddExplicitConvertToNumbers("float");
            Assert.AreEqual("((float)(1.1))", expr2);
        }
        [TestMethod]
        public void AddExplicitConvertToNumbers_Floatv2()
        {
            var expr = "-1.1";
            var expr2 = expr.AddExplicitConvertToNumbers("float");
            Assert.AreEqual("((float)(-1.1))", expr2);
        }
        [TestMethod]
        public void AddExplicitConvertToNumbers_Floatv3()
        {
            var expr = "+1.1";
            var expr2 = expr.AddExplicitConvertToNumbers("float");
            Assert.AreEqual("((float)(+1.1))", expr2);
        }
        [TestMethod]
        public void AddExplicitConvertToNumbers_Floatv4()
        {
            var expr = "1.1e+10";
            var expr2 = expr.AddExplicitConvertToNumbers("float");
            Assert.AreEqual("((float)(1.1e+10))", expr2);
        }
        [TestMethod]
        public void AddExplicitConvertToNumbers_Floatv5()
        {
            var expr = "1.1e-10";
            var expr2 = expr.AddExplicitConvertToNumbers("float");
            Assert.AreEqual("((float)(1.1e-10))", expr2);
        }
        [TestMethod]
        public void AddExplicitConvertToNumbers_Floatv6()
        {
            var expr = "1e-10";
            var expr2 = expr.AddExplicitConvertToNumbers("float");
            Assert.AreEqual("((float)(1e-10))", expr2);
        }
    }
}
