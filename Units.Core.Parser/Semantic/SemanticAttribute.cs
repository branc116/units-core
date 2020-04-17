using System;
using System.Text.RegularExpressions;

namespace Units.Core.Parser.Semantic
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class SemanticAttribute : Attribute
    {
        public SemanticAttribute(SemanticMatch semanticMatch = SemanticMatch.All, SemanticHandleKind semanticHandleKind = SemanticHandleKind.ForEach)
        {
            SemanticMatch = semanticMatch;
            SemanticHandleKind = semanticHandleKind;
        }
        public SemanticAttribute(string name, SemanticMatch semanticMatch = SemanticMatch.Exact, SemanticHandleKind semanticHandleKind = SemanticHandleKind.ForEach)
        {
            Name = name;
            SemanticHandleKind = semanticHandleKind;
            SemanticMatch = semanticMatch;
        }
        public bool IsMatch(string input)
        {
            var isMatch = SemanticMatch switch
            {
                SemanticMatch.All => true,
                SemanticMatch.Exact => Name.Equals(input, StringComparison.OrdinalIgnoreCase),
                SemanticMatch.Regex => Regex.IsMatch(input, Name),
                _ => false
            };
            return isMatch;
        }
        public string Name { get; set; }
        public SemanticMatch SemanticMatch { get; set; }
        public SemanticHandleKind SemanticHandleKind { get; set; }
    }
}
