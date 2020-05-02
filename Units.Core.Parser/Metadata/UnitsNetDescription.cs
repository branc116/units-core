using System.Collections.Generic;
using System.Linq;
using Units.Core.Parser.State;

namespace Units.Core.Parser.Metadata
{
    public class UnitsNetDescription
    {
        public string Name { get; set; }
        public string BaseUnit { get; set; }
        public string XmlDoc { get; set; }
        public Dictionary<string, int> BaseDimensions { get; set; }
        public UnitsNetUnit[] Units { get; set; }
    }

    public class Basedimensions
    {
        public static Dictionary<string, IUnit> Bases = new Dictionary<string, IUnit>
        {
            {"N", new State.Unit("AmountOfSubstance") },
            {"I", new State.Unit("ElectricCurrent") },
            {"L", new State.Unit("Length") },
            {"J", new State.Unit("LuminousIntensity") },
            {"M", new State.Unit("Mass") },
            {"Θ", new State.Unit("Temperature") },
            {"T", new State.Unit("Duration") }
        };
        /// <summary>AmountOfSubstance.</summary>
        public int N { get; set; }
        /// <summary>ElectricCurrent.</summary>
        public int I { get; set; }
        /// <summary>Length.</summary>
        public int L { get; set; }
        /// <summary>LuminousIntensity.</summary>
        public int J { get; set; }
        /// <summary>Mass.</summary>
        public int M { get; set; }
        /// <summary>Temperature.</summary>
        public int Θ { get; set; }
        /// <summary>Time.</summary>
        public int T { get; set; }
    }

    public class UnitsNetUnit
    {
        public string SingularName { get; set; }
        public string PluralName { get; set; }
        public UnitsNetBaseunits BaseUnits { get; set; }
        public string FromUnitToBaseFunc { get; set; }
        public string FromBaseToUnitFunc { get; set; }
        public string[] Prefixes { get; set; }
        public Localization[] Localization { get; set; }
        public string XmlDocSummary { get; set; }
        public string XmlDocRemarks { get; set; }
        public string Postfix => Localization.FirstOrDefault(i => i.Culture == "en-US")?.Abbreviations.FirstOrDefault();
    }

    public class UnitsNetBaseunits
    {
        public string L { get; set; }
    }

    public class Localization
    {
        public string Culture { get; set; }
        public string[] Abbreviations { get; set; }
    }
}
