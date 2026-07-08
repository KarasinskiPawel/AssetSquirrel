using AssetSquirrel.CoreBusiness;
using AssetSquirrel.UseCases.PluginInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetsSquirrel.Plugins.InMemory.Files
{
    public class FileManagementRepository : IFileManagementRepository
    {
        const string BaseFolder = @"Files\Invoices";
        private readonly string _basePath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot", BaseFolder);
        private readonly IErrorsRepository errorsRepository;

        public FileManagementRepository(IErrorsRepository errorsRepository)
        {
            this.errorsRepository = errorsRepository;
        }

        public async Task<Result<bool>> IfFolderExist(int invoiceId)
        {
            try
            {
                var folderPath = System.IO.Path.Combine(_basePath, invoiceId.ToString());
                return Result<bool>.Ok(System.IO.Directory.Exists(folderPath));
            }
            catch (Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.InMemory.Files", "FileManagementRepository", "IfFolderExist", e);
                return Result<bool>.Fail(e.Message);
            }
        }

        public async Task<Result<bool>> CreateFolder(int invoiceId)
        {
            try
            {
                var folderPath = System.IO.Path.Combine(_basePath, invoiceId.ToString());

                if (!System.IO.Directory.Exists(folderPath))
                {
                    System.IO.Directory.CreateDirectory(folderPath);
                    return Result<bool>.Ok(true);
                }

                return Result<bool>.Ok(false);
            }
            catch (Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.InMemory.Files", "FileManagementRepository", "CreateFolder", e);
                return Result<bool>.Fail(e.Message);
            }
        }

        public async Task<Result<bool>> IfFilesExist(int invoiceId)
        {
            try
            {
                var folderPath = System.IO.Path.Combine(_basePath, invoiceId.ToString());
                return Result<bool>.Ok(System.IO.Directory.GetFiles(folderPath).Length > 0);
            }
            catch (Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.InMemory.Files", "FileManagementRepository", "IfFilesExist", e);
                return Result<bool>.Fail(e.Message);
            }
        }

        public async Task<Result<bool>> DeleteFiles(int invoiceId)
        {
            try
            {
                var folderPath = System.IO.Path.Combine(_basePath, invoiceId.ToString());
                if (System.IO.Directory.Exists(folderPath))
                {
                    var files = System.IO.Directory.GetFiles(folderPath);
                    foreach (var file in files)
                    {
                        System.IO.File.Delete(file);
                    }
                    return Result<bool>.Ok(true);
                }
                return Result<bool>.Ok(false);
            }
            catch (Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.InMemory.Files", "FileManagementRepository", "DeleteFiles", e);
                return Result<bool>.Fail(e.Message);
            }
        }

        public async Task<Result<bool>> AddNewFile(int invoiceId, string fileName, string contentType, Stream fileStream)
        {
            try
            {
                var folderPath = System.IO.Path.Combine(_basePath, invoiceId.ToString());

                if (!System.IO.Directory.Exists(folderPath))
                {
                    System.IO.Directory.CreateDirectory(folderPath);
                }

                if (System.IO.Directory.GetFiles(folderPath).Length > 0)
                {
                    await DeleteFiles(invoiceId);
                }

                var filePath = System.IO.Path.Combine(folderPath, fileName);

                using (var file = System.IO.File.Create(filePath))
                {
                    await fileStream.CopyToAsync(file);
                }
                return Result<bool>.Ok(true);
            }
            catch (Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.InMemory.Files", "FileManagementRepository", "AddNewFile", e);
                return Result<bool>.Fail(e.Message);
            }
        }
    }
}
