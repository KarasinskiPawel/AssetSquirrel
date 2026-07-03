namespace AssetSquirrel.UseCases.PluginInterfaces
{
    public interface IEquipmentHandoverFileManagementRepository
    {
        Task<bool> AddNewFile(int equipmentHandoverId, string fileName, string contentType, Stream fileStream);
        bool CreateFolder(int equipmentHandoverId);
        bool DeleteFiles(int equipmentHandoverId);
        bool IfFilesExist(int equipmentHandoverId);
        bool IfFolderExist(int equipmentHandoverId);
    }
}
