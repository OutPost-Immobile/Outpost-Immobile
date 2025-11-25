namespace OutpostImmobile.Core.Paralizator.Internal;

internal class Paralizator : IParalizator
{
    private readonly IServiceProvider _serviceProvider;

    public Paralizator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task<TQuery> Send<TQuery>(TQuery query)
    {
        throw new NotImplementedException();
    }
}