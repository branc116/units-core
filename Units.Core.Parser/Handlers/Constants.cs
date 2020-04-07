using System.Text.RegularExpressions;

namespace Units.Core.Parser.Handlers
{
    /// <summary>
    /// Regex constants.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Used for <see cref="State.Unit.Name"/>, <see cref="State.MesurmentUnit.Name"/>, <see cref="State.RealDef.WrapName"/>
        /// </summary>
        public const string UnitName = @"((\w)+)";
        /// <summary>
        /// Used for <see cref="State.RealDef.ClrName"/>
        /// </summary>
        public const string FullName = @"((\w|\.)+)";
        /// <summary>
        /// Used for <see cref="State.Operator.Symbol"/>
        /// </summary>
        public const string OperatorReg = @"((<=)|(>=)|(==)|(!=)|(&&)|<|>|\*|\+|-|/|\||&)";
        /// <summary>
        /// Used for something like this "(...) | (...) | (...)"
        /// </summary>
        public const string EndOrMore = @"(( *\| *)|( *$))";
        public const string FloatLike = @"((\w|\.|-|\+)+)"; //TODO
    }

}
