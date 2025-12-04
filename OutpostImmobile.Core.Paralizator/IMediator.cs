using OutpostImmobile.Core.Paralizator.Abstraction;

namespace OutpostImmobile.Core.Paralizator;

public interface IMediator
{
    public Task<TResponse> Send<TRequest, TResponse>(IRequest<TRequest, TResponse> request);
}