using System.Collections.Generic;
using System.Linq;

namespace Units.Core.Parser.State
{
    public class ParserState
    {
        public HashSet<IUnit> Units { get; set; } = new HashSet<IUnit>();
        public HashSet<IOperator> Operators { get; set; } = new HashSet<IOperator>();
        public HashSet<MesurmentUnit> MesurmentUnits { get; set; } = new HashSet<MesurmentUnit>();
        public HashSet<SelfOp> SelfOps { get; set; } = new HashSet<SelfOp>();
        public HashSet<RealDef> RealDefs { get; set; } = new HashSet<RealDef>();
        public Dictionary<IUnit, HashSet<OperatorNodeEdge>> GraphEdges { get; set; } = new Dictionary<IUnit, HashSet<OperatorNodeEdge>>();

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
        public void AddNewCompositUnit(IUnit left, IOperator @operator, IUnit right)
        {
            if (@operator is BinaryOperator opb)
            {
                var unit = new BinaryCompositUnit(left, opb, right).Simplify();
                Units.Add(unit);
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

    public interface IExportHandle
    {
        bool ExportAll(ParserState state, string location) =>
            ExportUnits(state, location) && ExportWrappers(state, location);
        bool ExportUnits(ParserState state, string location);
        bool ExportWrappers(ParserState state, string location);
    }
    public class MockExportHandle : IExportHandle
    {
        public bool ExportUnits(ParserState state, string location)
        {
            return true;
        }

        public bool ExportWrappers(ParserState state, string location)
        {
            return true;
        }
    }
}
