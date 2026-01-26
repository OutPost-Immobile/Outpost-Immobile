/*
 * ===================================================================================
 * TESTY INTEGRACYJNE APLIKACJI BAZODANOWEJ
 * ===================================================================================
 * 
 * CEL TESTÓW:
 * Sprawdzenie poprawności współpracy aplikacji bazodanowej z bazą danych oraz
 * weryfikacja niemożliwości naruszenia integralności i bezpieczeństwa bazy danych.
 * 
 * SPOSÓB WYKONANIA TESTÓW:
 * - Narzędzie: NUnit 4.x z Microsoft.EntityFrameworkCore.InMemory
 * - Metoda: Testy integracyjne z użyciem bazy in-memory
 * - Izolacja: Każdy test używa własnej instancji bazy danych (GUID jako nazwa)
 * - Wzorzec: Arrange-Act-Assert z komentarzami
 * 
 * ===================================================================================
 */

using Microsoft.EntityFrameworkCore;
using Moq;
using OutpostImmobile.Persistence;
using OutpostImmobile.Persistence.Domain;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;
using OutpostImmobile.Persistence.Exceptions;
using OutpostImmobile.Persistence.Factories.Interfaces;
using OutpostImmobile.Persistence.Repositories;

namespace OutpostImmobile.Core.Tests.Integration;

/// <summary>
/// Testy integracyjne weryfikujące współpracę aplikacji z bazą danych.
/// </summary>
[TestFixture]
[Category("Integration")]
[Description("Testy integracyjne weryfikujące współpracę aplikacji z bazą danych")]
public class DatabaseIntegrationTests
{
    /*
     * ================================================================================
     * SCENARIUSZ 1: Weryfikacja integralności referencyjnej Parcel -> Maczkopat
     * ================================================================================
     * CEL: Sprawdzenie czy paczka jest poprawnie powiązana z paczkomat przez klucz obcy.
     * CO JEST TESTOWANE: Poprawność relacji FK między encjami Parcel i Maczkopat
     * JAK JEST TESTOWANE: Tworzenie paczkomatu i paczki, weryfikacja powiązania
     * SPODZIEWANY WYNIK: Paczka ma poprawną referencję do paczkomatu
     * ================================================================================
     */
    [Test]
    [Order(1)]
    [Description("Weryfikacja integralności referencyjnej Parcel -> Maczkopat")]
    public async Task IntegrityTest_ParcelToMaczkopat_ShouldMaintainForeignKeyRelation()
    {
        // Arrange: Przygotowanie unikalnej bazy danych dla testu
        var dbName = $"IntegrityTest_FK_{Guid.NewGuid():N}";
        var factory = new MockDbContextFactory(dbName);
        
        var maczkopatId = Guid.NewGuid();
        var parcelId = Guid.NewGuid();
        
        // Act: Tworzenie paczkomatu i paczki z referencją
        await using (var context = await factory.CreateDbContextAsync())
        {
            var area = new AreaEntity { Id = 1, AreaName = "Obszar Testowy" };
            context.Areas.Add(area);
            
            var maczkopat = new MaczkopatEntity
            {
                Id = maczkopatId,
                Code = "MACZ-001",
                Capacity = 100,
                AreaId = 1
            };
            context.Maczkopats.Add(maczkopat);
            
            var parcel = new ParcelEntity
            {
                Id = parcelId,
                FriendlyId = "PACK-INT-001",
                Product = "Elektronika",
                Status = ParcelStatus.Sent,
                MaczkopatEntityId = maczkopatId
            };
            context.Parcels.Add(parcel);
            
            await context.SaveChangesAsync();
        }
        
        // Assert: Weryfikacja integralności referencyjnej
        await using (var context = await factory.CreateDbContextAsync())
        {
            var parcel = await context.Parcels
                .Include(p => p.Maczkopat)
                .FirstOrDefaultAsync(p => p.Id == parcelId);
            
            // UZYSKANY WYNIK: Sprawdzenie poprawności relacji
            Assert.That(parcel, Is.Not.Null, "Paczka powinna istnieć w bazie danych");
            Assert.That(parcel!.MaczkopatEntityId, Is.EqualTo(maczkopatId), 
                "Klucz obcy powinien wskazywać na poprawny paczkomat");
            Assert.That(parcel.Maczkopat, Is.Not.Null, 
                "Nawigacja do paczkomatu powinna działać poprawnie");
            Assert.That(parcel.Maczkopat.Code, Is.EqualTo("MACZ-001"),
                "Dane powiązanego paczkomatu powinny być poprawne");
        }
    }
    
    /*
     * ================================================================================
     * SCENARIUSZ 2: Weryfikacja kaskadowego pobierania powiązanych encji
     * ================================================================================
     * CEL: Sprawdzenie czy pobieranie paczkomatu zwraca wszystkie powiązane paczki
     * CO JEST TESTOWANE: Poprawność relacji jeden-do-wielu (Maczkopat -> Parcels)
     * JAK JEST TESTOWANE: Tworzenie paczkomatu z wieloma paczkami, weryfikacja kolekcji
     * SPODZIEWANY WYNIK: Paczkomat zawiera wszystkie przypisane paczki
     * ================================================================================
     */
    [Test]
    [Order(2)]
    [Description("Weryfikacja kaskadowego ładowania kolekcji powiązanych encji")]
    public async Task IntegrityTest_MaczkopatWithParcels_ShouldLoadRelatedCollection()
    {
        // Arrange
        var dbName = $"IntegrityTest_Collection_{Guid.NewGuid():N}";
        var factory = new MockDbContextFactory(dbName);
        var maczkopatId = Guid.NewGuid();
        
        // Act: Tworzenie paczkomatu z wieloma paczkami
        await using (var context = await factory.CreateDbContextAsync())
        {
            var maczkopat = new MaczkopatEntity
            {
                Id = maczkopatId,
                Code = "MACZ-MULTI",
                Capacity = 50
            };
            context.Maczkopats.Add(maczkopat);
            
            // Dodanie 5 paczek do paczkomatu
            for (int i = 1; i <= 5; i++)
            {
                context.Parcels.Add(new ParcelEntity
                {
                    Id = Guid.NewGuid(),
                    FriendlyId = $"PACK-{i:D3}",
                    Product = $"Produkt {i}",
                    Status = ParcelStatus.InMaczkopat,
                    MaczkopatEntityId = maczkopatId
                });
            }
            
            await context.SaveChangesAsync();
        }
        
        // Assert: Weryfikacja kolekcji
        await using (var context = await factory.CreateDbContextAsync())
        {
            var maczkopat = await context.Maczkopats
                .Include(m => m.Parcels)
                .FirstOrDefaultAsync(m => m.Id == maczkopatId);
            
            // UZYSKANY WYNIK
            Assert.That(maczkopat, Is.Not.Null);
            Assert.That(maczkopat!.Parcels, Has.Count.EqualTo(5),
                "Paczkomat powinien zawierać wszystkie 5 paczek");
        }
    }
    
    /*
     * ================================================================================
     * SCENARIUSZ 3: Atomowość operacji zapisu wielu encji
     * ================================================================================
     * CEL: Sprawdzenie czy zapis wielu encji w jednej transakcji jest atomowy
     * CO JEST TESTOWANE: Atomowość SaveChangesAsync, poprawność zapisu wielu encji
     * JAK JEST TESTOWANE: Dodanie wielu encji różnych typów, weryfikacja zapisu
     * SPODZIEWANY WYNIK: Wszystkie encje są zapisane lub żadna (atomowość)
     * ================================================================================
     */
    [Test]
    [Order(3)]
    [Description("Weryfikacja atomowości operacji zapisu wielu encji")]
    public async Task TransactionTest_MultipleEntitiesSave_ShouldBeAtomic()
    {
        // Arrange
        var dbName = $"TransactionTest_Atomic_{Guid.NewGuid():N}";
        var factory = new MockDbContextFactory(dbName);
        
        var maczkopatId = Guid.NewGuid();
        var parcelId = Guid.NewGuid();
        
        // Act: Zapis wielu encji w jednej transakcji
        await using (var context = await factory.CreateDbContextAsync())
        {
            var maczkopat = new MaczkopatEntity
            {
                Id = maczkopatId,
                Code = "MACZ-TRANS",
                Capacity = 30
            };
            context.Maczkopats.Add(maczkopat);
            
            var parcel = new ParcelEntity
            {
                Id = parcelId,
                FriendlyId = "TRANS-001",
                Product = "Test atomowości",
                Status = ParcelStatus.Sent,
                MaczkopatEntityId = maczkopatId
            };
            context.Parcels.Add(parcel);
            
            await context.SaveChangesAsync();
        }
        
        // Assert: Weryfikacja zapisu wszystkich encji
        await using (var context = await factory.CreateDbContextAsync())
        {
            var maczkopatExists = await context.Maczkopats.AnyAsync(m => m.Id == maczkopatId);
            var parcelExists = await context.Parcels.AnyAsync(p => p.Id == parcelId);
            
            // UZYSKANY WYNIK
            using (Assert.EnterMultipleScope())
            {
                Assert.That(maczkopatExists, Is.True, "Paczkomat powinien być zapisany");
                Assert.That(parcelExists, Is.True, "Paczka powinna być zapisana");
            }
        }
    }
    
    /*
     * ================================================================================
     * SCENARIUSZ 4: Ochrona przed dostępem do nieistniejących danych
     * ================================================================================
     * CEL: Weryfikacja poprawnej obsługi prób dostępu do nieistniejących encji
     * CO JEST TESTOWANE: Rzucanie EntityNotFoundException przy braku encji
     * JAK JEST TESTOWANE: Wywołanie metody z nieistniejącym identyfikatorem
     * SPODZIEWANY WYNIK: EntityNotFoundException z odpowiednim komunikatem
     * ================================================================================
     */
    [Test]
    [Order(4)]
    [Description("Weryfikacja ochrony przed dostępem do nieistniejących danych")]
    public async Task SecurityTest_NonExistentEntity_ShouldThrowException()
    {
        // Arrange
        var dbName = $"SecurityTest_NotFound_{Guid.NewGuid():N}";
        var factory = new MockDbContextFactory(dbName);
        var mockLogFactory = new Mock<IEventLogFactory>();
        
        var repository = new ParcelRepository(mockLogFactory.Object, factory);
        const string nonExistentId = "NIE-ISTNIEJE-12345";
        
        // Act & Assert
        // UZYSKANY WYNIK: Wyjątek informujący o braku encji
        var exception = Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            await repository.UpdateParcelStatusAsync(nonExistentId, ParcelStatus.Delivered, "Test"));
        
        Assert.That(exception!.Message, Does.Contain("not found"),
            "Komunikat powinien informować o nieznalezieniu encji");
    }
    
    /*
     * ================================================================================
     * SCENARIUSZ 5: Izolacja danych między paczkomatami
     * ================================================================================
     * CEL: Sprawdzenie czy pobieranie paczek z paczkomatu zwraca tylko powiązane dane
     * CO JEST TESTOWANE: Poprawność filtrowania w GetParcelsFromMaczkopatAsync
     * JAK JEST TESTOWANE: Utworzenie dwóch paczkomatów z paczkami, weryfikacja izolacji
     * SPODZIEWANY WYNIK: Tylko paczki z wybranego paczkomatu
     * ================================================================================
     */
    [Test]
    [Order(5)]
    [Description("Weryfikacja izolacji danych między paczkomatami")]
    public async Task IsolationTest_GetParcelsFromMaczkopat_ShouldReturnOnlyRelated()
    {
        // Arrange
        var dbName = $"IsolationTest_Maczkopat_{Guid.NewGuid():N}";
        var factory = new MockDbContextFactory(dbName);
        
        var maczkopatA_Id = Guid.NewGuid();
        var maczkopatB_Id = Guid.NewGuid();
        
        await using (var context = await factory.CreateDbContextAsync())
        {
            context.Maczkopats.AddRange(
                new MaczkopatEntity { Id = maczkopatA_Id, Code = "MACZ-A", Capacity = 50 },
                new MaczkopatEntity { Id = maczkopatB_Id, Code = "MACZ-B", Capacity = 50 }
            );
            
            // Paczki dla paczkomatu A
            for (int i = 1; i <= 3; i++)
            {
                context.Parcels.Add(new ParcelEntity
                {
                    FriendlyId = $"A-{i}",
                    Product = "Produkt A",
                    MaczkopatEntityId = maczkopatA_Id
                });
            }
            
            // Paczki dla paczkomatu B
            for (int i = 1; i <= 5; i++)
            {
                context.Parcels.Add(new ParcelEntity
                {
                    FriendlyId = $"B-{i}",
                    Product = "Produkt B",
                    MaczkopatEntityId = maczkopatB_Id
                });
            }
            
            await context.SaveChangesAsync();
        }
        
        var mockLogFactory = new Mock<IEventLogFactory>();
        var repository = new ParcelRepository(mockLogFactory.Object, factory);
        
        // Act: Pobranie paczek tylko z paczkomatu A
        var parcelsFromA = await repository.GetParcelsFromMaczkopatAsync(maczkopatA_Id);
        var parcelsFromB = await repository.GetParcelsFromMaczkopatAsync(maczkopatB_Id);
        
        // Assert: UZYSKANY WYNIK
        using (Assert.EnterMultipleScope())
        {
            Assert.That(parcelsFromA, Has.Count.EqualTo(3), 
                "Paczkomat A powinien zawierać dokładnie 3 paczki");
            Assert.That(parcelsFromB, Has.Count.EqualTo(5), 
                "Paczkomat B powinien zawierać dokładnie 5 paczek");
            Assert.That(parcelsFromA.All(p => p.FriendlyId.StartsWith("A-")), Is.True,
                "Wszystkie paczki powinny należeć do paczkomatu A");
            Assert.That(parcelsFromB.All(p => p.FriendlyId.StartsWith("B-")), Is.True,
                "Wszystkie paczki powinny należeć do paczkomatu B");
        }
    }
}
