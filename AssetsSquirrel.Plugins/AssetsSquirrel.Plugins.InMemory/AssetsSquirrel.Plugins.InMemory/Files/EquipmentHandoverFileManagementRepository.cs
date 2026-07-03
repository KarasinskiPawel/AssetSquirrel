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

        public EquipmentHandoverFileManagementRepository()
        {

        }

        public bool IfFolderExist(int equipmentHandoverId)
        {
            var folderPath = System.IO.Path.Combine(_basePath, equipmentHandoverId.ToString());
            return System.IO.Directory.Exists(folderPath);
        }

        public bool CreateFolder(int equipmentHandoverId)
        {
            var folderPath = System.IO.Path.Combine(_basePath, equipmentHandoverId.ToString());

            if (!System.IO.Directory.Exists(folderPath))
            {
                System.IO.Directory.CreateDirectory(folderPath);
                return true;
            }

            return false;
        }

        public bool IfFilesExist(int equipmentHandoverId)
        {
            var folderPath = System.IO.Path.Combine(_basePath, equipmentHandoverId.ToString());
            return System.IO.Directory.GetFiles(folderPath).Length > 0;
        }

        public bool DeleteFiles(int equipmentHandoverId)
        {
            var folderPath = System.IO.Path.Combine(_basePath, equipmentHandoverId.ToString());
            if (System.IO.Directory.Exists(folderPath))
            {
                var files = System.IO.Directory.GetFiles(folderPath);
                foreach (var file in files)
                {
                    System.IO.File.Delete(file);
                }
                return true;
            }
            return false;
        }

        public async Task<bool> AddNewFile(int equipmentHandoverId, string fileName, string contentType, Stream fileStream)
        {
            var folderPath = System.IO.Path.Combine(_basePath, equipmentHandoverId.ToString());

            if (!System.IO.Directory.Exists(folderPath))
            {
                System.IO.Directory.CreateDirectory(folderPath);
            }

            if (System.IO.Directory.GetFiles(folderPath).Length > 0)
            {
                DeleteFiles(equipmentHandoverId);
            }

            var filePath = System.IO.Path.Combine(folderPath, fileName);

            using (var file = System.IO.File.Create(filePath))
            {
                await fileStream.CopyToAsync(file);
            }
            return true;
        }
    }
}
