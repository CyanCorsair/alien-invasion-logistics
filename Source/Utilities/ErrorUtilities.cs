using System;

namespace AlienInvasionLogistics.Source.Utilities;

public static class ErrorUtilities
{
    public enum MessageLevel
    {
        Info,
        Warning,
        Error,
        Critical
    }

    public class ErrorInfo
    {
        public string Message { get; set; }
        public Exception Exception { get; set; }
        public MessageLevel Severity { get; set; }
        public DateTime Timestamp { get; set; }
        public string StackTrace { get; set; }
    }
}