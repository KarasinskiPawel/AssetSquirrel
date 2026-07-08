using AssetSquirrel.CoreBusiness;

namespace AssetSquirrel.UseCases.PluginInterfaces
{
    public interface IEquipmentHandoverFileManagementRepository
    {
        Task<Result<bool>> AddNewFile(int equipmentHandoverId, string fileName, string contentType, Stream fileStream);
        Task<Result<bool>> CreateFolder(int equipmentHandoverId);
        Task<Result<bool>> DeleteFiles(int equipmentHandoverId);
        Task<Result<bool>> IfFilesExist(int equipmentHandoverId);
        Task<Result<bool>> IfFolderExist(int equipmentHandoverId);
    }
}
