namespace FolderSyncApp.Services
{
    // Interface for folder sync service
    // This is an interface that defines what a folder sync service should do.
    // It just says: “You must implement a method called Sync(source, replica)”.
    // This allows flexibility in changing or testing sync logic later.
    public interface IFolderSyncService
    {
        void Sync(string sourcePath, string replicaPath);
    }



}