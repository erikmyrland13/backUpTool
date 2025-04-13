using System;
using System.IO;

namespace FolderSyncApp.Logging
{
    // Logger implementation that writes to console and file
    // A basic logger that writes messages to:
    // The console
    // A text log file
    // It formats log entries with:
    // Timestamps
    // Log levels (INFO or ERROR)
    // Very useful for debugging or reviewing sync history.


    public class ConsoleFileLogger : ILogger
    {
        private readonly string _logFilePath;

        // Constructor accepting log file path
        public ConsoleFileLogger(string logFilePath)
        {
            _logFilePath = logFilePath;
        }

        // Log informational message
        public void LogInfo(string message)
        {
            Log("INFO", message);
        }

        // Log error message
        public void LogError(string message)
        {
            Log("ERROR", message);
        }

        // Internal method to log message with timestamp and level
        private void Log(string level, string message)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string fullMessage = $"[{timestamp}] [{level}] {message}";

            // Output to console
            Console.WriteLine(fullMessage);

            // Append to log file
            File.AppendAllText(_logFilePath, fullMessage + Environment.NewLine);
        }
    }
}
