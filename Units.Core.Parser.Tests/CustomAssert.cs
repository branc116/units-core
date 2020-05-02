using System;
using MathNet.Symbolics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Units.Core.Parser.Tests
{
    public static class CustomAssert
    {
        public static bool ExpressionsAreEqual(string expected, string actual)
        {
            var x = SymbolicExpression.Variable("x");
            SymbolicExpression e = null;
            try
            {
                e = SymbolicExpression.Parse(expected.CleanExpression());
            }catch (Exception ex)
            {

            }
            var a = SymbolicExpression.Parse(actual);
            for (int i = 1; i >= 0; i <<= 1)
            {
                var resExpected = e.Substitute(x, SymbolicExpression.FromInt64(i)).Approximate().ToString();
                var resActual = a.Substitute(x, SymbolicExpression.FromInt64(i)).Approximate().ToString();
                try
                {
                    var resDoubleExpected = double.Parse(resExpected, System.Globalization.CultureInfo.GetCultureInfo("en-US").NumberFormat);
                    var resDoubleActual = double.Parse(resActual, System.Globalization.CultureInfo.GetCultureInfo("en-US").NumberFormat);
                    Assert.AreEqual(resDoubleExpected, resDoubleActual, 10 * Math.Pow(10, Math.Log10(Math.Abs(resDoubleExpected)) - 9));
                }catch (Exception ex)
                {
                    throw;
                }
            }
            return true;
        }
    }
}
