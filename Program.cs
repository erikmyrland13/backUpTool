using System;
using System.IO;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        // Check if the user provided 4 command-line arguments
        if (args.Length != 4)
        {
            Console.WriteLine("Usage: dotnet run <source> <replica> <intervalInSeconds> <logFilePath>");
            return;
        }

        // Assign each argument to a variable
        string sourcePath = args[0];                      // Path to the source folder
        string replicaPath = args[1];                     // Path to the replica (copy) folder
        int intervalSeconds = int.Parse(args[2]);         // Time interval (in seconds) for sync
        string logFile = args[3];                         // Path to the log file

        // Initial log message
        Log($"Starting folder sync every {intervalSeconds} seconds...", logFile);

        // Infinite loop to repeat sync every X seconds
        while (true)
        {
            try
            {
                // Attempt to sync the folders
                SyncFolders(sourcePath, replicaPath, logFile);
            }
            catch (Exception ex)
            {
                // Log any unexpected errors during sync
                Log($"Error during sync: {ex.Message}", logFile);
            }

            // Wait for the specified interval before syncing again
            Thread.Sleep(intervalSeconds * 1000);
        }
    }

    // Method to sync content from source to replica
    static void SyncFolders(string source, string replica, string logFile)
    {
        // Create the replica folder if it doesn't exist
        if (!Directory.Exists(replica))
        {
            Directory.CreateDirectory(replica);
            Log($"Created replica folder: {replica}", logFile);
        }

        // Loop through all files in the source folder (including subfolders)
        foreach (string srcFile in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
        {
            // Calculate the relative path to maintain folder structure
            string relativePath = srcFile.Substring(source.Length + 1);
            string destFile = Path.Combine(replica, relativePath);  // Where file should go in replica
            string destDir = Path.GetDirectoryName(destFile)!;       // Ensure directory exists

            // Create destination directory if needed
            if (!Directory.Exists(destDir))
                Directory.CreateDirectory(destDir);

            // Copy file if it doesn't exist OR if it's newer in source
            if (!File.Exists(destFile) || File.GetLastWriteTimeUtc(srcFile) > File.GetLastWriteTimeUtc(destFile))
            {
                File.Copy(srcFile, destFile, true);
                Log($"Copied/Updated: {relativePath}", logFile);
            }
        }

        // Loop through all files in the replica folder
        foreach (string destFile in Directory.GetFiles(replica, "*", SearchOption.AllDirectories))
        {
            // Get the relative path from the replica folder
            string relativePath = destFile.Substring(replica.Length + 1);
            string srcFile = Path.Combine(source, relativePath);  // The equivalent file in source

            // Delete the file from replica if it no longer exists in source
            if (!File.Exists(srcFile))
            {
                File.Delete(destFile);
                Log($"Deleted: {relativePath}", logFile);
            }
        }
    }

    // Method to log messages to both console and a log file
    static void Log(string message, string logFile)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string fullMessage = $"[{timestamp}] {message}";

        Console.WriteLine(fullMessage);  // Print to terminal
        File.AppendAllText(logFile, fullMessage + Environment.NewLine);  // Write to log file
    }
}
