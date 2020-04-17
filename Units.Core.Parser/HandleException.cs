using System;

namespace Units.Core.Parser
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "Error code must be in a constructor")]
    public class HandleException : Exception
    {
        public int ErrorCode { get; }
        public HandleException(string message, int errorCore) : base(message.Trim())
        {
            ErrorCode = errorCore;
        }

        public HandleException(string message, Exception innerException, int errorCore) : base(message.Trim(), innerException)
        {
            ErrorCode = errorCore;
        }
    }
}
