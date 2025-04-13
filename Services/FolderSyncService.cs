// Services/FolderSyncService.cs
using System;
using System.IO;
using FolderSyncApp.Logging;

namespace FolderSyncApp.Services
{
    // This is the actual implementation of the sync logic.
    // It: Creates missing folders in the replica
    // Copies or updates files from source → replica
    // Deletes any files from replica that no longer exist in the source
    // It uses logging to inform what it did (copy, update, delete)
    // Clean, readable, and testable logic based on the interface.

    public class FolderSyncService : IFolderSyncService
    {
        private readonly ILogger _logger;

        // Constructor with dependency injection for logger
        public FolderSyncService(ILogger logger)
        {
            _logger = logger;
        }

        public void Sync(string source, string replica)
        {
            // Ensure replica directory exists
            if (!Directory.Exists(replica))
            {
                Directory.CreateDirectory(replica);
                _logger.LogInfo($"Created replica folder: {replica}");
            }

            // Copy or update files from source to replica
            foreach (var srcFile in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
            {
                string relativePath = srcFile.Substring(source.Length + 1); // Get relative path
                string destFile = Path.Combine(replica, relativePath); // Full path in replica
                string destDir = Path.GetDirectoryName(destFile)!;

                // Ensure destination directory exists
                if (!Directory.Exists(destDir))
                    Directory.CreateDirectory(destDir);

                // Copy file if it doesn't exist or is newer in source
                if (!File.Exists(destFile) || File.GetLastWriteTimeUtc(srcFile) > File.GetLastWriteTimeUtc(destFile))
                {
                    File.Copy(srcFile, destFile, true);
                    _logger.LogInfo($"Copied/Updated: {relativePath}");
                }
            }

            // Remove files from replica that are no longer in source
            foreach (var destFile in Directory.GetFiles(replica, "*", SearchOption.AllDirectories))
            {
                string relativePath = destFile.Substring(replica.Length + 1);
                string srcFile = Path.Combine(source, relativePath);

                // Delete file if it doesn't exist in source
                if (!File.Exists(srcFile))
                {
                    File.Delete(destFile);
                    _logger.LogInfo($"Deleted: {relativePath}");
                }
            }
        }
    }
}
