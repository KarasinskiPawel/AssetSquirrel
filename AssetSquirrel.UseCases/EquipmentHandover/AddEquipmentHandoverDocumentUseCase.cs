using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.EquipmentHandover.Interfaces;
using AssetSquirrel.UseCases.PluginInterfaces;

namespace AssetSquirrel.UseCases.EquipmentHandover
{
    public class AddEquipmentHandoverDocumentUseCase : IAddEquipmentHandoverDocumentUseCase
    {
        private readonly IEquipmentHandoverFileManagementRepository fileManagementRepository;
        private readonly IEditEquipmentHandoverUseCase editEquipmentHandoverUseCase;

        public AddEquipmentHandoverDocumentUseCase(
            IEquipmentHandoverFileManagementRepository fileManagementRepository,
            IEditEquipmentHandoverUseCase editEquipmentHandoverUseCase
            )
        {
            this.fileManagementRepository = fileManagementRepository;
            this.editEquipmentHandoverUseCase = editEquipmentHandoverUseCase;
        }

        public async Task<Result<EquipmentHandoverDto>> AddEquipmentHandoverDocumentAsync(EquipmentHandoverDto handover, string fileName, string contentType, Stream fileStream)
        {
            if (await fileManagementRepository.AddNewFile(handover.EquipmentHandoverId, fileName, contentType, fileStream))
            {
                handover.FilePath = System.IO.Path.Combine("Files", "EquipmentHandovers", handover.EquipmentHandoverId.ToString(), fileName);
                handover.UploadDate = DateTime.Now;
                handover.IsPosted = true;

                return await editEquipmentHandoverUseCase.UpdateEquipmentHandoverAsync(handover);
            }

            return Result<EquipmentHandoverDto>.Fail("Failed to save the equipment handover document file.");
        }
    }
}
