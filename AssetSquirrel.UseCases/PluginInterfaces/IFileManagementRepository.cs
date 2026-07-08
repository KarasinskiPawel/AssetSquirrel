using AssetSquirrel.CoreBusiness;

namespace AssetSquirrel.UseCases.PluginInterfaces
{
    public interface IFileManagementRepository
    {
        Task<Result<bool>> AddNewFile(int invoiceId, string fileName, string contentType, Stream fileStream);
        Task<Result<bool>> CreateFolder(int invoiceId);
        Task<Result<bool>> DeleteFiles(int invoiceId);
        Task<Result<bool>> IfFilesExist(int invoiceId);
        Task<Result<bool>> IfFolderExist(int invoiceId);
    }
}
