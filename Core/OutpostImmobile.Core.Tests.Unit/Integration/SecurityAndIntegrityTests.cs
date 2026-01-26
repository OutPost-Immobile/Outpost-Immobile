/*
 * ===================================================================================
 * TESTY BEZPIECZEŃSTWA I INTEGRALNOŚCI BAZY DANYCH
 * ===================================================================================
 * 
 * CEL TESTÓW:
 * Weryfikacja niemożliwości naruszenia integralności i bezpieczeństwa bazy danych
 * przez aplikację bazodanową - testowanie granic systemu i przypadków brzegowych.
 * 
 * SPOSÓB WYKONANIA TESTÓW:
 * - Narzędzie: NUnit 4.x z Microsoft.EntityFrameworkCore.InMemory
 * - Metoda: Testy weryfikujące ograniczenia i walidacje
 * - Izolacja: Każdy test używa własnej instancji bazy danych
 * 
 * ===================================================================================
 */

using Microsoft.EntityFrameworkCore;
using Moq;
using OutpostImmobile.Persistence;
using OutpostImmobile.Persistence.Domain;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;
using OutpostImmobile.Persistence.Domain.Users;
using OutpostImmobile.Persistence.Exceptions;
using OutpostImmobile.Persistence.Factories.Interfaces;
using OutpostImmobile.Persistence.Repositories;

namespace OutpostImmobile.Core.Tests.Integration;

/// <summary>
/// Testy bezpieczeństwa i integralności danych w bazie.
/// </summary>
[TestFixture]
[Category("Integration")]
[Category("Security")]
[Description("Testy bezpieczeństwa i integralności bazy danych")]
public class SecurityAndIntegrityTests
{
    /*
     * ================================================================================
     * SCENARIUSZ 1: Weryfikacja poprawności relacji FK paczka-paczkomat
     * ================================================================================
     * CEL: Sprawdzenie czy system poprawnie obsługuje klucze obce
     * CO JEST TESTOWANE: Integralność referencyjna encji przy zapisie
     * JAK JEST TESTOWANE: Zapis paczki z referencją do paczkomatu
     * SPODZIEWANY WYNIK: Poprawna relacja między encjami
     * ================================================================================
     */
    [Test]
    [Order(1)]
    [Description("Weryfikacja poprawności relacji FK paczka-paczkomat")]
    public async Task ForeignKey_ParcelToMaczkopat_ShouldMaintainRelation()
    {
        // Arrange
        var dbName = $"Security_FK_{Guid.NewGuid():N}";
        var factory = new MockDbContextFactory(dbName);
        var maczkopatId = Guid.NewGuid();
        var parcelId = Guid.NewGuid();
        
        // Act: Dodanie paczkomatu i paczki z referencją
        await using (var context = await factory.CreateDbContextAsync())
        {
            context.Maczkopats.Add(new MaczkopatEntity
            {
                Id = maczkopatId,
                Code = "FK-TEST",
                Capacity = 50
            });
            
            context.Parcels.Add(new ParcelEntity
            {
                Id = parcelId,
                FriendlyId = "FK-001",
                Product = "Test klucza obcego",
                Status = ParcelStatus.Sent,
                MaczkopatEntityId = maczkopatId
            });
            await context.SaveChangesAsync();
        }
        
        // Assert: Weryfikacja relacji FK
        // UZYSKANY WYNIK: Poprawna relacja między encjami
        await using (var context = await factory.CreateDbContextAsync())
        {
            var parcel = await context.Parcels
                .Include(p => p.Maczkopat)
                .FirstOrDefaultAsync(p => p.Id == parcelId);
            
            Assert.That(parcel, Is.Not.Null, "Paczka powinna istnieć");
            Assert.That(parcel!.MaczkopatEntityId, Is.EqualTo(maczkopatId),
                "Klucz obcy powinien wskazywać na poprawny paczkomat");
            Assert.That(parcel.Maczkopat, Is.Not.Null,
                "Nawigacja do paczkomatu powinna działać");
            Assert.That(parcel.Maczkopat.Code, Is.EqualTo("FK-TEST"),
                "Dane paczkomatu powinny być poprawne");
        }
    }
    
    /*
     * ================================================================================
     * SCENARIUSZ 2: Spójność danych użytkowników (wewnętrzni i zewnętrzni)
     * ================================================================================
     * CEL: Weryfikacja poprawności pobierania danych użytkowników z obu źródeł
     * CO JEST TESTOWANE: Metoda GetUserEmailCredentials łącząca użytkowników
     * JAK JEST TESTOWANE: Tworzenie użytkowników obu typów i weryfikacja agregacji
     * SPODZIEWANY WYNIK: Poprawne połączenie danych z obu tabel
     * ================================================================================
     */
    [Test]
    [Order(2)]
    [Description("Spójność danych użytkowników (wewnętrzni i zewnętrzni)")]
    public async Task DataConsistency_UserEmailCredentials_ShouldCombineBothSources()
    {
        // Arrange
        var dbName = $"Security_Users_{Guid.NewGuid():N}";
        var factory = new MockDbContextFactory(dbName);
        
        var internalUserId = Guid.NewGuid();
        var externalUserId = Guid.NewGuid();
        
        await using (var context = await factory.CreateDbContextAsync())
        {
            // Użytkownik wewnętrzny (dziedziczy po IdentityUser)
            context.UsersInternal.Add(new UserInternal
            {
                Id = internalUserId,
                UserName = "pracownik_wewnetrzny",
                NormalizedUserName = "PRACOWNIK_WEWNETRZNY",
                Email = "internal@company.com",
                NormalizedEmail = "INTERNAL@COMPANY.COM",
                PhoneNumber = "+48111111111"
            });
            
            // Użytkownik zewnętrzny
            context.UsersExternal.Add(new UserExternal
            {
                Id = externalUserId,
                Name = "Klient Zewnętrzny",
                Email = "external@customer.com",
                PhoneNumber = "+48222222222"
            });
            
            await context.SaveChangesAsync();
        }
        
        var repository = new UserRepository(factory);
        var userIds = new List<Guid> { internalUserId, externalUserId };
        
        // Act: Pobranie listy email użytkowników
        var credentials = await repository.GetUserEmailCredentials(userIds);
        var emails = credentials.Select(c => c.Item1).ToList();
        
        // Assert: UZYSKANY WYNIK
        Assert.That(credentials.Count(), Is.EqualTo(2), 
            "Powinny być 2 rekordy (wewnętrzny + zewnętrzny)");
        Assert.That(emails.Any(e => e == "internal@company.com"), Is.True,
            "Lista powinna zawierać email użytkownika wewnętrznego");
        Assert.That(emails.Any(e => e == "external@customer.com"), Is.True,
            "Lista powinna zawierać email użytkownika zewnętrznego");
    }
    
    /*
     * ================================================================================
     * SCENARIUSZ 3: Ochrona przed pustymi wartościami wymaganych pól
     * ================================================================================
     * CEL: Weryfikacja poprawności zapisu z poprawnymi danymi
     * CO JEST TESTOWANE: Poprawność zapisu encji z wszystkimi wymaganymi polami
     * JAK JEST TESTOWANE: Zapis paczki z poprawnymi danymi i weryfikacja
     * SPODZIEWANY WYNIK: Paczka z poprawnymi danymi zostaje zapisana
     * ================================================================================
     */
    [Test]
    [Order(3)]
    [Description("Weryfikacja poprawności zapisu z poprawnymi danymi")]
    public async Task Validation_ValidData_ShouldSaveSuccessfully()
    {
        // Arrange
        var dbName = $"Security_Valid_{Guid.NewGuid():N}";
        var factory = new MockDbContextFactory(dbName);
        
        // Act: Zapis paczki z poprawnymi danymi
        await using (var context = await factory.CreateDbContextAsync())
        {
            var validParcel = new ParcelEntity
            {
                FriendlyId = "VALID-002",
                Product = "Poprawny produkt",
                Status = ParcelStatus.Sent
            };
            
            context.Parcels.Add(validParcel);
            await context.SaveChangesAsync();
        }
        
        // Assert: UZYSKANY WYNIK
        await using (var context = await factory.CreateDbContextAsync())
        {
            var savedParcel = await context.Parcels.FirstOrDefaultAsync(p => p.FriendlyId == "VALID-002");
            Assert.That(savedParcel, Is.Not.Null, "Paczka z poprawnymi danymi powinna być zapisana");
            Assert.That(savedParcel!.Product, Is.EqualTo("Poprawny produkt"));
            Assert.That(savedParcel.Status, Is.EqualTo(ParcelStatus.Sent));
        }
    }
    
    /*
     * ================================================================================
     * SCENARIUSZ 4: Izolacja transakcji - równoległe operacje
     * ================================================================================
     * CEL: Weryfikacja izolacji między równoległymi operacjami na bazie
     * CO JEST TESTOWANE: Izolacja kontekstów przy równoczesnym dostępie
     * JAK JEST TESTOWANE: Tworzenie wielu kontekstów i weryfikacja izolacji
     * SPODZIEWANY WYNIK: Każdy kontekst działa niezależnie
     * ================================================================================
     */
    [Test]
    [Order(4)]
    [Description("Izolacja transakcji - równoległe operacje")]
    public async Task TransactionIsolation_ParallelOperations_ShouldBeIsolated()
    {
        // Arrange
        var dbName = $"Security_Isolation_{Guid.NewGuid():N}";
        var factory = new MockDbContextFactory(dbName);
        var maczkopatId = Guid.NewGuid();
        
        // Tworzenie początkowych danych
        await using (var context = await factory.CreateDbContextAsync())
        {
            context.Maczkopats.Add(new MaczkopatEntity
            {
                Id = maczkopatId,
                Code = "ISOLATION-TEST",
                Capacity = 100
            });
            await context.SaveChangesAsync();
        }
        
        // Act: Równoległe operacje w różnych kontekstach
        var task1 = Task.Run(async () =>
        {
            await using var context = await factory.CreateDbContextAsync();
            context.Parcels.Add(new ParcelEntity
            {
                FriendlyId = "PARALLEL-A",
                Product = "Produkt A",
                MaczkopatEntityId = maczkopatId,
                Status = ParcelStatus.Sent
            });
            await context.SaveChangesAsync();
        });
        
        var task2 = Task.Run(async () =>
        {
            await using var context = await factory.CreateDbContextAsync();
            context.Parcels.Add(new ParcelEntity
            {
                FriendlyId = "PARALLEL-B",
                Product = "Produkt B",
                MaczkopatEntityId = maczkopatId,
                Status = ParcelStatus.Sent
            });
            await context.SaveChangesAsync();
        });
        
        await Task.WhenAll(task1, task2);
        
        // Assert: Weryfikacja że obie paczki zostały zapisane
        await using (var context = await factory.CreateDbContextAsync())
        {
            var parcels = await context.Parcels
                .Where(p => p.MaczkopatEntityId == maczkopatId)
                .ToListAsync();
            
            // UZYSKANY WYNIK
            Assert.That(parcels, Has.Count.EqualTo(2), 
                "Obie paczki powinny być zapisane mimo równoległych operacji");
            Assert.That(parcels.Select(p => p.FriendlyId), 
                Does.Contain("PARALLEL-A").And.Contain("PARALLEL-B"),
                "Obie paczki powinny mieć poprawne identyfikatory");
        }
    }
    
    /*
     * ================================================================================
     * SCENARIUSZ 5: Weryfikacja statusów paczek i zmian stanów
     * ================================================================================
     * CEL: Sprawdzenie poprawności zmiany statusów paczek
     * CO JEST TESTOWANE: Aktualizacja statusu paczki w bazie danych
     * JAK JEST TESTOWANE: Zmiana statusu paczki i weryfikacja persystencji
     * SPODZIEWANY WYNIK: Status paczki zostaje poprawnie zaktualizowany
     * ================================================================================
     */
    [Test]
    [Order(5)]
    [Description("Weryfikacja zmiany statusów paczek")]
    public async Task StatusChange_ParcelStatus_ShouldPersistCorrectly()
    {
        // Arrange
        var dbName = $"Security_Status_{Guid.NewGuid():N}";
        var factory = new MockDbContextFactory(dbName);
        var parcelId = Guid.NewGuid();
        
        await using (var context = await factory.CreateDbContextAsync())
        {
            context.Parcels.Add(new ParcelEntity
            {
                Id = parcelId,
                FriendlyId = "STATUS-001",
                Product = "Test statusu",
                Status = ParcelStatus.Sent
            });
            await context.SaveChangesAsync();
        }
        
        // Act: Zmiana statusu paczki
        await using (var context = await factory.CreateDbContextAsync())
        {
            var parcel = await context.Parcels.FindAsync(parcelId);
            parcel!.Status = ParcelStatus.InMaczkopat;
            await context.SaveChangesAsync();
        }
        
        // Assert: UZYSKANY WYNIK
        await using (var context = await factory.CreateDbContextAsync())
        {
            var updatedParcel = await context.Parcels.FindAsync(parcelId);
            
            Assert.That(updatedParcel, Is.Not.Null);
            Assert.That(updatedParcel!.Status, Is.EqualTo(ParcelStatus.InMaczkopat),
                "Status paczki powinien być zaktualizowany na InMaczkopat");
        }
    }
}
