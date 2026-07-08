using AssetSquirrel.CoreBusiness;
using AssetSquirrel.UseCases.PluginInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetsSquirrel.Plugins.InMemory.Files
{
    public class EquipmentHandoverFileManagementRepository : IEquipmentHandoverFileManagementRepository
    {
        const string BaseFolder = @"Files\EquipmentHandovers";
        private readonly string _basePath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot", BaseFolder);
        private readonly IErrorsRepository errorsRepository;

        public EquipmentHandoverFileManagementRepository(IErrorsRepository errorsRepository)
        {
            this.errorsRepository = errorsRepository;
        }

        public async Task<Result<bool>> IfFolderExist(int equipmentHandoverId)
        {
            try
            {
                var folderPath = System.IO.Path.Combine(_basePath, equipmentHandoverId.ToString());
                return Result<bool>.Ok(System.IO.Directory.Exists(folderPath));
            }
            catch (Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.InMemory.Files", "EquipmentHandoverFileManagementRepository", "IfFolderExist", e);
                return Result<bool>.Fail(e.Message);
            }
        }

        public async Task<Result<bool>> CreateFolder(int equipmentHandoverId)
        {
            try
            {
                var folderPath = System.IO.Path.Combine(_basePath, equipmentHandoverId.ToString());

                if (!System.IO.Directory.Exists(folderPath))
                {
                    System.IO.Directory.CreateDirectory(folderPath);
                    return Result<bool>.Ok(true);
                }

                return Result<bool>.Ok(false);
            }
            catch (Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.InMemory.Files", "EquipmentHandoverFileManagementRepository", "CreateFolder", e);
                return Result<bool>.Fail(e.Message);
            }
        }

        public async Task<Result<bool>> IfFilesExist(int equipmentHandoverId)
        {
            try
            {
                var folderPath = System.IO.Path.Combine(_basePath, equipmentHandoverId.ToString());
                return Result<bool>.Ok(System.IO.Directory.GetFiles(folderPath).Length > 0);
            }
            catch (Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.InMemory.Files", "EquipmentHandoverFileManagementRepository", "IfFilesExist", e);
                return Result<bool>.Fail(e.Message);
            }
        }

        public async Task<Result<bool>> DeleteFiles(int equipmentHandoverId)
        {
            try
            {
                var folderPath = System.IO.Path.Combine(_basePath, equipmentHandoverId.ToString());
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
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.InMemory.Files", "EquipmentHandoverFileManagementRepository", "DeleteFiles", e);
                return Result<bool>.Fail(e.Message);
            }
        }

        public async Task<Result<bool>> AddNewFile(int equipmentHandoverId, string fileName, string contentType, Stream fileStream)
        {
            try
            {
                var folderPath = System.IO.Path.Combine(_basePath, equipmentHandoverId.ToString());

                if (!System.IO.Directory.Exists(folderPath))
                {
                    System.IO.Directory.CreateDirectory(folderPath);
                }

                if (System.IO.Directory.GetFiles(folderPath).Length > 0)
                {
                    await DeleteFiles(equipmentHandoverId);
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
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.InMemory.Files", "EquipmentHandoverFileManagementRepository", "AddNewFile", e);
                return Result<bool>.Fail(e.Message);
            }
        }
    }
}
