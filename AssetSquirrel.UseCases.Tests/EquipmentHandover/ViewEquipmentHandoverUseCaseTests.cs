using System.Linq.Expressions;
using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.EquipmentHandover;
using AssetSquirrel.UseCases.PluginInterfaces;
using Moq;

namespace AssetSquirrel.UseCases.Tests.EquipmentHandover
{
    public class ViewEquipmentHandoverUseCaseTests
    {
        [Fact]
        public async Task GetEquipmentHandoverAsync_ReturnsMappedDtos()
        {
            var handovers = new List<AssetSquirrel.CoreBusiness.EquipmentHandover>
            {
                new()
                {
                    EquipmentHandoverId = 1,
                    HandoverDocumentNumber = "HD-001",
                    FromLocationId = 10,
                    ToLocationId = 20,
                    FromEmployeeId = 100,
                    ToEmployeeId = 200,
                    HandoverDate = new DateTime(2026, 1, 15),
                    Comment = "First handover",
                    IsPosted = true,
                    IsActive = true,
                    EquipmentHandoverDetails = new List<EquipmentHandoverDetail>
                    {
                        new()
                        {
                            EquipmentHandoverDetailId = 1000,
                            EquipmentId = 500,
                            Equipment = new AssetSquirrel.CoreBusiness.Equipment { EquipmentId = 500, ModelName = "ThinkPad T14", SerialNumber = "SN-500", InventoryNumber = "49100000010" },
                            IsActive = true
                        }
                    }
                },
                new()
                {
                    EquipmentHandoverId = 2,
                    HandoverDocumentNumber = "HD-002",
                    FromLocationId = 11,
                    ToLocationId = 21,
                    FromEmployeeId = 101,
                    ToEmployeeId = 201,
                    HandoverDate = new DateTime(2026, 2, 20),
                    Comment = "Second handover",
                    IsPosted = false,
                    IsActive = false
                }
            };

            var repository = new Mock<IEquipmentHandoverRepository>();
            repository
                .Setup(r => r.GetEquipmentHandoversAsync(It.IsAny<Expression<Func<AssetSquirrel.CoreBusiness.EquipmentHandover, bool>>>()))
                .ReturnsAsync(handovers);

            var useCase = new ViewEquipmentHandoverUseCase(repository.Object);

            var result = await useCase.GetEquipmentHandoverAsync(h => h.IsActive);

            Assert.Equal(2, result.Count);
            Assert.Contains(result, dto => dto.EquipmentHandoverId == 1 && dto.HandoverDocumentNumber == "HD-001" && dto.ToEmployeeId == 200);
            Assert.Contains(result, dto => dto.EquipmentHandoverId == 2 && dto.HandoverDocumentNumber == "HD-002" && dto.ToEmployeeId == 201);

            var detail = Assert.Single(result.Single(dto => dto.EquipmentHandoverId == 1).EquipmentHandoverDetails);
            Assert.Equal("SN-500", detail.SerialNumber);
            Assert.Equal("49100000010", detail.InventoryNumber);
        }
    }
}
