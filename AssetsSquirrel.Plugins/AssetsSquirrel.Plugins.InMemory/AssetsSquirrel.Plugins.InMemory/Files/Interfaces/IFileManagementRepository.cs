namespace AssetsSquirrel.Plugins.InMemory.Files.Interfaces
{
    public interface IFileManagementRepository
    {
        bool AddNewFile(int invoiceId, string fileName, string contentType, Stream fileStream);
        bool CreateFolder(int invoiceId);
        bool DeleteFiles(int invoiceId);
        bool IfFilesExist(int invoiceId);
        bool IfFolderExist(int invoiceId);
    }
}