namespace Units.Core.Parser.Semantic.Models
{
    public struct OperatorDef_Unary
    {
        public char Right;
        public char Res;
        public string Operator;

        public OperatorDef_Unary(char res, string @operator, char right)
        {
            Right = right;
            Res = res;
            Operator = @operator;
        }
    }
}
