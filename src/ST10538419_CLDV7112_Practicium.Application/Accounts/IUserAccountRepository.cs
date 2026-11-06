using ST10538419_CLDV7112_Practicium.Domain.Entities;

namespace ST10538419_CLDV7112_Practicium.Application.Accounts;

public interface IUserAccountRepository
{
    Task<IReadOnlyList<UserAccount>> ListAsync(string? search, CancellationToken cancellationToken);

    Task<UserAccount?> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<UserAccount?> FindByUsernameAsync(string username, CancellationToken cancellationToken);

    Task<bool> UsernameExistsAsync(string username, Guid? excludingId, CancellationToken cancellationToken);

    Task AddAsync(UserAccount account, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
