using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.PluginInterfaces;
using AssetSquirrel.UseCases.Suppilers;
using Moq;

namespace AssetSquirrel.UseCases.Tests.Suppilers
{
    public class EditSuppilerUseCaseTests
    {
        [Fact]
        public async Task UpdateSuppilerAsync_MapsDtoToEntity_AndDelegatesToUpdateSuppilerAsync()
        {
            var repository = new Mock<ISuppilersRepository>();
            Suppiler? captured = null;
            repository
                .Setup(r => r.UpdateSuppilerAsync(It.IsAny<Suppiler>()))
                .Callback<Suppiler>(s => captured = s)
                .ReturnsAsync((Suppiler s) => Result<Suppiler>.Ok(s));

            var useCase = new EditSuppilerUseCase(repository.Object);
            var dto = new SuppilerDto
            {
                SuppilerId = 3,
                Name = "Lantre",
                Description = "Office supplies vendor",
                IsActive = false
            };

            var result = await useCase.UpdateSuppilerAsync(dto);

            Assert.True(result.Success);
            Assert.NotNull(captured);
            Assert.Equal(dto.SuppilerId, captured.SuppilerId);
            Assert.Equal(dto.Description, captured.Description);
            Assert.Equal(dto.IsActive, captured.IsActive);
            repository.Verify(r => r.UpdateSuppilerAsync(It.IsAny<Suppiler>()), Times.Once);
        }

        [Fact]
        public async Task UpdateSuppilerAsync_ReturnsFailureWithMessage_WhenRepositoryReportsFailure()
        {
            var repository = new Mock<ISuppilersRepository>();
            repository
                .Setup(r => r.UpdateSuppilerAsync(It.IsAny<Suppiler>()))
                .ReturnsAsync(Result<Suppiler>.Fail("Suppiler not found."));

            var useCase = new EditSuppilerUseCase(repository.Object);
            var dto = new SuppilerDto { SuppilerId = 3, Name = "Lantre" };

            var result = await useCase.UpdateSuppilerAsync(dto);

            Assert.False(result.Success);
            Assert.Equal("Suppiler not found.", result.Message);
        }
    }
}
