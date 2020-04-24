using System;
using System.Collections.Generic;
using Units.Core.Parser.State;

namespace Units.Core.Parser
{
    public class UnitComparer : IComparer<(IUnit, int)>
    {
        public IUnit Wanted { get; }
        public string WantedString { get; }
        public UnitComparer(IUnit wanted)
        {
            Wanted = wanted;
            WantedString = wanted.SiName();
        }
        public int Dist((IUnit unit, int depth) a)
        {
            var name1 = a.unit is IReadonlyUnit rou ? rou.SiName(true) : a.unit.SiName();
            var to = Math.Min(WantedString.Length, name1.Length);
            var count1 = 0;
            for (int i = 1; i <= to; i++)
            {
                count1 += WantedString[^i] == name1[^i] ? 0 : 1;
            }
            var distance = count1 << a.depth;
            return distance;
        }
        public int Compare((IUnit, int) x, (IUnit, int) y)
        {
            var name1 = x.Item1 is IReadonlyUnit rou ? rou.SiName(true) : x.Item1.SiName();
            var name2 = y.Item1 is IReadonlyUnit rou2 ? rou2.SiName(true) : y.Item1.SiName();
            //if (name1 == name2)
            //    return 0;
            //if (y.Item2 != x.Item2)
            //    return x.Item2 < y.Item2 ? -1 : 1;

            var to = Math.Min(name1.Length, Math.Min(WantedString.Length, name2.Length));
            var count1 = 0;
            var count2 = 0;
            for (int i = 1; i <= to; i++)
            {
                count1 += WantedString[^i] == name1[^i] ? 1 : 0;
                count2 += WantedString[^i] == name2[^i] ? 1 : 0;
            }
            //if (count1 == count2 && name1 == name2 && x.Item2)

            return count1 < count2 ? -1 : 1;
        }
    }
}
