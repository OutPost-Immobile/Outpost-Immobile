using NetTopologySuite.Geometries;
using OutpostImmobile.Persistence.Domain;
using OutpostImmobile.Persistence.Domain.Users;
using OutpostImmobile.Persistence.Exceptions;
using OutpostImmobile.Persistence.Repositories;

namespace OutpostImmobile.Core.Tests.Repositories;

[TestFixture]
public class RouteRepositoryTests
{
    private MockDbContextFactory _factory;
    private RouteRepository _sut;

    [SetUp]
    public void Setup()
    {
        var dbName = Guid.NewGuid().ToString("N");
        _factory = new MockDbContextFactory(dbName);
        _sut = new RouteRepository(_factory);
    }
    
    [Test]
    public async Task GetRoutesAsync_ReturnsAllRoutes()
    {
        // Arrange
        await using (var context = await _factory.CreateDbContextAsync())
        {
            context.Routes.AddRange(
                new RouteEntity
                {
                    Id = 1,
                    StartAddressId = 10,
                    EndAddressId = 20,
                    StartAddressName = "Poznan",
                    EndAddressName = "Wroclaw"
                },
                new RouteEntity
                {
                    Id = 2,
                    StartAddressId = 11,
                    EndAddressId = 21,
                    StartAddressName = "Warszawa",
                    EndAddressName = "Radom"
                }
            );
            
            await context.SaveChangesAsync();
        }

        // Act
        var result = await _sut.GetRoutesAsync();

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result.Select(x => x.Id), Is.EquivalentTo([1, 2]));
    }

    [Test]
    public async Task GetRouteFromCourierAsync_ReturnsRoute_WhenCourierHasAssignedRoute()
    {
        // Arrange
        var courierId = Guid.NewGuid();
        var routeId = 100L;

        await using (var context = await _factory.CreateDbContextAsync())
        {
            // Seed Route
            context.Routes.Add(new RouteEntity
            {
                Id = routeId,
                StartAddressName = string.Empty,
                EndAddressName = string.Empty
            });
            
            context.UsersInternal.Add(new UserInternal
            {
                Id = courierId,
                RouteId = routeId
            });
            
            await context.SaveChangesAsync();
        }

        // Act
        var result = await _sut.GetRouteFromCourierAsync(courierId);

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.First().Id, Is.EqualTo(routeId));
    }

    [Test]
    public async Task GetRouteFromCourierAsync_ThrowsNotFound_WhenCourierDoesNotExist()
    {
        // Arrange
        var missingCourierId = Guid.NewGuid();

        // Act & Assert
        var ex = Assert.ThrowsAsync<EntityNotFoundException>(async () => 
            await _sut.GetRouteFromCourierAsync(missingCourierId));
        
        Assert.That(ex.Message, Is.EqualTo("Route not found"));
    }

    [Test]
    public async Task GetRouteFromCourierAsync_ThrowsNotFound_WhenCourierHasNoRouteAssigned()
    {
        // Arrange
        var courierId = Guid.NewGuid();

        await using (var context = await _factory.CreateDbContextAsync())
        {
            context.UsersInternal.Add(new UserInternal
            {
                Id = courierId,
                RouteId = null // No route assigned
            });
            await context.SaveChangesAsync();
        }

        // Act & Assert
        var ex = Assert.ThrowsAsync<EntityNotFoundException>(async () => 
            await _sut.GetRouteFromCourierAsync(courierId));

        Assert.That(ex.Message, Is.EqualTo("Route not found"));
    }
    
    [Test]
    public async Task GetPointsFromRouteAsync_ReturnsStartAndEndPoints_WhenRouteExists()
    {
        // Arrange
        var routeId = 555L;
        var startAddrId = 10L;
        var endAddrId = 20L;
        
        // Setup NetTopologySuite Points
        var startPoint = new Point(50.0, 19.0) { SRID = 4326 };
        var endPoint = new Point(51.0, 21.0) { SRID = 4326 };

        await using (var context = await _factory.CreateDbContextAsync())
        {
            // Seed Addresses
            context.Addresses.AddRange(
                new AddressEntity
                {
                    Id = startAddrId,
                    Location = startPoint,
                    City = string.Empty,
                    PostalCode = string.Empty,
                    Street = string.Empty,
                    CountryCode = string.Empty,
                    BuildingNumber = string.Empty
                },
                new AddressEntity
                {
                    Id = endAddrId,
                    Location = endPoint,
                    City = string.Empty,
                    PostalCode = string.Empty,
                    Street = string.Empty,
                    CountryCode = string.Empty,
                    BuildingNumber = string.Empty
                }
            );

            // Seed Route
            context.Routes.Add(new RouteEntity
            {
                Id = routeId,
                StartAddressId = startAddrId,
                EndAddressId = endAddrId,
                StartAddressName = string.Empty,
                EndAddressName = string.Empty
            });

            await context.SaveChangesAsync();
        }

        // Act
        var result = await _sut.GetPointsFromRouteAsync(routeId);

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
        
        var startTuple = result.FirstOrDefault(x => x.Item1);
        Assert.That(startTuple.Item2, Is.EqualTo(startPoint));
        
        var endTuple = result.FirstOrDefault(x => !x.Item1);
        Assert.That(endTuple.Item2, Is.EqualTo(endPoint));
    }

    [Test]
    public async Task GetPointsFromRouteAsync_ThrowsNotFound_WhenRouteDoesNotExist()
    {
        // Arrange
        var missingRouteId = 999L;

        // Act & Assert
        var ex = Assert.ThrowsAsync<EntityNotFoundException>(async () => 
            await _sut.GetPointsFromRouteAsync(missingRouteId));

        Assert.That(ex.Message, Does.Contain($"Route with id {missingRouteId} does not exist"));
    }

    [Test]
    public void GetRouteGeoJsonAsync_ThrowsException_WhenMoreThanTwoPointsProvided()
    {
        // Arrange
        var p1 = new Point(0, 0);
        var p2 = new Point(1, 1);
        var p3 = new Point(2, 2);

        var points = new List<(bool, Point)>
        {
            (true, p1),
            (false, p2),
            (false, p3)
        };

        // Act & Assert
        Assert.ThrowsAsync<WrongPointsCountException>(async () =>
        {
            var enumerator = _sut.GetRouteGeoJsonAsync(points).GetAsyncEnumerator();
            await enumerator.MoveNextAsync();
        });
    }
}