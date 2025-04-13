// FolderSyncService.cs
using System;
using System.IO;

public class FolderSyncService : IFolderSyncService
{
    private readonly ILogger _logger;

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

        foreach (var srcFile in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
        {
            string relativePath = srcFile.Substring(source.Length + 1); 
            string destFile = Path.Combine(replica, relativePath); 
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

        foreach (var destFile in Directory.GetFiles(replica, "*", SearchOption.AllDirectories))
        {
            string relativePath = destFile.Substring(replica.Length + 1);
            string srcFile = Path.Combine(source, relativePath);

            if (!File.Exists(srcFile))
            {
                File.Delete(destFile);
                _logger.LogInfo($"Deleted: {relativePath}");
            }
        }
    }
}
