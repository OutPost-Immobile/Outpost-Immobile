using System.Net.NetworkInformation;
using DispatchR.Abstractions.Send;
using OutpostImmobile.Core.Infrastructure.QueryResults;

namespace OutpostImmobile.Core.Infrastructure.Queries;

/// <summary>
/// Przykladowo tutaj jest wersja z Value taskiem bo jest szybsza, ale normalnie robimy IRequest<T, Task<T>>
/// </summary>
public record InfrastructureQuery : IRequest<InfrastructureQuery, ValueTask<PingDto>>;

internal class InfrastructureQueryHandler : IRequestHandler<InfrastructureQuery, ValueTask<PingDto>>
{
    /// <summary>
    /// Normalnie tutaj wszystko lecimy na async Task ale no nie mamy jeszcze bazy zeby to ogarnac
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public ValueTask<PingDto> Handle(InfrastructureQuery request, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(new PingDto
        {
            Message = "Pong"
        });
    }
}