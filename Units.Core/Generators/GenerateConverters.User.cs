using System;
using System.Collections.Generic;
using System.Linq;
using Units.Core.Parser;
using Units.Core.Parser.State;

namespace Units.Core.Generators
{
    public partial class GenerateConverters
    {
        public ParserState State { get; }
        public IUnit Unit { get; }
        public GenerateConverters(ParserState state, IUnit unit)
        {
            State = state;
            Unit = unit;
        }
        public List<ConverterModel> GetConverts()
        {
            var rawName = Unit is Scalar ? "this" : "RawValue";
            var a = State.MesurmentUnits
                .Where(i => i.For.Equals(Unit))
                .SelectMany(i =>
                {
                    var expr1 = $"new {Unit.Name}({i.ConvertFrom.Aprox().AddExplicitConvertToNumbers("Scalar")})";
                    var expr2 = i.ConvertTo.Aprox().AddExplicitConvertToNumbers("Scalar").Replace("x", rawName);
                    return new[] {
                        new ConverterModel($"static {Unit.Name} From{i.Name}", @params: "Scalar x", expr1, i.Summary, i.Remarks ?? $"https://duckduckgo.com/?q={i.Name}"),
                        new ConverterModel($"Scalar To{i.Name}", string.Empty, expr2, i.Summary, i.Remarks ?? $"https://duckduckgo.com/?q={i.Name}")
                    };
                })
                .ToList();
            return a;
        }
    }

    public class ConverterModel
    {
        public readonly string name;
        public readonly string @params;
        public readonly string expr;
        public readonly string summary;
        public readonly string remarks;

        public ConverterModel(string name, string @params, string expr, string summary, string remarks)
        {
            this.name = name;
            this.@params = @params;
            this.expr = expr;
            this.summary = summary ?? expr;
            this.remarks = remarks ?? string.Empty;
        }
    }
}
