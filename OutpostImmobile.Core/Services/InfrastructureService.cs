using OutpostImmobile.Core.Interfaces;
using OutpostImmobile.Core.Models;

namespace OutpostImmobile.Core.Services;

internal class InfrastructureService : IInfrastructureService
{
    public ValueTask<PingDto> PingAsync()
    {
        return ValueTask.FromResult(new PingDto
        {
            Message = "Pong"
        });
    }
}