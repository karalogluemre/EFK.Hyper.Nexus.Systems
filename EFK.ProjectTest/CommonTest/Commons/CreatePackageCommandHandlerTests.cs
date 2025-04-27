using AutoMapper;
using Commons.Application.Features.Commands.Package.Create;
using Commons.Application.Repositories.Common;
using Commons.Domain.Models;
using Commons.Domain.Models.Packages;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

public class CreatePackageCommandHandlerTests
{
    private readonly Mock<IUnitOfWork<DbContext>> unitOfWorkMock;
    private readonly Mock<IMapper> mapperMock;
    private readonly CreatePackageCommandHandler<DbContext> handler;

    public CreatePackageCommandHandlerTests()
    {
        unitOfWorkMock = new Mock<IUnitOfWork<DbContext>>();
        mapperMock = new Mock<IMapper>();

        handler = new CreatePackageCommandHandler<DbContext>(
            unitOfWorkMock.Object,
            mapperMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldCreatePackageSuccessfully()
    {
        // Arrange
        var request = new CreatePackageCommandRequest
        {
            Name = "Premium"
        };

        var mappedPackage = new Package {Id=Guid.NewGuid(), Name = "Premium" };

        this.mapperMock.Setup(m => m.Map<Package>(It.IsAny<CreatePackageCommandRequest>()))
            .Returns(mappedPackage);

        this.unitOfWorkMock.Setup(uow => uow.GetWriteRepository<Package>().AddOrUpdateBulkAsync(It.IsAny<List<Package>>()))
            .ReturnsAsync(new BaseResponse { Succeeded = true });

        this.unitOfWorkMock.Setup(uow => uow.GetWriteRepository<Package>().AddOrUpdateBulkAsync(It.IsAny<List<Package>>()))
                .ReturnsAsync(new BaseResponse
                {
                    Succeeded = true,
                    Message = "1 kayıt başarıyla eklendi." 
                });

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Message.Should().Be("1 kayıt başarıyla eklendi.");
    }
}