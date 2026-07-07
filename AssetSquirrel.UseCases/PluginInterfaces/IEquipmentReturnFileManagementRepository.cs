namespace AssetSquirrel.UseCases.PluginInterfaces
{
    public interface IEquipmentReturnFileManagementRepository
    {
        Task<bool> AddNewFile(int equipmentReturnId, string fileName, string contentType, Stream fileStream);
        bool CreateFolder(int equipmentReturnId);
        bool DeleteFiles(int equipmentReturnId);
        bool IfFilesExist(int equipmentReturnId);
        bool IfFolderExist(int equipmentReturnId);
    }
}
