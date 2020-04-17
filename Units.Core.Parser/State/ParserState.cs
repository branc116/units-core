using System.Collections.Generic;
using System.Linq;

namespace Units.Core.Parser.State
{
    public class ParserState
    {
        public HashSet<IUnit> Units { get; set; } = new HashSet<IUnit>();
        public HashSet<Operator> Operators { get; set; } = new HashSet<Operator>();
        public HashSet<MesurmentUnit> MesurmentUnits { get; set; } = new HashSet<MesurmentUnit>();
        public HashSet<SelfOp> SelfOps { get; set; } = new HashSet<SelfOp>();
        public HashSet<RealDef> RealDefs { get; set; } = new HashSet<RealDef>();
        public Dictionary<IUnit, HashSet<(Operator op, IUnit op2, IUnit res)>> GraphEdges { get; set; } = new Dictionary<IUnit, HashSet<(Operator, IUnit, IUnit)>>();

        public IExportHandle Exporter { get; set; }

        public void AddNewCompositUnit(string leftUnit, string symbol, string rightUnit, string newUnitName)
        {
            var u1 = GetUnit(leftUnit);
            var u2 = GetUnit(rightUnit);
            var op = GetOperator(symbol);
            var unit = new CompositUnit(u1, op, u2, newUnitName);
            Units.Add(unit);
            AddEdges(unit, u1, op, u2);
        }
        public void AddNewCompositUnit(IUnit left, Operator @operator, IUnit right)
        {
            var unit = new CompositUnit(left, @operator, right).Simplify();
            Units.Add(unit);
            AddEdges(unit, left, @operator, right);
        }
        public void AddEdges(IUnit res, IUnit left, Operator @operator, IUnit right)
        {
            foreach (var (op1, @op, op2, ress) in @operator.GetEdges(left, right, res))
            {
                if (!GraphEdges.ContainsKey(op1))
                    GraphEdges.Add(op1, new HashSet<(Operator, IUnit, IUnit)>());
                GraphEdges[op1].Add((@op, op2, ress));
            };
        }
        public void AddUnit(string name)
        {
            Units.Add(new Unit(name));
        }
        public IUnit GetUnit(string unitName) =>
            Units.FirstOrDefault(i => i.Name == unitName);
        public Operator GetOperator(string symbol) =>
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
