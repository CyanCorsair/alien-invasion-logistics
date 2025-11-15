using Godot;
using System;
using System.IO;

namespace Core.Utilities
{
    public enum ErrorSeverity
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
        public ErrorSeverity Severity { get; set; }
        public DateTime Timestamp { get; set; }
        public string StackTrace { get; set; }
    }

    [GlobalClass]
    public partial class ErrorHandler : Node
    {
        private static ErrorHandler _instance;
        private const string LOG_FILE_PATH = "user://error_log.txt";

        public override void _Ready()
        {
            _instance = this;
            GD.Print("ErrorHandler initialized");
        }

        public static void LogError(string message, Exception ex = null, ErrorSeverity severity = ErrorSeverity.Error)
        {
            var errorInfo = new ErrorInfo
            {
                Message = message,
                Exception = ex,
                Severity = severity,
                Timestamp = DateTime.UtcNow,
                StackTrace = ex?.StackTrace ?? Environment.StackTrace
            };

            // Log to Godot console
            var logMessage = $"[{severity}] {message}";
            if (ex != null)
            {
                logMessage += $"\n  Exception: {ex.Message}\n  StackTrace: {ex.StackTrace}";
            }

            switch (severity)
            {
                case ErrorSeverity.Info:
                    GD.Print(logMessage);
                    break;
                case ErrorSeverity.Warning:
                    GD.PushWarning(logMessage);
                    break;
                case ErrorSeverity.Error:
                case ErrorSeverity.Critical:
                    GD.PrintErr(logMessage);
                    break;
            }

            // Write to log file
            WriteToLogFile(errorInfo);

            // Show dialog for critical errors
            if (severity == ErrorSeverity.Critical && _instance != null)
            {
                _instance.CallDeferred(nameof(ShowCriticalErrorDialog), message);
            }
        }

        private static void WriteToLogFile(ErrorInfo errorInfo)
        {
            try
            {
                using var file = FileAccess.Open(LOG_FILE_PATH, FileAccess.ModeFlags.ReadWrite);
                if (file != null)
                {
                    file.SeekEnd();
                    file.StoreLine($"\n[{errorInfo.Timestamp:yyyy-MM-dd HH:mm:ss}] [{errorInfo.Severity}]");
                    file.StoreLine($"Message: {errorInfo.Message}");
                    if (errorInfo.Exception != null)
                    {
                        file.StoreLine($"Exception: {errorInfo.Exception.Message}");
                        file.StoreLine($"StackTrace: {errorInfo.StackTrace}");
                    }
                    file.StoreLine("---");
                }
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Failed to write to log file: {ex.Message}");
            }
        }

        private void ShowCriticalErrorDialog(string message)
        {
            var dialog = new AcceptDialog();
            dialog.DialogText = $"A critical error occurred:\n\n{message}\n\nThe application will continue, but may be unstable.";
            dialog.Title = "Critical Error";
            dialog.Size = new Vector2I(500, 200);
            AddChild(dialog);
            dialog.PopupCentered();
        }

        public static void HandleDatabaseError(Exception ex, string operation)
        {
            LogError($"Database error during {operation}", ex, ErrorSeverity.Error);
        }

        public static void HandleSceneLoadError(Exception ex, string scenePath)
        {
            LogError($"Failed to load scene: {scenePath}", ex, ErrorSeverity.Critical);
        }
    }
}
