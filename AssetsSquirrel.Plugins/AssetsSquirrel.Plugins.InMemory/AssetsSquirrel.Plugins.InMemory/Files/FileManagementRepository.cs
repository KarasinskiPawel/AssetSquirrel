using AssetsSquirrel.Plugins.InMemory.Files.Interfaces;
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
        private readonly string _basePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, BaseFolder);

        public FileManagementRepository()
        {

        }

        public bool IfFolderExist(int invoiceId)
        {
            var folderPath = System.IO.Path.Combine(_basePath, invoiceId.ToString());
            return System.IO.Directory.Exists(folderPath);
        }

        public bool CreateFolder(int invoiceId)
        {
            var folderPath = System.IO.Path.Combine(_basePath, invoiceId.ToString());

            if (!System.IO.Directory.Exists(folderPath))
            {
                System.IO.Directory.CreateDirectory(folderPath);
                return true;
            }

            return false;
        }

        public bool IfFilesExist(int invoiceId)
        {
            var folderPath = System.IO.Path.Combine(_basePath, invoiceId.ToString());
            return System.IO.Directory.GetFiles(folderPath).Length > 0;
        }

        public bool DeleteFiles(int invoiceId)
        {
            var folderPath = System.IO.Path.Combine(_basePath, invoiceId.ToString());
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

        public bool AddNewFile(int invoiceId, string fileName, string contentType, Stream fileStream)
        {
            var folderPath = System.IO.Path.Combine(_basePath, invoiceId.ToString());

            if (!System.IO.Directory.Exists(folderPath))
            {
                System.IO.Directory.CreateDirectory(folderPath);
            }

            if (System.IO.Directory.GetFiles(folderPath).Length > 0)
            {
                DeleteFiles(invoiceId);
            }

            var filePath = System.IO.Path.Combine(folderPath, fileName);

            using (var file = System.IO.File.Create(filePath))
            {
                fileStream.CopyTo(file);
            }
            return true;
        }
    }
}
