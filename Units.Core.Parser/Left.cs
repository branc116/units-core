#nullable enable
namespace Units.Core.Parser
{
    internal class Left
    {
        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public Left(string className, string methodName)
        {
            ClassName = className;
            MethodName = methodName;
        }
    }
}
