using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.EquipmentReturn.Interfaces;
using AssetSquirrel.UseCases.PluginInterfaces;

namespace AssetSquirrel.UseCases.EquipmentReturn
{
    public class AddEquipmentReturnDocumentUseCase : IAddEquipmentReturnDocumentUseCase
    {
        private readonly IEquipmentReturnFileManagementRepository fileManagementRepository;
        private readonly IEditEquipmentReturnUseCase editEquipmentReturnUseCase;

        public AddEquipmentReturnDocumentUseCase(
            IEquipmentReturnFileManagementRepository fileManagementRepository,
            IEditEquipmentReturnUseCase editEquipmentReturnUseCase
            )
        {
            this.fileManagementRepository = fileManagementRepository;
            this.editEquipmentReturnUseCase = editEquipmentReturnUseCase;
        }

        public async Task<Result<EquipmentReturnDto>> AddEquipmentReturnDocumentAsync(EquipmentReturnDto equipmentReturn, string fileName, string contentType, Stream fileStream)
        {
            var fileResult = await fileManagementRepository.AddNewFile(equipmentReturn.EquipmentReturnId, fileName, contentType, fileStream);

            if (fileResult.Success)
            {
                equipmentReturn.FilePath = System.IO.Path.Combine("Files", "EquipmentReturns", equipmentReturn.EquipmentReturnId.ToString(), fileName);
                equipmentReturn.UploadDate = DateTime.Now;

                return await editEquipmentReturnUseCase.UpdateEquipmentReturnAsync(equipmentReturn);
            }

            return Result<EquipmentReturnDto>.Fail(fileResult.Message ?? "Failed to save the equipment return document file.");
        }
    }
}
