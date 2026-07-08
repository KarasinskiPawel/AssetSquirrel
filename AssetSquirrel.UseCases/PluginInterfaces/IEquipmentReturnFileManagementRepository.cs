using AssetSquirrel.CoreBusiness;

namespace AssetSquirrel.UseCases.PluginInterfaces
{
    public interface IEquipmentReturnFileManagementRepository
    {
        Task<Result<bool>> AddNewFile(int equipmentReturnId, string fileName, string contentType, Stream fileStream);
        Task<Result<bool>> CreateFolder(int equipmentReturnId);
        Task<Result<bool>> DeleteFiles(int equipmentReturnId);
        Task<Result<bool>> IfFilesExist(int equipmentReturnId);
        Task<Result<bool>> IfFolderExist(int equipmentReturnId);
    }
}
