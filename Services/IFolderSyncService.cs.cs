// IFolderSyncService.cs
// Interface for folder sync service
public interface IFolderSyncService
{
    void Sync(string sourcePath, string replicaPath);
}