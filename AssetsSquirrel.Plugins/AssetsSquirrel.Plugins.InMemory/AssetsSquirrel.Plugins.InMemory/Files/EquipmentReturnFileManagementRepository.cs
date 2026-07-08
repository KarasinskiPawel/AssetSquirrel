using AssetSquirrel.CoreBusiness;
using AssetSquirrel.UseCases.PluginInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetsSquirrel.Plugins.InMemory.Files
{
    public class EquipmentReturnFileManagementRepository : IEquipmentReturnFileManagementRepository
    {
        const string BaseFolder = @"Files\EquipmentReturns";
        private readonly string _basePath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot", BaseFolder);
        private readonly IErrorsRepository errorsRepository;

        public EquipmentReturnFileManagementRepository(IErrorsRepository errorsRepository)
        {
            this.errorsRepository = errorsRepository;
        }

        public async Task<Result<bool>> IfFolderExist(int equipmentReturnId)
        {
            try
            {
                var folderPath = System.IO.Path.Combine(_basePath, equipmentReturnId.ToString());
                return Result<bool>.Ok(System.IO.Directory.Exists(folderPath));
            }
            catch (Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.InMemory.Files", "EquipmentReturnFileManagementRepository", "IfFolderExist", e);
                return Result<bool>.Fail(e.Message);
            }
        }

        public async Task<Result<bool>> CreateFolder(int equipmentReturnId)
        {
            try
            {
                var folderPath = System.IO.Path.Combine(_basePath, equipmentReturnId.ToString());

                if (!System.IO.Directory.Exists(folderPath))
                {
                    System.IO.Directory.CreateDirectory(folderPath);
                    return Result<bool>.Ok(true);
                }

                return Result<bool>.Ok(false);
            }
            catch (Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.InMemory.Files", "EquipmentReturnFileManagementRepository", "CreateFolder", e);
                return Result<bool>.Fail(e.Message);
            }
        }

        public async Task<Result<bool>> IfFilesExist(int equipmentReturnId)
        {
            try
            {
                var folderPath = System.IO.Path.Combine(_basePath, equipmentReturnId.ToString());
                return Result<bool>.Ok(System.IO.Directory.GetFiles(folderPath).Length > 0);
            }
            catch (Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.InMemory.Files", "EquipmentReturnFileManagementRepository", "IfFilesExist", e);
                return Result<bool>.Fail(e.Message);
            }
        }

        public async Task<Result<bool>> DeleteFiles(int equipmentReturnId)
        {
            try
            {
                var folderPath = System.IO.Path.Combine(_basePath, equipmentReturnId.ToString());
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
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.InMemory.Files", "EquipmentReturnFileManagementRepository", "DeleteFiles", e);
                return Result<bool>.Fail(e.Message);
            }
        }

        public async Task<Result<bool>> AddNewFile(int equipmentReturnId, string fileName, string contentType, Stream fileStream)
        {
            try
            {
                var folderPath = System.IO.Path.Combine(_basePath, equipmentReturnId.ToString());

                if (!System.IO.Directory.Exists(folderPath))
                {
                    System.IO.Directory.CreateDirectory(folderPath);
                }

                if (System.IO.Directory.GetFiles(folderPath).Length > 0)
                {
                    await DeleteFiles(equipmentReturnId);
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
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.InMemory.Files", "EquipmentReturnFileManagementRepository", "AddNewFile", e);
                return Result<bool>.Fail(e.Message);
            }
        }
    }
}
