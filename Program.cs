using System;
using System.Threading;
using FolderSyncApp.Services;
using FolderSyncApp.Logging;

namespace FolderSyncApp
{

// This is the starting point of the app.
// It reads command-line arguments (source path, replica path, sync interval, log file path).
// It sets up dependencies: the sync service and logger.
// It runs an infinite loop to perform sync every X seconds.

// run  - > dotnet run "/Users/erikmyrland/Desktop/sourceFolder" "/Users/erikmyrland/Desktop/replicaFolder" 10 "/Users/erikmyrland/Desktop/sync_log.txt"


    class Program
    {
        static void Main(string[] args)
        {
            // Validate command-line arguments
            if (args.Length != 4)
            {
                Console.WriteLine("Usage: dotnet run <source> <replica> <intervalInSeconds> <logFilePath>");
                return;
            }

            // Read input arguments
            string sourcePath = args[0]; // Source folder path
            string replicaPath = args[1]; // Replica folder path
            int intervalSeconds = int.Parse(args[2]); // Sync interval in seconds
            string logFilePath = args[3]; // Log file path

            // Initialize logger and folder sync service
            ILogger logger = new ConsoleFileLogger(logFilePath);
            IFolderSyncService syncService = new FolderSyncService(logger);

            logger.LogInfo($"Starting sync every {intervalSeconds} seconds...");

            // Infinite loop to perform periodic syncing
            while (true)
            {
                try
                {
                    // Perform folder synchronization
                    syncService.Sync(sourcePath, replicaPath);
                }
                catch (Exception ex)
                {
                    // Log any unexpected errors
                    logger.LogError("Error during sync: " + ex.Message);
                }

                // Wait for the next sync interval
                Thread.Sleep(intervalSeconds * 1000);
            }
        }
    }
}