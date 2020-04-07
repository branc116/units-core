using System;
using System.Collections.Generic;
using System.Linq;

namespace Units.Core.Parser.State
{
    /// <summary>
    /// Unit that is Defined by 2 other units and the operator
    /// </summary>
    public class CompositUnit : Unit, IEquatable<CompositUnit>
    {
        public Unit Unit1 { get; set; } = new Scalar();
        public Operator Operator { get; set; }
        public Unit Unit2 { get; set; }
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
        public Unit Simplify()
        {
            var dict = GetBaseUnitCount();
            var up = dict.Where(i => i.Value > 0).ToList();
            Unit unit = null;
            if (up.Any())
            {
                foreach (var u in up)
                {
                    for (int i = 0; i < u.Value; i++)
                    {
                        if (unit is null)
                            unit = new Unit { Name = u.Key.Name };
                        else
                        {
                            unit = new CompositUnit
                            {
                                Name = u.Key.Name,
                                Operator = new Operator { Name = "Times", Symbol = "*" },
                                Unit1 = unit.Clone(),
                                Unit2 = new Unit { Name = u.Key.Name }
                            };
                            unit.Name = unit.SiName();
                        }

                    }
                }
            }
            var down = dict.Where(i => i.Value < 0).ToList();
            if (down.Any())
            {
                Unit over = null;
                foreach (var u in down)
                {
                    for (int i = 0; i > u.Value; i--)
                    {
                        if (over is null)
                            over = new Unit { Name = u.Key.Name };
                        else
                        {
                            over = new CompositUnit
                            {
                                Name = u.Key.Name,
                                Operator = new Operator
                                {
                                    Name = "Times",
                                    Symbol = "*"
                                },
                                Unit1 = over.Clone(),
                                Unit2 = new Unit { Name = u.Key.Name }
                            };
                            over.Name = over.SiName();
                        }
                    }
                }
                unit = unit != null ?
                    new CompositUnit
                    {
                        Name = "over",
                        Operator = new Operator
                        {
                            Name = "Per",
                            Symbol = "/"
                        },
                        Unit1 = unit,
                        Unit2 = over.Clone()
                    } :
                    new CompositUnit
                    {
                        Name = "over",
                        Operator = new Operator { Name = "Per", Symbol = "/" },
                        Unit1 = new Scalar(),
                        Unit2 = over.Clone()
                    };
            }
            unit = unit ?? new Scalar();
            unit.Name = unit.SiName();
            return unit;
        }
        /// <summary>
        /// Count the dimensionality for each base unit.
        /// </summary>
        /// <returns>Dimension for each base unit</returns>
        /// <remarks>
        /// Needs to be more general. Same as <see cref="Simplify"/>.
        /// </remarks>
        public Dictionary<Unit, int> GetBaseUnitCount()
        {
            var dict = new Dictionary<Unit, int>();
            var s = new Stack<(Unit, bool)>();
            s.Push((this, false));
            while (s.Count != 0)
            {
                var (cur, down) = s.Pop();
                if (cur is CompositUnit cu)
                {
                    s.Push((cu.Unit1, down));
                    s.Push((cu.Unit2, cu.Operator.Symbol == "/" ? !down : down));
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
                    dict[u] += down ? -1 : 1;
                }
            }
            return dict.Where(i => i.Value != 0).ToDictionary(i => i.Key, i => i.Value);
        }
        public override bool Equals(object obj)
        {
            var a = this.Simplify();
            if (a is CompositUnit)
                return Equals(obj as CompositUnit);
            else
                return a.Equals(obj);
        }

        public bool Equals(CompositUnit other)
        {
            return other != null && this.SiName() == other.SiName();
        }

        public override int GetHashCode()
        {
            return this.SiName().GetHashCode();
        }
        /// <inheritdoc/>
        public override Unit Clone()
        {
            return new CompositUnit
            {
                Name = $"{Name}",
                Operator = new Operator { Name = Operator.Name, Symbol = Operator.Symbol },
                Unit1 = Unit1,
                Unit2 = Unit2
            };
        }
        public override string ToString()
        {
            return $"{Name} ({Unit1.Name} {Operator.Name} {Unit2.Name})";
        }
    }
}
