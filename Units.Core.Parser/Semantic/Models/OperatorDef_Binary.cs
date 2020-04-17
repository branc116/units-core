namespace Units.Core.Parser.Semantic.Models
{
    public struct OperatorDef_Binary
    {
        public char Res;
        public char Left;
        public char Symbol;
        public char Right;

        public OperatorDef_Binary(char res, char left, char symbol, char right)
        {
            Res = res;
            Left = left;
            Symbol = symbol;
            Right = right;
        }
    }
}
