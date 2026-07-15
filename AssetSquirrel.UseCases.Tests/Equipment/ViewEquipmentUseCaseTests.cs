using System.Linq.Expressions;
using System.Threading;
using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.EquipmentUseCase;
using AssetSquirrel.UseCases.PluginInterfaces;
using Moq;

namespace AssetSquirrel.UseCases.Tests.Equipment
{
    public class ViewEquipmentUseCaseTests
    {
        [Fact]
        public async Task GetEquipmentAsync_ReturnsPassthroughDtos()
        {
            var equipment = new List<EquipmentDto>
            {
                new() { EquipmentId = 1, ModelName = "ThinkPad T14", SerialNumber = "SN-12345" },
                new() { EquipmentId = 2, ModelName = "Dell OptiPlex", SerialNumber = "SN-54321" }
            };

            var repository = new Mock<IEquipmentRepository>();
            repository
                .Setup(r => r.GetEquipmentAsync(It.IsAny<Expression<Func<CoreBusiness.Equipment, bool>>>()))
                .ReturnsAsync(equipment);

            var useCase = new ViewEquipmentUseCase(repository.Object);

            var result = await useCase.GetEquipmentAsync(e => e.IsActive);

            Assert.Equal(2, result.Count);
            Assert.Same(equipment, result);
            Assert.Contains(result, dto => dto.EquipmentId == 1 && dto.ModelName == "ThinkPad T14" && dto.SerialNumber == "SN-12345");
            Assert.Contains(result, dto => dto.EquipmentId == 2 && dto.ModelName == "Dell OptiPlex" && dto.SerialNumber == "SN-54321");
        }

        [Fact]
        public async Task GetEquipmentCountAsync_PassesPredicateThrough_AndReturnsRepositoryResult()
        {
            var repository = new Mock<IEquipmentRepository>();
            repository
                .Setup(r => r.GetEquipmentCountAsync(It.IsAny<Expression<Func<CoreBusiness.Equipment, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(42);

            var useCase = new ViewEquipmentUseCase(repository.Object);

            var result = await useCase.GetEquipmentCountAsync(e => e.IsActive);

            Assert.Equal(42, result);
            repository.Verify(r => r.GetEquipmentCountAsync(It.IsAny<Expression<Func<CoreBusiness.Equipment, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetEquipmentPageAsync_PassesArgumentsThrough_AndReturnsRepositoryResult()
        {
            var page = new List<EquipmentDto> { new() { EquipmentId = 1, ModelName = "ThinkPad T14", SerialNumber = "SN-12345" } };

            int? capturedStartIndex = null;
            int? capturedCount = null;
            string? capturedSortColumn = null;
            bool? capturedSortDescending = null;

            var repository = new Mock<IEquipmentRepository>();
            repository
                .Setup(r => r.GetEquipmentPageAsync(
                    It.IsAny<Expression<Func<CoreBusiness.Equipment, bool>>>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<string?>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .Callback<Expression<Func<CoreBusiness.Equipment, bool>>, int, int, string?, bool, CancellationToken>(
                    (_, startIndex, count, sortColumn, sortDescending, _) =>
                    {
                        capturedStartIndex = startIndex;
                        capturedCount = count;
                        capturedSortColumn = sortColumn;
                        capturedSortDescending = sortDescending;
                    })
                .ReturnsAsync(page);

            var useCase = new ViewEquipmentUseCase(repository.Object);

            var result = await useCase.GetEquipmentPageAsync(e => e.IsActive, 200, 200, "ModelName", true);

            Assert.Same(page, result);
            Assert.Equal(200, capturedStartIndex);
            Assert.Equal(200, capturedCount);
            Assert.Equal("ModelName", capturedSortColumn);
            Assert.True(capturedSortDescending);
            repository.Verify(r => r.GetEquipmentPageAsync(
                It.IsAny<Expression<Func<CoreBusiness.Equipment, bool>>>(),
                200, 200, "ModelName", true, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteEquipmentAsync_MapsDtoToEntity_AndDelegatesToRepository()
        {
            var repository = new Mock<IEquipmentRepository>();
            CoreBusiness.Equipment? captured = null;
            repository
                .Setup(r => r.DeleteEquipmentAsync(It.IsAny<CoreBusiness.Equipment>()))
                .Callback<CoreBusiness.Equipment>(e => captured = e)
                .ReturnsAsync((CoreBusiness.Equipment e) => Result<CoreBusiness.Equipment>.Ok(e));

            var useCase = new ViewEquipmentUseCase(repository.Object);
            var dto = new EquipmentDto { EquipmentId = 5, ModelName = "ThinkPad T14", SerialNumber = "SN-12345" };

            var result = await useCase.DeleteEquipmentAsync(dto);

            Assert.True(result.Success);
            Assert.NotNull(captured);
            Assert.Equal(5, captured.EquipmentId);
            repository.Verify(r => r.DeleteEquipmentAsync(It.IsAny<CoreBusiness.Equipment>()), Times.Once);
        }

        [Fact]
        public async Task UpdateEquipmentAsync_MapsDtoToEntity_AndDelegatesToRepository()
        {
            var repository = new Mock<IEquipmentRepository>();
            CoreBusiness.Equipment? captured = null;
            repository
                .Setup(r => r.UpdateEquipmentAsync(It.IsAny<CoreBusiness.Equipment>()))
                .Callback<CoreBusiness.Equipment>(e => captured = e)
                .ReturnsAsync((CoreBusiness.Equipment e) => Result<CoreBusiness.Equipment>.Ok(e));

            var useCase = new ViewEquipmentUseCase(repository.Object);
            var dto = new EquipmentDto { EquipmentId = 7, ModelName = "Dell OptiPlex", SerialNumber = "SN-54321" };

            var result = await useCase.UpdateEquipmentAsync(dto);

            Assert.True(result.Success);
            Assert.NotNull(captured);
            Assert.Equal(7, captured.EquipmentId);
            repository.Verify(r => r.UpdateEquipmentAsync(It.Is<CoreBusiness.Equipment>(e => e.EquipmentId == 7)), Times.Once);
        }
    }
}
