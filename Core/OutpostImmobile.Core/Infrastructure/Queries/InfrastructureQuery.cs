using OutpostImmobile.Core.Infrastructure.QueryResults;
using OutpostImmobile.Core.Mediator.Abstraction;

namespace OutpostImmobile.Core.Infrastructure.Queries;

/// <summary>
/// Przykladowo tutaj jest wersja z Value taskiem bo jest szybsza, ale normalnie robimy IRequest<T, Task<T>>
/// </summary>
public record InfrastructureQuery : IRequest<InfrastructureQuery, PingDto>;

internal class InfrastructureQueryHandler : IRequestHandler<InfrastructureQuery, PingDto>
{
    /// <summary>
    /// Normalnie tutaj wszystko lecimy na async Task ale no nie mamy jeszcze bazy zeby to ogarnac
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<PingDto> Handle(InfrastructureQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new PingDto
        {
            Message = "Pong"
        });
    }
}