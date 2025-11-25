namespace OutpostImmobile.Core.Paralizator;

public interface IParalizator
{
    public Task<TQuery> Send<TQuery>(TQuery query);
}