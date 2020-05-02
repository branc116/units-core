using System;
using System.Collections.Generic;
using System.Linq;

namespace Units.Core.Parser.State
{
    /// <summary>
    /// Unit that is Defined by 2 other units and the operator
    /// </summary>
    public class BinaryCompositUnit : ReadonlyUnit<BinaryCompositUnit>
    {
        public IUnit Unit1 { get; }
        public BinaryOperator Operator { get; }
        public IUnit Unit2 { get; }
        public BinaryCompositUnit(IUnit unit1, BinaryOperator @operator, IUnit unit2, string name, bool isInfered) : base(name, isInfered)
        {
            Unit1 = unit1;
            Operator = @operator;
            Unit2 = unit2;
        }
        public BinaryCompositUnit(IUnit unit1, BinaryOperator @operator, IUnit unit2, string name) : this(unit1, @operator, unit2, name, false) { }
        public BinaryCompositUnit(IUnit unit1, BinaryOperator @operator, IUnit unit2) : this(unit1, @operator, unit2, null) { }
        /// <inheritdoc/>
        public override string SiName()
        {
            return SiName(this);
        }
        public static string SiName(IUnit unit)
        {
            var a = GetNamedBaseUnitsCount(unit);
            if (!a.Any())
                return "Scalar";
            var up = a.Where(i => i.Value > 0).ToList();
            var down = a.Where(i => i.Value < 0).ToList();
            return (up.Any() ? up
                .OrderByDescending(i => i.Value)
                .ThenBy(i => i.Key.Item1.Name)
                .ThenBy(i => i.Key.Item2)
                .Select(i => i.Value > 1 ? $"{i.Key.Item2}{i.Key.Item1.Name}{i.Value}" : i.Key.Item2 + i.Key.Item1.Name)
                .Aggregate((i, j) => $"{i}{j}") : "Scalar") + (down.Any() ?
                    "Over" + down
                        .OrderByDescending(i => i.Value)
                        .ThenBy(i => i.Key.Item1.Name)
                        .ThenBy(i => i.Key.Item2)
                        .Select(i => i.Value < -1 ? $"{i.Key.Item2}{i.Key.Item1.Name}{i.Value * -1}" : i.Key.Item2 + i.Key.Item1.Name)
                        .Aggregate((i, j) => $"{i}{j}") :
                    string.Empty);
        }
        /// <summary>
        /// Simplify current composit unit and return the new unit that is defined only by the <see cref="State.Unit"/>
        /// </summary>
        /// <returns>Simplyfied unit <see cref="BinaryCompositUnit"/> or <see cref="Scalar"/> or <see cref="Unit"/></returns>
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
                            unit = new BinaryCompositUnit(unit, BinaryOperator.TIMES, u.Key, null, IsInfered).WithSiName();
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
                            over = new BinaryCompositUnit(over, BinaryOperator.TIMES, u.Key, null, IsInfered).WithSiName();
                        }
                    }
                }
                unit = unit != null ?
                    new BinaryCompositUnit(unit, BinaryOperator.OVER, over, null, IsInfered) :
                    new BinaryCompositUnit(Scalar.Get, BinaryOperator.OVER, over, null, IsInfered);
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
                if (cur is BinaryCompositUnit cu)
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
        public static Dictionary<(IUnit, string), double> GetNamedBaseUnitsCount(IUnit unit)
        {
            var dict = new Dictionary<(IUnit, string), double>();
            var s = new Stack<(IUnit, string, double)>();
            s.Push((unit, string.Empty, 1));
            while (s.Count != 0)
            {
                var (cur, name, count) = s.Pop();
                if (cur is BinaryCompositUnit cu)
                {
                    var newNameLeft = JoinName(name, cu.Operator.CountLeft.postfix);
                    var newNameRight = JoinName(name, cu.Operator.CountRight.postfix);
                    if (newNameLeft != name)
                    {
                        s.Push((cu.Unit1, newNameLeft + count.ToString("F"), 1));
                    }
                    else
                    {
                        s.Push((cu.Unit1, newNameLeft, count * cu.Operator.CountLeft.count));
                    }
                    if (newNameRight != name)
                    {
                        s.Push((cu.Unit2, newNameRight + count.ToString("F"), 1));
                    }
                    else
                    {
                        s.Push((cu.Unit2, newNameRight, count * cu.Operator.CountRight.count));
                    }
                }
                else if (cur is UnaryCompositUnit ucu)
                {
                    var newNameRight = JoinName(name, ucu.UnaryOperator.Count.postfix);
                    if (newNameRight != name)
                    {
                        s.Push((ucu.Unit, newNameRight + count.ToString("F"), 1));
                    }
                    else
                    {
                        s.Push((ucu.Unit, newNameRight, count * ucu.UnaryOperator.Count.count));
                    }
                }
                else if (cur is Unit u)
                {
                    if (!dict.ContainsKey((u, name)))
                        dict.Add((u, name), 0);
                    dict[(u, name)] += count;
                }
                else if (cur is Scalar || cur is null)
                {
                    continue;
                }
                else
                {
                    throw new HandleException($"Can't use Unit of type {cur.GetType()}", 0904);
                }
            }
            return dict.Where(i => i.Value != 0).ToDictionary(i => i.Key, i => i.Value);
        }
        public static string JoinName(string name, char? added)
        {
            return added is char c ? name + c : name;
        }
        /// <inheritdoc/>
        public override IUnit WithSiName()
        {
            return Rename(SiName(true));
        }
        public override string ToString()
        {
            return $"{Name} ({Unit1.Name} {Operator.Name} {Unit2.Name})";
        }

        public override IUnit Rename(string newName)
        {
            return new BinaryCompositUnit(Unit1, Operator, Unit2, newName, IsInfered);
        }
    }
}
