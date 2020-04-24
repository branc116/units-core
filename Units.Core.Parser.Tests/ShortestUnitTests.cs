using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Units.Core.Parser.State;

namespace Units.Core.Parser.Tests
{
    [TestClass]
    public class ShortestUnitTests
    {
        [TestMethod]
        public void Easy()
        {
            var l = new Unit("length");
            var t = new Unit("time");
            var speed = new BinaryCompositUnit(l, BinaryOperator.OVER, t);
            var state = new ParserState
            {
                Operators = new HashSet<IOperator>
                {
                    BinaryOperator.TIMES,
                    BinaryOperator.OVER
                },
                Units = new HashSet<IUnit>
                {
                    l, t, speed
                }
            };
            var (unit, depth) = Helpers.ShortestUnit(speed, state);
            Assert.AreEqual(speed.SiName(), unit.SiName());
            Assert.AreEqual(1, depth);
        }
        [TestMethod]
        public void Medium()
        {
            var l = new Unit("length");
            var t = new Unit("time");
            var speed = new BinaryCompositUnit(l, BinaryOperator.OVER, t);
            var acceleration = new BinaryCompositUnit(speed, BinaryOperator.OVER, t);
            var state = new ParserState
            {
                Operators = new HashSet<IOperator>
                {
                    BinaryOperator.TIMES,
                    BinaryOperator.OVER
                },
                Units = new HashSet<IUnit>
                {
                    l, t, speed
                }
            };
            var (unit, depth) = Helpers.ShortestUnit(acceleration, state);
            Assert.AreEqual(acceleration.SiName(), unit.SiName());
            Assert.AreEqual(2, depth);
        }
        [TestMethod]
        public void Hard()
        {
            var l = new Unit("length");
            var t = new Unit("time");
            var area = new BinaryCompositUnit(l, BinaryOperator.TIMES, l);
            var volumen = new BinaryCompositUnit(area, BinaryOperator.TIMES, l);
            var hv = new BinaryCompositUnit(volumen, BinaryOperator.TIMES, l);
            var speed = new BinaryCompositUnit(l, BinaryOperator.OVER, t);
            var acceleration = new BinaryCompositUnit(speed, BinaryOperator.OVER, t);
            var state = new ParserState
            {
                Operators = new HashSet<IOperator>
                {
                    BinaryOperator.TIMES,
                    BinaryOperator.OVER,
                    new UnaryOperator("SQ", "SQ") {Count = (2, null)}
                },
                Units = new HashSet<IUnit>
                {
                    l, t, speed, hv, volumen, area, acceleration
                }
            };
            var (unit, depth) = Helpers.ShortestUnit(hv, state);
            Assert.AreEqual(hv.SiName(), unit.SiName());
            Assert.AreEqual(2, depth);
        }
    }
}
