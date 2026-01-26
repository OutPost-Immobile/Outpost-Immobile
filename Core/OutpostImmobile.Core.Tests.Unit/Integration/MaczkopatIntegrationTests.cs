/*
 * ===================================================================================
 * TESTY INTEGRACYJNE OPERACJI NA PACZKOMATACH (MACZKOPAT)
 * ===================================================================================
 * 
 * CEL TESTÓW:
 * Weryfikacja poprawności operacji na encjach Maczkopat oraz ich interakcji z bazą
 * danych, w tym logowania zdarzeń i zarządzania stanami.
 * 
 * SPOSÓB WYKONANIA TESTÓW:
 * - Narzędzie: NUnit 4.x z Microsoft.EntityFrameworkCore.InMemory + Moq
 * - Metoda: Testy integracyjne z mockowaniem zewnętrznych zależności
 * - Izolacja: Każdy test używa własnej instancji bazy danych
 * 
 * ===================================================================================
 */

using Microsoft.EntityFrameworkCore;
using Moq;
using OutpostImmobile.Persistence;
using OutpostImmobile.Persistence.Domain;
using OutpostImmobile.Persistence.Domain.Logs;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;
using OutpostImmobile.Persistence.Factories.Interfaces;
using OutpostImmobile.Persistence.Factories.Request;
using OutpostImmobile.Persistence.Repositories;

namespace OutpostImmobile.Core.Tests.Integration;

/// <summary>
/// Testy integracyjne operacji na paczkomatach i logowaniu zdarzeń.
/// </summary>
[TestFixture]
[Category("Integration")]
[Description("Testy integracyjne operacji na paczkomatach (Maczkopat)")]
public class MaczkopatIntegrationTests
{
    /*
     * ================================================================================
     * SCENARIUSZ 1: Logowanie zdarzenia otwarcia skrytki paczkomatu
     * ================================================================================
     * CEL: Weryfikacja poprawnego logowania zdarzenia otwarcia skrytki
     * CO JEST TESTOWANE: Metoda AddLogAsync z typem LockerOpened
     * JAK JEST TESTOWANE: Wywołanie metody z mockiem IEventLogFactory i weryfikacja zapisu
     * SPODZIEWANY WYNIK: Nowy rekord logu w tabeli MaczkopatEventLogs
     * ================================================================================
     */
    [Test]
    [Order(1)]
    [Description("Logowanie zdarzenia otwarcia skrytki paczkomatu")]
    public async Task AddLogAsync_LockerOpened_ShouldCreateLogEntry()
    {
        // Arrange
        var dbName = $"MaczkopatLog_Opened_{Guid.NewGuid():N}";
        var factory = new MockDbContextFactory(dbName);
        var maczkopatId = Guid.NewGuid();
        var ct = CancellationToken.None;
        
        var expectedEntity = new MaczkopatEventLogEntity
        {
            Id = Guid.NewGuid(),
            MaczkopatId = maczkopatId,
            Message = "Skrytka otwarta - test integracyjny",
            EventLogType = MaczkopatEventLogType.LockerOpened
        };
        
        var mockLogFactory = new Mock<IEventLogFactory>();
        mockLogFactory
            .Setup(x => x.CreateEventLogAsync(It.IsAny<CreateMaczkopatEventLogRequest>(), ct))
            .ReturnsAsync(expectedEntity);
        
        var repository = new MaczkopatRepository(mockLogFactory.Object, factory);
        
        // Act: Dodanie logu
        await repository.AddLogAsync(maczkopatId, MaczkopatEventLogType.LockerOpened, ct);
        
        // Assert: Weryfikacja zapisu w bazie
        await using (var context = await factory.CreateDbContextAsync())
        {
            var savedLog = await context.MaczkopatEventLogs.FirstOrDefaultAsync();
            
            // UZYSKANY WYNIK
            Assert.That(savedLog, Is.Not.Null, "Log powinien być zapisany w bazie");
            Assert.That(savedLog!.EventLogType, Is.EqualTo(MaczkopatEventLogType.LockerOpened),
                "Typ zdarzenia powinien być LockerOpened");
            Assert.That(savedLog.Message, Does.Contain("test integracyjny"),
                "Opis powinien zawierać informację testową");
        }
    }
    
    /*
     * ================================================================================
     * SCENARIUSZ 2: Logowanie zdarzenia zamknięcia skrytki
     * ================================================================================
     * CEL: Weryfikacja poprawnego logowania zdarzenia zamknięcia skrytki
     * CO JEST TESTOWANE: Metoda AddLogAsync z typem LockerClosed
     * JAK JEST TESTOWANE: Wywołanie metody i weryfikacja zapisu logu
     * SPODZIEWANY WYNIK: Poprawny zapis zdarzenia zamknięcia
     * ================================================================================
     */
    [Test]
    [Order(2)]
    [Description("Logowanie zdarzenia zamknięcia skrytki paczkomatu")]
    public async Task AddLogAsync_LockerClosed_ShouldCreateLogEntry()
    {
        // Arrange
        var dbName = $"MaczkopatLog_Closed_{Guid.NewGuid():N}";
        var factory = new MockDbContextFactory(dbName);
        var maczkopatId = Guid.NewGuid();
        var ct = CancellationToken.None;
        
        var expectedEntity = new MaczkopatEventLogEntity
        {
            Id = Guid.NewGuid(),
            MaczkopatId = maczkopatId,
            Message = "Skrytka zamknięta po odbiorze",
            EventLogType = MaczkopatEventLogType.LockerClosed
        };
        
        var mockLogFactory = new Mock<IEventLogFactory>();
        mockLogFactory
            .Setup(x => x.CreateEventLogAsync(It.IsAny<CreateMaczkopatEventLogRequest>(), ct))
            .ReturnsAsync(expectedEntity);
        
        var repository = new MaczkopatRepository(mockLogFactory.Object, factory);
        
        // Act
        await repository.AddLogAsync(maczkopatId, MaczkopatEventLogType.LockerClosed, ct);
        
        // Assert: UZYSKANY WYNIK
        await using (var context = await factory.CreateDbContextAsync())
        {
            var logs = await context.MaczkopatEventLogs.ToListAsync();
            
            Assert.That(logs, Has.Count.EqualTo(1));
            Assert.That(logs[0].EventLogType, Is.EqualTo(MaczkopatEventLogType.LockerClosed));
        }
    }
    
    /*
     * ================================================================================
     * SCENARIUSZ 3: Logowanie błędu paczkomatu
     * ================================================================================
     * CEL: Weryfikacja poprawnego logowania błędu paczkomatu
     * CO JEST TESTOWANE: Metoda AddLogAsync z typem Error
     * JAK JEST TESTOWANE: Utworzenie logu błędu i weryfikacja zapisu
     * SPODZIEWANY WYNIK: Poprawne zapisanie informacji o błędzie
     * ================================================================================
     */
    [Test]
    [Order(3)]
    [Description("Logowanie błędu paczkomatu")]
    public async Task AddLogAsync_Error_ShouldCreateErrorEntry()
    {
        // Arrange
        var dbName = $"MaczkopatLog_Error_{Guid.NewGuid():N}";
        var factory = new MockDbContextFactory(dbName);
        var maczkopatId = Guid.NewGuid();
        var ct = CancellationToken.None;
        
        var errorLog = new MaczkopatEventLogEntity
        {
            Id = Guid.NewGuid(),
            MaczkopatId = maczkopatId,
            Message = "Błąd czytnika kodów kreskowych",
            EventLogType = MaczkopatEventLogType.Error
        };
        
        var mockLogFactory = new Mock<IEventLogFactory>();
        mockLogFactory
            .Setup(x => x.CreateEventLogAsync(It.IsAny<CreateMaczkopatEventLogRequest>(), ct))
            .ReturnsAsync(errorLog);
        
        var repository = new MaczkopatRepository(mockLogFactory.Object, factory);
        
        // Act
        await repository.AddLogAsync(maczkopatId, MaczkopatEventLogType.Error, ct);
        
        // Assert: UZYSKANY WYNIK
        await using (var context = await factory.CreateDbContextAsync())
        {
            var savedLog = await context.MaczkopatEventLogs
                .FirstOrDefaultAsync(l => l.EventLogType == MaczkopatEventLogType.Error);
            
            Assert.That(savedLog, Is.Not.Null);
            Assert.That(savedLog!.Message, Does.Contain("czytnika"));
        }
    }
    
    /*
     * ================================================================================
     * SCENARIUSZ 4: Weryfikacja pojemności paczkomatu
     * ================================================================================
     * CEL: Sprawdzenie czy system poprawnie śledzi liczbę paczek w paczkomacie
     * CO JEST TESTOWANE: Relacja Maczkopat -> Parcels i walidacja pojemności
     * JAK JEST TESTOWANE: Dodanie paczek do limitu pojemności i weryfikacja zliczania
     * SPODZIEWANY WYNIK: Poprawne zliczanie paczek względem pojemności
     * ================================================================================
     */
    [Test]
    [Order(4)]
    [Description("Weryfikacja pojemności paczkomatu")]
    public async Task CapacityCheck_FullMaczkopat_ShouldTrackParcelCount()
    {
        // Arrange
        var dbName = $"Capacity_Full_{Guid.NewGuid():N}";
        var factory = new MockDbContextFactory(dbName);
        var maczkopatId = Guid.NewGuid();
        const int capacity = 5;
        
        await using (var context = await factory.CreateDbContextAsync())
        {
            context.Maczkopats.Add(new MaczkopatEntity
            {
                Id = maczkopatId,
                Code = "FULL-001",
                Capacity = capacity
            });
            
            // Dodajemy paczki do pełnej pojemności
            for (int i = 0; i < capacity; i++)
            {
                context.Parcels.Add(new ParcelEntity
                {
                    FriendlyId = $"FULL-{i}",
                    Product = "Test",
                    Status = ParcelStatus.InMaczkopat,
                    MaczkopatEntityId = maczkopatId
                });
            }
            
            await context.SaveChangesAsync();
        }
        
        // Act: Sprawdzenie stanu pojemności
        await using (var context = await factory.CreateDbContextAsync())
        {
            var maczkopat = await context.Maczkopats
                .Include(m => m.Parcels)
                .FirstAsync(m => m.Id == maczkopatId);
            
            var parcelCount = maczkopat.Parcels.Count(p => p.Status == ParcelStatus.InMaczkopat);
            var isFull = parcelCount >= maczkopat.Capacity;
            
            // Assert: UZYSKANY WYNIK
            Assert.That(parcelCount, Is.EqualTo(capacity),
                "Liczba paczek powinna być równa pojemności");
            Assert.That(isFull, Is.True, 
                "Paczkomat powinien być pełny");
            Assert.That(maczkopat.Capacity, Is.EqualTo(capacity),
                "Pojemność paczkomatu powinna wynosić 5");
        }
    }
    
    /*
     * ================================================================================
     * SCENARIUSZ 5: Izolacja danych między logami różnych paczkomatów
     * ================================================================================
     * CEL: Weryfikacja że logi są poprawnie przypisywane do konkretnych paczkomatów
     * CO JEST TESTOWANE: Poprawność relacji MaczkopatEventLog -> Maczkopat
     * JAK JEST TESTOWANE: Dodanie logów dla różnych paczkomatów i weryfikacja izolacji
     * SPODZIEWANY WYNIK: Każdy paczkomat ma tylko swoje logi
     * ================================================================================
     */
    [Test]
    [Order(5)]
    [Description("Weryfikacja izolacji logów między paczkomatami")]
    public async Task LogIsolation_MultipleMaczkopats_ShouldHaveSeparateLogs()
    {
        // Arrange
        var dbName = $"LogIsolation_{Guid.NewGuid():N}";
        var factory = new MockDbContextFactory(dbName);
        var maczkopatA_Id = Guid.NewGuid();
        var maczkopatB_Id = Guid.NewGuid();
        var ct = CancellationToken.None;
        
        var logA = new MaczkopatEventLogEntity
        {
            Id = Guid.NewGuid(),
            MaczkopatId = maczkopatA_Id,
            Message = "Log paczkomatu A",
            EventLogType = MaczkopatEventLogType.LockerOpened
        };
        
        var logB = new MaczkopatEventLogEntity
        {
            Id = Guid.NewGuid(),
            MaczkopatId = maczkopatB_Id,
            Message = "Log paczkomatu B",
            EventLogType = MaczkopatEventLogType.LockerClosed
        };
        
        var mockLogFactoryA = new Mock<IEventLogFactory>();
        mockLogFactoryA
            .Setup(x => x.CreateEventLogAsync(It.IsAny<CreateMaczkopatEventLogRequest>(), ct))
            .ReturnsAsync(logA);
        
        var mockLogFactoryB = new Mock<IEventLogFactory>();
        mockLogFactoryB
            .Setup(x => x.CreateEventLogAsync(It.IsAny<CreateMaczkopatEventLogRequest>(), ct))
            .ReturnsAsync(logB);
        
        var repositoryA = new MaczkopatRepository(mockLogFactoryA.Object, factory);
        var repositoryB = new MaczkopatRepository(mockLogFactoryB.Object, factory);
        
        // Act: Dodanie logów dla obu paczkomatów
        await repositoryA.AddLogAsync(maczkopatA_Id, MaczkopatEventLogType.LockerOpened, ct);
        await repositoryB.AddLogAsync(maczkopatB_Id, MaczkopatEventLogType.LockerClosed, ct);
        
        // Assert: Weryfikacja izolacji
        await using (var context = await factory.CreateDbContextAsync())
        {
            var logsA = await context.MaczkopatEventLogs
                .Where(l => l.MaczkopatId == maczkopatA_Id)
                .ToListAsync();
            
            var logsB = await context.MaczkopatEventLogs
                .Where(l => l.MaczkopatId == maczkopatB_Id)
                .ToListAsync();
            
            // UZYSKANY WYNIK
            Assert.That(logsA, Has.Count.EqualTo(1), "Paczkomat A powinien mieć 1 log");
            Assert.That(logsB, Has.Count.EqualTo(1), "Paczkomat B powinien mieć 1 log");
            Assert.That(logsA[0].Message, Does.Contain("paczkomatu A"));
            Assert.That(logsB[0].Message, Does.Contain("paczkomatu B"));
        }
    }
}
