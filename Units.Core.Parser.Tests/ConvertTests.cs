using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Units.Core.Parser.Tests
{
    [TestClass]
    public class ConvertTests
    {
        [TestMethod]
        public void ConvertsTest_Mass()
        {
            var txt = @"!UnitsNet Dimension, Mass";
            var state = Parser.ParseGrammarString(txt);
            var mass = state.Units.FirstOrDefault(i => i.Name == "Mass");
            Assert.IsNotNull(mass);
            var description = state.Descriptions[mass].FirstOrDefault();
            Assert.IsNotNull(description);
            foreach(var mu in description.Units)
            {
                var muCore = state.MesurmentUnits.FirstOrDefault(i => i.For.Equals(mass) && i.Name == mu.SingularName);
                Assert.IsNotNull(muCore);
                CustomAssert.ExpressionsAreEqual(mu.FromBaseToUnitFunc, muCore.ConvertTo);
                CustomAssert.ExpressionsAreEqual(mu.FromUnitToBaseFunc, muCore.ConvertFrom);
            }
        }
        [TestMethod]
        public void ConvertsTest_Length()
        {
            var txt = @"!UnitsNet Dimension, Length";
            var state = Parser.ParseGrammarString(txt);
            var length = state.Units.FirstOrDefault(i => i.Name == "Length");
            Assert.IsNotNull(length);
            var description = state.Descriptions[length].FirstOrDefault();
            Assert.IsNotNull(description);
            foreach(var mu in description.Units)
            {
                var muCore = state.MesurmentUnits.FirstOrDefault(i => i.For.Equals(length) && i.Name == mu.SingularName);
                Assert.IsNotNull(muCore);
                CustomAssert.ExpressionsAreEqual(mu.FromBaseToUnitFunc, muCore.ConvertTo);
                CustomAssert.ExpressionsAreEqual(mu.FromUnitToBaseFunc, muCore.ConvertFrom);
            }
        }
        [TestMethod]
        public void ConvertsTest_All()
        {
            var txt = @"!UnitsNet All";
            var state = Parser.ParseGrammarString(txt);
            foreach (var coreUnit in state.Units)
            {
                Assert.IsNotNull(coreUnit);
                var description = state.Descriptions[coreUnit].FirstOrDefault();
                Assert.IsNotNull(description);
                foreach (var mu in description.Units)
                {
                    var muCore = state.MesurmentUnits.FirstOrDefault(i => i.For.Equals(coreUnit) && i.Name == mu.SingularName);
                    Assert.IsNotNull(muCore);
                    CustomAssert.ExpressionsAreEqual(mu.FromBaseToUnitFunc, muCore.ConvertTo);
                    CustomAssert.ExpressionsAreEqual(mu.FromUnitToBaseFunc, muCore.ConvertFrom);
                }
            }
        }
    }
}
