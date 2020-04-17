using System;
using System.Collections.Generic;
using System.Linq;

namespace Units.Core.Parser.State
{
    /// <summary>
    /// Unit that is Defined by 2 other units and the operator
    /// </summary>
    public class CompositUnit : ReadonlyUnit<CompositUnit>
    {
        public IUnit Unit1 { get; }
        public Operator Operator { get; }
        public IUnit Unit2 { get; }
        public CompositUnit(IUnit unit1, Operator @operator, IUnit unit2, string name) : base(name)
        {
            Unit1 = unit1;
            Operator = @operator;
            Unit2 = unit2;
        }
        public CompositUnit(IUnit unit1, Operator @operator, IUnit unit2) : this(unit1, @operator, unit2, null) { }
        /// <inheritdoc/>
        public override string SiName()
        {
            var a = GetBaseUnitCount();
            if (!a.Any())
                return "Scalar";
            var up = a.Where(i => i.Value > 0).ToList();
            var down = a.Where(i => i.Value < 0).ToList();
            return (up.Any() ? up
                .OrderByDescending(i => i.Value)
                .ThenBy(i => i.Key.Name)
                .Select(i => i.Value > 1 ? $"{i.Key.Name}{i.Value}" : i.Key.Name)
                .Aggregate((i, j) => $"{i}{j}") : "Scalar") + (down.Any() ?
                    "Over" + down
                        .OrderByDescending(i => i.Value)
                        .ThenBy(i => i.Key.Name)
                        .Select(i => i.Value < -1 ? $"{i.Key.Name}{i.Value * -1}" : i.Key.Name)
                        .Aggregate((i, j) => $"{i}{j}") :
                    string.Empty);
        }
        /// <summary>
        /// Simplify current composit unit and return the new unit that is defined only by the <see cref="State.Unit"/>
        /// </summary>
        /// <returns>Simplyfied unit <see cref="CompositUnit"/> or <see cref="Scalar"/> or <see cref="Unit"/></returns>
        /// <remarks>
        /// Usefull for e.g.
        /// 
        /// Composit unit is defined as:
        /// 
        /// <see cref="Unit1"/> is t²,
        /// 
        /// <see cref="Operator"/> is /,
        /// 
        /// and <see cref="Unit2"/> t.
        /// 
        /// This is same as <see cref="Unit"/> whit <see cref="Unit.Name"/> == "t"
        /// 
        /// ----TODO----
        /// 
        /// Implementation has hardcoded values. It needs to be more general.
        /// 
        /// This can become more general, only when the goal on <see cref="Handlers.HandleOperators"/> is met.
        /// 
        /// ----TODO----
        /// 
        /// </remarks>
        public override IUnit Simplify()
        {
            var dict = GetBaseUnitCount();
            var up = dict.Where(i => i.Value > 0).ToList();
            IUnit unit = null;
            if (up.Any())
            {
                foreach (var u in up)
                {
                    for (int i = 0; i < u.Value; i++)
                    {
                        if (unit is null)
                            unit = u.Key;
                        else
                        {
                            unit = new CompositUnit(unit, Operator.TIMES, u.Key, null).WithSiName();
                        }

                    }
                }
            }
            var down = dict.Where(i => i.Value < 0).ToList();
            if (down.Any())
            {
                IUnit over = null;
                foreach (var u in down)
                {
                    for (int i = 0; i > u.Value; i--)
                    {
                        if (over is null)
                            over = u.Key;
                        else
                        {
                            over = new CompositUnit(over, Operator.TIMES, u.Key, null).WithSiName();
                        }
                    }
                }
                unit = unit != null ?
                    new CompositUnit(unit, Operator.OVER, over, null) :
                    new CompositUnit(Scalar.Get, Operator.OVER, over, null);
            }
            unit ??= Scalar.Get;
            return unit.WithSiName();
        }
        /// <summary>
        /// Count the dimensionality for each base unit.
        /// </summary>
        /// <returns>Dimension for each base unit</returns>
        /// <remarks>
        /// Needs to be more general. Same as <see cref="Simplify"/>.
        /// </remarks>
        public Dictionary<IUnit, int> GetBaseUnitCount()
        {
            var dict = new Dictionary<IUnit, int>();
            var s = new Stack<(IUnit, double)>();
            s.Push((this, 1));
            while (s.Count != 0)
            {
                var (cur, count) = s.Pop();
                if (cur is CompositUnit cu)
                {
                    s.Push((cu.Unit1, cu.Operator.CountLeft.count * count));
                    s.Push((cu.Unit2, cu.Operator.CountRight.count * count));
                }
                else if (cur is Scalar)
                {
                    continue;
                }
                else if (cur is Unit u)
                {
                    if (!dict.ContainsKey(u))
                    {
                        dict.Add(u, 0);
                    }
                    dict[u] += (int)Math.Round(count);
                }
            }
            return dict.Where(i => i.Value != 0).ToDictionary(i => i.Key, i => i.Value);
        }
        /// <inheritdoc/>
        public override IUnit WithSiName()
        {
            return new CompositUnit(Unit1, Operator, Unit2, SiName(true));
        }
        public override string ToString()
        {
            return $"{Name} ({Unit1.Name} {Operator.Name} {Unit2.Name})";
        }
    }
}
