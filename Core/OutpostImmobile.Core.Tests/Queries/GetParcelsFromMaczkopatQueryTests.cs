using System.Reflection;
using Moq;
using NUnit.Framework;
using OutpostImmobile.Core.Common.Helpers;
using OutpostImmobile.Core.Parcels.Queries;
using OutpostImmobile.Core.Parcels.QueryResults;
using OutpostImmobile.Persistence.Domain;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;
using OutpostImmobile.Persistence.Enums;
using OutpostImmobile.Persistence.Interfaces;

namespace OutpostImmobile.Core.Tests.Queries;

[TestFixture]
[TestOf(typeof(GetParcelsFromMaczkopatQuery))]
public class GetParcelsFromMaczkopatQueryTests
{
    private static object CreateHandler(
        IParcelRepository repository,
        IStaticEnumHelper staticEnumHelper)
    {
        var handlerType = typeof(GetParcelsFromMaczkopatQuery).Assembly.GetType(
            "OutpostImmobile.Core.Parcels.Queries.GetParcelsFromMaczkopatQueryHandler",
            throwOnError: true);

        return Activator.CreateInstance(handlerType!, repository, staticEnumHelper)!;
    }

    private static Task<List<ParcelDto>> InvokeHandle(
        object handler,
        GetParcelsFromMaczkopatQuery query,
        CancellationToken ct)
    {
        var method = handler.GetType().GetMethod(
            "Handle",
            BindingFlags.Instance | BindingFlags.Public);

        Assert.That(method, Is.Not.Null, "Handle method not found on handler.");

        return (Task<List<ParcelDto>>)method!.Invoke(handler, new object[] { query, ct })!;
    }

    [Test]
    [Order(1)]
    [TestCase(
        ParcelStatus.InWarehouse, "W magazynie",
        ParcelStatus.InMaczkopat, "Umieszczono w paczkomacie")]
    public async Task Handle_MapsParcelStatus_ToTranslatedValue_WhenParcelsExist(
        ParcelStatus firstStatus,
        string firstTranslation,
        ParcelStatus secondStatus,
        string secondTranslation)
    {
        // jeśli
        var maczkopatId = Guid.NewGuid();

        var parcels = new List<ParcelEntity>
        {
            new() { FriendlyId = "P1", Status = firstStatus, Product = "X" },
            new() { FriendlyId = "P2", Status = secondStatus, Product = "Y" }
        };

        var translations = new Dictionary<int, string>
        {
            [(int)firstStatus] = firstTranslation,
            [(int)secondStatus] = secondTranslation
        };

        var repository = new Mock<IParcelRepository>();
        repository
            .Setup(r => r.GetParcelsFromMaczkopatAsync(maczkopatId))
            .ReturnsAsync(parcels);

        var enumHelper = new Mock<IStaticEnumHelper>();
        enumHelper
            .Setup(e => e.GetStaticEnumTranslations(nameof(ParcelStatus), TranslationLanguage.Pl))
            .ReturnsAsync(translations);

        var handler = CreateHandler(repository.Object, enumHelper.Object);

        // gdy
        var result = await InvokeHandle(
            handler,
            new GetParcelsFromMaczkopatQuery { MaczkopatId = maczkopatId },
            CancellationToken.None);

        // wtedy
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0].Status, Is.EqualTo(firstTranslation));
        Assert.That(result[1].Status, Is.EqualTo(secondTranslation));
    }

    [Test]
    [Order(2)]
    public async Task Handle_SetsDtoStatusToNull_WhenParcelStatusIsNull()
    {
        // jeśli
        var maczkopatId = Guid.NewGuid();

        var parcels = new List<ParcelEntity>
        {
            new() { FriendlyId = "P-NULL", Status = null, Product = "X" }
        };

        var repository = new Mock<IParcelRepository>();
        repository
            .Setup(r => r.GetParcelsFromMaczkopatAsync(maczkopatId))
            .ReturnsAsync(parcels);

        var enumHelper = new Mock<IStaticEnumHelper>();
        enumHelper
            .Setup(e => e.GetStaticEnumTranslations(nameof(ParcelStatus), TranslationLanguage.Pl))
            .ReturnsAsync(new Dictionary<int, string>());

        var handler = CreateHandler(repository.Object, enumHelper.Object);

        // gdy
        var result = await InvokeHandle(
            handler,
            new GetParcelsFromMaczkopatQuery { MaczkopatId = maczkopatId },
            CancellationToken.None);

        // wtedy
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].FriendlyId, Is.EqualTo("P-NULL"));
        Assert.That(result[0].Status, Is.Null);
    }

    [Test]
    [Order(3)]
    public async Task Handle_CallsRepositoryAndStaticEnumHelper_WithExpectedParameters()
    {
        // jeśli
        var maczkopatId = Guid.NewGuid();

        var repository = new Mock<IParcelRepository>(MockBehavior.Strict);
        repository
            .Setup(r => r.GetParcelsFromMaczkopatAsync(maczkopatId))
            .ReturnsAsync(new List<ParcelEntity>());

        var enumHelper = new Mock<IStaticEnumHelper>(MockBehavior.Strict);
        enumHelper
            .Setup(e => e.GetStaticEnumTranslations(nameof(ParcelStatus), TranslationLanguage.Pl))
            .ReturnsAsync(new Dictionary<int, string>());

        var handler = CreateHandler(repository.Object, enumHelper.Object);

        // gdy
        var result = await InvokeHandle(
            handler,
            new GetParcelsFromMaczkopatQuery { MaczkopatId = maczkopatId },
            CancellationToken.None);

        // wtedy
        Assert.That(result, Is.Empty);

        repository.Verify(
            r => r.GetParcelsFromMaczkopatAsync(maczkopatId),
            Times.Once);

        enumHelper.Verify(
            e => e.GetStaticEnumTranslations(nameof(ParcelStatus), TranslationLanguage.Pl),
            Times.Once);
    }
}
