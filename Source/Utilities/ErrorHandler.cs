using System;
using Godot;
using Environment = System.Environment;
using FileAccess = Godot.FileAccess;


namespace AlienInvasionLogistics.Source.Utilities;

public partial class ErrorHandler : Node
{
    private const string LogFilePath = "user://error_log.txt";
    private static ErrorHandler _instance;

    public override void _Ready()
    {
        _instance = this;
        GD.Print("ErrorHandler initialized");
    }

    public static void LogMessage(string message, Exception ex = null, ErrorUtilities.MessageLevel severity = ErrorUtilities.MessageLevel.Error)
    {
        var errorInfo = new ErrorUtilities.ErrorInfo
        {
            Message = message,
            Exception = ex,
            Severity = severity,
            Timestamp = DateTime.UtcNow,
            StackTrace = ex?.StackTrace ?? Environment.StackTrace
        };

        // Log to Godot console
        var logMessage = $"[{severity}] {message}";
        if (ex != null) logMessage += $"\n  Exception: {ex.Message}\n  StackTrace: {ex.StackTrace}";

        switch (severity)
        {
            case ErrorUtilities.MessageLevel.Info:
                GD.Print(logMessage);
                break;
            case ErrorUtilities.MessageLevel.Warning:
                GD.PushWarning(logMessage);
                break;
            case ErrorUtilities.MessageLevel.Error:
            case ErrorUtilities.MessageLevel.Critical:
                GD.PrintErr(logMessage);
                break;
        }

        // Write to log file
        WriteToLogFile(errorInfo);

        // Show dialog for critical errors
        if (severity == ErrorUtilities.MessageLevel.Critical && _instance != null)
            _instance.CallDeferred(nameof(ShowCriticalErrorDialog), message);
    }

    private static void WriteToLogFile(ErrorUtilities.ErrorInfo errorInfo)
    {
        try
        {
            using var file = FileAccess.Open(LogFilePath, FileAccess.ModeFlags.ReadWrite);
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
        dialog.DialogText =
            $"A critical error occurred:\n\n{message}\n\nThe application will continue, but may be unstable.";
        dialog.Title = "Critical Error";
        dialog.Size = new Vector2I(500, 200);
        AddChild(dialog);
        dialog.PopupCentered();
    }

    public static void HandleDatabaseError(Exception ex, string operation)
    {
        LogMessage($"Database error during {operation}", ex);
    }

    public static void HandleSceneLoadError(Exception ex, string scenePath)
    {
        LogMessage($"Failed to load scene: {scenePath}", ex, ErrorUtilities.MessageLevel.Critical);
    }
}