using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.EquipmentAssignment;
using AssetSquirrel.UseCases.PluginInterfaces;
using Moq;

namespace AssetSquirrel.UseCases.Tests.EquipmentAssignment
{
    public class ViewEquipmentAssignmentUseCaseTests
    {
        [Fact]
        public async Task GetEquipmentAssignmentOverviewAsync_PassesFilterThrough_AndReturnsRepositoryResult()
        {
            var overview = new List<EquipmentAssignmentOverviewDto>
            {
                new() { EquipmentId = 1, ModelName = "ThinkPad T14", SerialNumber = "SN-1", AssignedEmployeeId = 7, AssignedEmployeeName = "Jan Kowalski" },
                new() { EquipmentId = 2, ModelName = "Dell OptiPlex", SerialNumber = "SN-2" }
            };

            EquipmentAssignmentFilter? capturedFilter = null;
            var repository = new Mock<IEquipmentAssignmentRepository>();
            repository
                .Setup(r => r.GetEquipmentAssignmentOverviewAsync(It.IsAny<EquipmentAssignmentFilter>()))
                .Callback<EquipmentAssignmentFilter>(f => capturedFilter = f)
                .ReturnsAsync(overview);

            var useCase = new ViewEquipmentAssignmentUseCase(repository.Object);
            var filter = new EquipmentAssignmentFilter { IsActive = true, EmployeeId = 7 };

            var result = await useCase.GetEquipmentAssignmentOverviewAsync(filter);

            Assert.Same(overview, result);
            Assert.Same(filter, capturedFilter);
            Assert.Contains(result, dto => dto.EquipmentId == 1 && dto.IsAssigned);
            Assert.Contains(result, dto => dto.EquipmentId == 2 && !dto.IsAssigned);
        }
    }
}
