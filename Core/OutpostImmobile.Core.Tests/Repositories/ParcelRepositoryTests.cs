using Microsoft.EntityFrameworkCore;
using Moq;
using OutpostImmobile.Persistence.Domain;
using OutpostImmobile.Persistence.Domain.Logs;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;
using OutpostImmobile.Persistence.Exceptions;
using OutpostImmobile.Persistence.Factories.Interfaces;
using OutpostImmobile.Persistence.Factories.Request;
using OutpostImmobile.Persistence.Repositories;
namespace OutpostImmobile.Core.Tests.Repositories;

[TestFixture]
public class ParcelRepositoryTests
{
    [TestCase("PACK-123", ParcelStatus.Sent, ParcelStatus.Delivered, "Delivered")]
    [TestCase("ABC-987", ParcelStatus.InTransit, ParcelStatus.ToReturn, "Zwrot do nadawcy")]
    [TestCase("XYZ-000", ParcelStatus.InWarehouse, ParcelStatus.InMaczkopat, "Umieszczono w paczkomacie")]
    public async Task UpdateParcelStatusAsync_UpdatesStatus_And_AddsLog_WhenParcelExists(
        string friendlyId, 
        ParcelStatus initialStatus, 
        ParcelStatus newStatus, 
        string statusMessage)
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString("N");
        var factory = new MockDbContextFactory(dbName);
        var parcelId = Guid.NewGuid();

       
        await using (var dbContext = await factory.CreateDbContextAsync())
        {
            dbContext.Parcels.Add(new ParcelEntity
            {
                Id = parcelId,
                FriendlyId = friendlyId,
                Status = initialStatus, // Uses the parametrized initial status
                Product = "Standard",
                ParcelEventLogs = new List<ParcelEventLogEntity>() 
            });
            await dbContext.SaveChangesAsync();
        }
        
        var mockLogFactory = new Mock<IEventLogFactory>(); 
        
        var expectedLog = new ParcelEventLogEntity
        {
            Message = $"Status zmieniony na: {statusMessage}",
            ParcelId = parcelId,
            ParcelEventLogType = ParcelEventLogType.StatusChange
        };
        
        mockLogFactory
            .Setup(x => x.CreateEventLogAsync(It.IsAny<CreateParcelEventLogTypeRequest>()))
            .ReturnsAsync(expectedLog);

        var sut = new ParcelRepository(mockLogFactory.Object, factory);

        // Act
        await sut.UpdateParcelStatusAsync(friendlyId, newStatus, statusMessage);

        // Assert
        await using (var dbContext = await factory.CreateDbContextAsync())
        {
            var updatedParcel = await dbContext.Parcels
                .Include(p => p.ParcelEventLogs)
                .FirstOrDefaultAsync(p => p.FriendlyId == friendlyId);

            Assert.That(updatedParcel, Is.Not.Null);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(updatedParcel!.Status, Is.EqualTo(newStatus));
                Assert.That(updatedParcel.ParcelEventLogs, Has.Count.EqualTo(1));
            }

            var log = updatedParcel.ParcelEventLogs.First();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(log.Message, Is.EqualTo($"Status zmieniony na: {statusMessage}"));
                Assert.That(log.Id, Is.Not.EqualTo(Guid.Empty));
            }
        }
    }

    [TestCase("MISSING-ID")]
    [TestCase("")]
    [TestCase(null)] // This works now because the parameter is nullable
    public async Task UpdateParcelStatusAsync_ThrowsEntityNotFound_WhenParcelDoesNotExist(string? invalidFriendlyId)
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString("N");
        var factory = new MockDbContextFactory(dbName);
        var mockLogFactory = new Mock<IEventLogFactory>();

        var sut = new ParcelRepository(mockLogFactory.Object, factory);

        // Act & Assert
        // We must suppress the warning here (!) because we are intentionally testing the null case
        Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            await sut.UpdateParcelStatusAsync(invalidFriendlyId!, ParcelStatus.Delivered, "Test"));
    }
    

    [Test]
    public async Task GetParcelsFromMaczkopatAsync_ReturnsOnlyMatchingParcels()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString("N");
        var factory = new MockDbContextFactory(dbName);
        var targetMaczkopatId = Guid.NewGuid();
        var otherMaczkopatId = Guid.NewGuid();

        await using (var dbContext = await factory.CreateDbContextAsync())
        {
            dbContext.Parcels.AddRange(
                new ParcelEntity { Id = Guid.NewGuid(), MaczkopatEntityId = targetMaczkopatId, FriendlyId = "A", Product = "" },
                new ParcelEntity { Id = Guid.NewGuid(), MaczkopatEntityId = targetMaczkopatId, FriendlyId = "B", Product = "" },
                new ParcelEntity { Id = Guid.NewGuid(), MaczkopatEntityId = otherMaczkopatId, FriendlyId = "C", Product = "" }
            );
            await dbContext.SaveChangesAsync();
        }

        var mockLogFactory = new Mock<IEventLogFactory>();
        var sut = new ParcelRepository(mockLogFactory.Object, factory);

        // Act
        var result = await sut.GetParcelsFromMaczkopatAsync(targetMaczkopatId);

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result.Select(x => x.FriendlyId), Is.EquivalentTo(new[] { "A", "B" }));
    }

    [Test]
    public async Task GetParcelEventLogsAsync_ReturnsLogs_ForFriendlyId()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString("N");
        var factory = new MockDbContextFactory(dbName);
        const string friendlyId = "LOG-TEST";
        var parcelId = Guid.NewGuid();

        await using (var dbContext = await factory.CreateDbContextAsync())
        {
            dbContext.Parcels.Add(new ParcelEntity { Id = parcelId, FriendlyId = friendlyId, Product = "" });
            dbContext.ParcelEventLogs.AddRange(
                new ParcelEventLogEntity { Id = Guid.NewGuid(), ParcelId = parcelId, Message = "Log 1", ParcelEventLogType = ParcelEventLogType.StatusChange },
                new ParcelEventLogEntity { Id = Guid.NewGuid(), ParcelId = parcelId, Message = "Log 2", ParcelEventLogType = ParcelEventLogType.StatusChange }
            );
            await dbContext.SaveChangesAsync();
        }

        var mockLogFactory = new Mock<IEventLogFactory>();
        var sut = new ParcelRepository(mockLogFactory.Object, factory);

        // Act
        var result = (await sut.GetParcelEventLogsAsync(friendlyId)).ToList();

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task GetReceiverIdsFromParcels_ReturnsTuples_ForExistingFriendlyIds()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString("N");
        var factory = new MockDbContextFactory(dbName);
        var user1 = Guid.NewGuid();

        await using (var dbContext = await factory.CreateDbContextAsync())
        {
            dbContext.Parcels.AddRange(
                new ParcelEntity { Id = Guid.NewGuid(), FriendlyId = "P1", ReceiverUserExternalId = user1, Product = "" },
                new ParcelEntity { Id = Guid.NewGuid(), FriendlyId = "P2", ReceiverUserExternalId = null, Product = "" },
                new ParcelEntity { Id = Guid.NewGuid(), FriendlyId = "P3", ReceiverUserExternalId = Guid.NewGuid(), Product = "" }
            );
            await dbContext.SaveChangesAsync();
        }

        var mockLogFactory = new Mock<IEventLogFactory>();
        var sut = new ParcelRepository(mockLogFactory.Object, factory);
        var requestIds = new List<string> { "P1", "P2" };

        // Act
        var result = (await sut.GetReceiverIdsFromParcels(requestIds)).ToList();

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
        var p1 = result.First(x => x.Item1 == "P1");
        Assert.That(p1.Item2, Is.EqualTo(user1));
        var p2 = result.First(x => x.Item1 == "P2");
        Assert.That(p2.Item2, Is.Null);
    }
}