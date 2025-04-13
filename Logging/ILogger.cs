namespace FolderSyncApp.Logging
{
    // Interface for logging functionality
    // An interface for logging.
    // It defines two methods:
    // LogInfo() — for normal log messages
    // LogError() — for error messages
    // Just like with sync, this allows you to swap out logging systems easily.
    public interface ILogger
    {
        void LogInfo(string message); // Logs informational message
        void LogError(string message); // Logs error message
    }
}