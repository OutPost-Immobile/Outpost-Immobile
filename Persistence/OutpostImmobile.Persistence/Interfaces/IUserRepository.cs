namespace OutpostImmobile.Persistence.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<Tuple<string, string>>> GetUserEmailCredentials(IEnumerable<Guid> userId);
}