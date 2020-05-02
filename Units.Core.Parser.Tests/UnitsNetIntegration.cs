using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Units.Core.Parser.Tests
{
    [TestClass]
    public class UnitsNetIntegration
    {
        [TestMethod]
        public void BasicIntegration()
        {
            var text = @"!UnitsNet Dimension, Length | Dimension, Mass";
            Parser.ParseGrammarString(text);
        }
        [TestMethod]
        public void BasicIntegration_All()
        {
            var text = @"Operators(Binary) := (*, Times, 1, 1) | (/, Per, 1, -1)
Operator(*) := a = b * c | a = c * b | c = a / b | b = a / c
Operator(/) := a = b / c | c = b / a | b = a * c | b = c * a
!UnitsNet All
";
            var state = Parser.ParseGrammarString(text);
            var health = state.Units.GetHashSetHealth();
        }
        [TestMethod]
        public void BasicIntegration_AllInfer()
        {
            var text = @"Operators(Binary) := (*, Times, 1, 1) | (/, Per, 1, -1)
Operator(*) := a = b * c | a = c * b | c = a / b | b = a / c
Operator(/) := a = b / c | c = b / a | b = a * c | b = c * a
!UnitsNet All
!Infer Fast
";
            var state = Parser.ParseGrammarString(text);
            var health = state.Units.GetHashSetHealth();
        }
    }
}
