using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using MathNet.Numerics.Statistics;
using Units.Core.Parser.Metadata;

namespace Units.Core.Parser.State
{
    public class ParserState
    {
        public HashSet<IUnit> Units { get; set; } = new HashSet<IUnit>();
        public HashSet<IOperator> Operators { get; set; } = new HashSet<IOperator>();
        public HashSet<MesurmentUnit> MesurmentUnits { get; } = new HashSet<MesurmentUnit>();
        public HashSet<SelfOp> SelfOps { get; } = new HashSet<SelfOp>();
        public HashSet<RealDef> RealDefs { get; } = new HashSet<RealDef>();
        public Dictionary<IUnit, HashSet<OperatorNodeEdge>> GraphEdges { get; } = new Dictionary<IUnit, HashSet<OperatorNodeEdge>>();
        public Dictionary<IUnit, List<Metadata.UnitsNetDescription>> Descriptions { get; } = new Dictionary<IUnit, List<UnitsNetDescription>>();
        public IExportHandle Exporter { get; set; }

        public void AddNewCompositUnit(string leftUnit, string symbol, string rightUnit, string newUnitName)
        {
            var u1 = GetUnit(leftUnit);
            if (u1 is null)
                throw new HandleException($"Can't find unit with name {leftUnit}", 1124);
            var u2 = GetUnit(rightUnit);
            if (u2 is null)
                throw new HandleException($"Can't find unit with name {rightUnit}", 1124);
            var op = GetOperator(symbol);
            if (op is null)
                throw new HandleException($"Can't find operator {symbol}", 1125);
            if (op is BinaryOperator opb)
            {
                var unit = new BinaryCompositUnit(u1, opb, u2, newUnitName);
                Units.Add(unit);
                AddEdges(op, unit, u1, u2);
            }
            else
            {
                throw new HandleException($"Can't use unary operator on 2 parameters", 1900);
            }
        }
        public void AddNewCompositUnit(string @operator, string rightUnit, string newUnitName)
        {
            var right = GetUnit(rightUnit);
            var op = GetOperator(@operator);
            if (op is BinaryOperator)
                throw new HandleException($"Can't use binary operator on 1 parameter", 1121);
            var uop = op as UnaryOperator;
            if (uop is null)
                throw new HandleException($"Can't find unary operator", 1122);
            var unit = new UnaryCompositUnit(right, uop, newUnitName);
            Units.Add(unit);
            AddEdges(op, unit, right);
        }
        private readonly object _lock = new object();
        internal void AddUnitsNetDescription(UnitsNetDescription description, string unitsCoreName = null)
        {
            lock (_lock)
            {
                var unitsCoreUnit = GetUnit(unitsCoreName ?? description.Name);
                if (unitsCoreUnit is null)
                {
                    IUnit uppers = null, downers = null;
                    foreach (var bd in description.BaseDimensions?.Where(i => i.Value < 0) ?? Enumerable.Empty<KeyValuePair<string, int>>())
                    {
                        var a = Metadata.Basedimensions.Bases[bd.Key];
                        var n = bd.Value * -1;
                        while (--n >= 0)
                        {
                            if (downers is null)
                                downers = a;
                            else
                                downers = new BinaryCompositUnit(downers, BinaryOperator.TIMES, a);
                        }
                    }
                    foreach (var bd in description.BaseDimensions?.Where(i => i.Value > 0) ?? Enumerable.Empty<KeyValuePair<string, int>>())
                    {
                        var a = Metadata.Basedimensions.Bases[bd.Key];
                        var n = bd.Value;
                        while (--n >= 0)
                        {
                            if (uppers is null)
                                uppers = a;
                            else
                                uppers = new BinaryCompositUnit(uppers, BinaryOperator.TIMES, a);
                        }
                    }
                    unitsCoreUnit = downers is null ?
                        uppers?.Rename(unitsCoreName ?? description.Name) ?? Scalar.Get :
                        new BinaryCompositUnit(uppers ?? Scalar.Get, BinaryOperator.OVER, downers, unitsCoreName ?? description.Name, false);
                    if (!Units.Add(unitsCoreUnit))
                    {
                        var old = Units.First(i => i.Equals(unitsCoreUnit));
                        if (old.Name.Length > unitsCoreUnit.Name.Length)
                        {
                            Units.Remove(old);
                            Units.Add(unitsCoreUnit);
                        }
                    }
                }
                if (Descriptions.ContainsKey(unitsCoreUnit))
                    Descriptions[unitsCoreUnit].Add(description);
                else
                    Descriptions.Add(unitsCoreUnit, new List<UnitsNetDescription> { description });
                var adds = description.Units
                    .ToMesurmentUnits(unitsCoreUnit)
                    .Select(this.MesurmentUnits.Add)
                    .ToList();
            }
        }

        public void AddNewCompositUnit(IUnit left, IOperator @operator, IUnit right, bool isInfered = false, bool fast = false)
        {
            if (@operator is BinaryOperator opb)
            {
                var tmp = new BinaryCompositUnit(left, opb, right, null, isInfered);
                var (unit, depth) = fast ? Helpers.ShortestUnitFast(tmp, this) : Helpers.ShortestUnit(tmp, this);
                if (!Units.Add(unit))
                    unit = Units.FirstOrDefault(i => i.Equals(unit));
                if (!Units.Add(left))
                    left = Units.FirstOrDefault(i => i.Equals(left));
                if (!Units.Add(right))
                    right = Units.FirstOrDefault(i => i.Equals(right));
                AddEdges(@operator, unit, left, right);
            }
            else
            {
                throw new HandleException("Can't use unary operator on 2 parameters", 1903);
            }
        }

        public void AddEdges(IOperator @operator, IUnit res, params IUnit[] @params)
        {
            foreach (var (operands, @op, r) in @operator.GetInferedOperations(res, @params))
            {
                if (!GraphEdges.ContainsKey(operands[0]))
                    GraphEdges.Add(operands[0], new HashSet<OperatorNodeEdge>());
                GraphEdges[operands[0]].Add(new OperatorNodeEdge(op, r, operands.Skip(1).ToArray()));
            };
        }
        public void AddUnit(string name)
        {
            Units.Add(new Unit(name));
        }
        public IUnit GetUnit(string unitName) =>
            Units.FirstOrDefault(i => i.Name == unitName);
        public IOperator GetOperator(string symbol) =>
            Operators.FirstOrDefault(i => i.Symbol == symbol);
    }
}
