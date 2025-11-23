using OutpostImmobile.Core.Models;

namespace OutpostImmobile.Core.Interfaces;

public interface IInfrastructureService
{
    ValueTask<PingDto> PingAsync(); 
}