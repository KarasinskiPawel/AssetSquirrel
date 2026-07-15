using System.Threading;
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

        [Fact]
        public async Task GetEquipmentAssignmentOverviewCountAsync_PassesFilterThrough_AndReturnsRepositoryResult()
        {
            EquipmentAssignmentFilter? capturedFilter = null;
            var repository = new Mock<IEquipmentAssignmentRepository>();
            repository
                .Setup(r => r.GetEquipmentAssignmentOverviewCountAsync(It.IsAny<EquipmentAssignmentFilter>(), It.IsAny<CancellationToken>()))
                .Callback<EquipmentAssignmentFilter, CancellationToken>((f, _) => capturedFilter = f)
                .ReturnsAsync(42);

            var useCase = new ViewEquipmentAssignmentUseCase(repository.Object);
            var filter = new EquipmentAssignmentFilter { IsActive = true, EmployeeId = 7 };

            var result = await useCase.GetEquipmentAssignmentOverviewCountAsync(filter);

            Assert.Equal(42, result);
            Assert.Same(filter, capturedFilter);
        }

        [Fact]
        public async Task GetEquipmentAssignmentOverviewPageAsync_PassesArgumentsThrough_AndReturnsRepositoryResult()
        {
            var page = new List<EquipmentAssignmentOverviewDto>
            {
                new() { EquipmentId = 1, ModelName = "ThinkPad T14", SerialNumber = "SN-1", AssignedEmployeeId = 7, AssignedEmployeeName = "Jan Kowalski" }
            };

            EquipmentAssignmentFilter? capturedFilter = null;
            int? capturedStartIndex = null;
            int? capturedCount = null;
            string? capturedSortColumn = null;
            bool? capturedSortDescending = null;

            var repository = new Mock<IEquipmentAssignmentRepository>();
            repository
                .Setup(r => r.GetEquipmentAssignmentOverviewPageAsync(
                    It.IsAny<EquipmentAssignmentFilter>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<string?>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .Callback<EquipmentAssignmentFilter, int, int, string?, bool, CancellationToken>(
                    (filterArg, startIndex, count, sortColumn, sortDescending, _) =>
                    {
                        capturedFilter = filterArg;
                        capturedStartIndex = startIndex;
                        capturedCount = count;
                        capturedSortColumn = sortColumn;
                        capturedSortDescending = sortDescending;
                    })
                .ReturnsAsync(page);

            var useCase = new ViewEquipmentAssignmentUseCase(repository.Object);
            var filter = new EquipmentAssignmentFilter { IsActive = true, EmployeeId = 7 };

            var result = await useCase.GetEquipmentAssignmentOverviewPageAsync(filter, 200, 200, "AssignedLocationName", true);

            Assert.Same(page, result);
            Assert.Same(filter, capturedFilter);
            Assert.Equal(200, capturedStartIndex);
            Assert.Equal(200, capturedCount);
            Assert.Equal("AssignedLocationName", capturedSortColumn);
            Assert.True(capturedSortDescending);
        }
    }
}
