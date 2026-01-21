using Microsoft.EntityFrameworkCore;
using Moq;
using OutpostImmobile.Persistence.Domain.Logs;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;
using OutpostImmobile.Persistence.Exceptions;
using OutpostImmobile.Persistence.Factories.Interfaces;
using OutpostImmobile.Persistence.Factories.Request;
using OutpostImmobile.Persistence.Repositories;

namespace OutpostImmobile.Core.Tests.Repositories;

[TestFixture]
public class MaczkopatRepositoryTests
{
    private MockDbContextFactory _dbFactory;
    private Mock<IEventLogFactory> _eventLogFactoryMock;
    private MaczkopatRepository _repository;
    private CancellationToken _ct;

    [SetUp]
    public void SetUp()
    {
        _dbFactory = new MockDbContextFactory($"TestDb_{Guid.NewGuid()}");
        _eventLogFactoryMock = new Mock<IEventLogFactory>();
        _repository = new MaczkopatRepository(
            _eventLogFactoryMock.Object, 
            _dbFactory
        );

        _ct = CancellationToken.None;
    }

    [TearDown]
    public async Task TearDown()
    {
        await using var context = await _dbFactory.CreateDbContextAsync(_ct);
        await context.Database.EnsureDeletedAsync(_ct);
    }

    [Test]
    public async Task AddLogAsync_Should_Add_Entity_To_Database_When_Type_Is_Correct()
    {
        // Arrange
        var maczkopatId = Guid.NewGuid();
        var logType = MaczkopatEventLogType.LockerOpened; 

        var expectedEntity = new MaczkopatEventLogEntity
        {
            Id = Guid.NewGuid(),
            MaczkopatId = maczkopatId,
            Message = "Test Message",
            EventLogType = MaczkopatEventLogType.LockerOpened
        };

        _eventLogFactoryMock
            .Setup(x => x.CreateEventLogAsync(It.IsAny<CreateMaczkopatEventLogRequest>(), _ct))
            .ReturnsAsync(expectedEntity);

        // Act
        await _repository.AddLogAsync(maczkopatId, logType, _ct);

        // Assert
        await using var verifyContext = await _dbFactory.CreateDbContextAsync(_ct);
        
        var count = await verifyContext.Set<MaczkopatEventLogEntity>().CountAsync(cancellationToken: _ct);
        var storedLog = await verifyContext.Set<MaczkopatEventLogEntity>().FirstOrDefaultAsync(cancellationToken: _ct);
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(count, Is.EqualTo(1), "Database should contain exactly one log entry.");
            Assert.That(storedLog, Is.Not.Null, "The stored log should not be null.");
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(storedLog!.Id, Is.EqualTo(expectedEntity.Id), "IDs should match.");
            Assert.That(storedLog.MaczkopatId, Is.EqualTo(maczkopatId), "Maczkopat IDs should match.");
        }
    }

    [Test]
    public async Task AddLogAsync_Should_Throw_Exception_When_Factory_Returns_Wrong_Type()
    {
        // Arrange
        var maczkopatId = Guid.NewGuid();
        var wrongEntity = new EventLogBase
        {
            Message = string.Empty
        };
        
        _eventLogFactoryMock
            .Setup(x => x.CreateEventLogAsync(It.IsAny<CreateMaczkopatEventLogRequest>(), _ct))
            .ReturnsAsync(wrongEntity);

        // Act & Assert
        var exception = Assert.ThrowsAsync<WrongEventLogTypeException>(async () => 
        {
            await _repository.AddLogAsync(maczkopatId, MaczkopatEventLogType.LockerOpened, _ct);
        });
        
        Assert.That(exception!.Message, Is.EqualTo("Created log is not of MaczkopatEventLog"));
    }
}