using OutpostImmobile.Api.Controllers;

namespace OutpostImmobile.Api.Extensions;

public static class EndpointRouteBuilderExtensions
{
    /// <summary>
    /// Jak zrobisz jakikolwiek nowy kontroller to musisz go tutaj domapowac
    /// </summary>
    /// <param name="routes"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapRoutes(this IEndpointRouteBuilder routes)
    {
        routes.MapInfrastructure();
        routes.MapMaczkopats();
        routes.MapParcels();
        routes.MapRoute();
        
        return routes;
    }
}