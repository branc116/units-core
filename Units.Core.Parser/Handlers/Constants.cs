namespace Units.Core.Parser.Handlers
{
    public static class Constants
    {
        public const string UnitName = @"((\w)+)";
        public const string FullName = @"((\w|\.)+)";
        public const string OperatorReg = @"((<=)|(>=)|(==)|(!=)|(&&)|<|>|\*|\+|-|/|\||&)";
        public const string EndOrMore = @"(( *\| *)|( *$))";
    }

}
