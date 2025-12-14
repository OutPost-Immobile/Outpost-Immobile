// using Microsoft.EntityFrameworkCore;
// using OutpostImmobile.Core.Common.Helpers;
// using OutpostImmobile.Persistence;
// using OutpostImmobile.Persistence.Domain;
// using OutpostImmobile.Persistence.Domain.StaticEnums;
// using OutpostImmobile.Persistence.Enums;
//
// namespace OutpostImmobile.Core.Tests.StaticEnums;
// TODO
// [TestFixture]
// public class StaticEnumHelperTests
// {
//     private sealed class TestOutpostImmobileDbContext : OutpostImmobileDbContext
//     {
//         public TestOutpostImmobileDbContext(DbContextOptions<OutpostImmobileDbContext> options) : base(options)
//         {
//         }
//
//         protected override void OnModelCreating(ModelBuilder modelBuilder)
//         {
//             base.OnModelCreating(modelBuilder);
//
//             // InMemory provider can't map NetTopologySuite Point.UserData (object).
//             modelBuilder.Entity<RouteEntity>().Ignore(x => x.Locations);
//         }
//     }
//
//     private static OutpostImmobileDbContext CreateDbContext(string dbName)
//     {
//         var options = new DbContextOptionsBuilder<OutpostImmobileDbContext>()
//             .UseInMemoryDatabase(dbName)
//             .EnableSensitiveDataLogging(false)
//             .Options;
//
//         return new TestOutpostImmobileDbContext(options);
//     }
//
//     [Test]
//     public async Task GetStaticEnumTranslations_ReturnsDictionary_ForMatchingEnumName()
//     {
//         var dbName = Guid.NewGuid().ToString("N");
//         await using var dbContext = CreateDbContext(dbName);
//
//         dbContext.StaticEnums.Add(new StaticEnumEntity
//         {
//             EnumName = "VehicleType",
//             Translations = new List<StaticEnumTranslationEntity>
//             {
//                 new()
//                 {
//                     EnumName = "VehicleType",
//                     EnumValue = 1,
//                     Translation = "Car",
//                     TranslationLanguage = TranslationLanguage.Pl,
//                     EnumEntity = null!
//                 },
//                 new()
//                 {
//                     EnumName = "VehicleType",
//                     EnumValue = 2,
//                     Translation = "Truck",
//                     TranslationLanguage = TranslationLanguage.Pl,
//                     EnumEntity = null!
//                 }
//             }
//         });
//
//         dbContext.StaticEnums.Add(new StaticEnumEntity
//         {
//             EnumName = "OtherEnum",
//             Translations = new List<StaticEnumTranslationEntity>
//             {
//                 new()
//                 {
//                     EnumName = "OtherEnum",
//                     EnumValue = 1,
//                     Translation = "ShouldNotAppear",
//                     TranslationLanguage = TranslationLanguage.Pl,
//                     EnumEntity = null!
//                 }
//             }
//         });
//
//         await dbContext.SaveChangesAsync();
//
//         var sut = new StaticEnumHelper(dbContext);
//
//         var result = await sut.GetStaticEnumTranslations("VehicleType", TranslationLanguage.Pl);
//
//         Assert.That(result, Has.Count.EqualTo(2));
//         Assert.That(result[1], Is.EqualTo("Car"));
//         Assert.That(result[2], Is.EqualTo("Truck"));
//         Assert.That(result.ContainsValue("ShouldNotAppear"), Is.False);
//     }
//
//     [Test]
//     public async Task GetStaticEnumTranslations_ReturnsEmptyDictionary_WhenEnumNameDoesNotExist()
//     {
//         var dbName = Guid.NewGuid().ToString("N");
//         await using var dbContext = CreateDbContext(dbName);
//
//         dbContext.StaticEnums.Add(new StaticEnumEntity
//         {
//             EnumName = "ExistingEnum",
//             Translations = new List<StaticEnumTranslationEntity>
//             {
//                 new()
//                 {
//                     EnumName = "ExistingEnum",
//                     EnumValue = 123,
//                     Translation = "Value",
//                     TranslationLanguage = TranslationLanguage.Pl,
//                     EnumEntity = null!
//                 }
//             }
//         });
//
//         await dbContext.SaveChangesAsync();
//
//         var sut = new StaticEnumHelper(dbContext);
//
//         var result = await sut.GetStaticEnumTranslations("MissingEnum", TranslationLanguage.Pl);
//
//         Assert.That(result, Is.Not.Null);
//         Assert.That(result, Is.Empty);
//     }
// }