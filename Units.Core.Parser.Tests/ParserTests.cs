using Microsoft.VisualStudio.TestTools.UnitTesting;
using Units.Core.Parser.State;
namespace Units.Core.Parser.Tests
{
    [TestClass]
    public class ParserTests
    {
        private static BinaryOperator _times = new BinaryOperator("Times", "*")
        {
            CountLeft = (1, null),
            CountRight = (1, null)
        };
        private static BinaryOperator _over = new BinaryOperator("Over", "/")
        {
            CountLeft = (1, null),
            CountRight = (-1, null)
        };

        [TestMethod]
        public void ATimesB_Equals_BTimesA()
        {
            var u1 = new Unit("Lenght");
            var u2 = new Unit("Mass");
            var u3 = new BinaryCompositUnit(u1, _times, u2, null);
            var u4 = new BinaryCompositUnit(u2, _times, u1, null);
            Assert.AreEqual(u3, u4);
        }
        [TestMethod]
        public void APerA_Equals_Scalar()
        {
            var u1 = new Unit("Lenght");
            var u3 = new BinaryCompositUnit(u1, _over, u1);
            Assert.AreEqual(u3.SiName(), Scalar.Get.SiName());
            Assert.AreEqual(u3, Scalar.Get);
        }
        [TestMethod]
        public void ScalarTimesScalar_Eqals_Scalar()
        {
            var u1 = Scalar.Get;
            var u2 = Scalar.Get;
            var u3 = new BinaryCompositUnit(u1, _times, u2);
            Assert.AreEqual(u3, u1);
            Assert.AreEqual(u3, u2);
            Assert.AreEqual(u2, u3);
        }
    }
}
