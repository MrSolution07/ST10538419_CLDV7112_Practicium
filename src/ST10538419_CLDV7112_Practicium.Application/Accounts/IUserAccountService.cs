using ST10538419_CLDV7112_Practicium.Application.Common;
using ST10538419_CLDV7112_Practicium.Domain.Entities;

namespace ST10538419_CLDV7112_Practicium.Application.Accounts;

public interface IUserAccountService
{
    Task<IReadOnlyList<UserAccount>> ListAsync(string? search, CancellationToken cancellationToken);

    Task<int> CountAsync(CancellationToken cancellationToken);

    Task<UserAccount?> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<ServiceResult<UserAccount>> CreateAsync(UserAccountInput input, CancellationToken cancellationToken);

    Task<ServiceResult<UserAccount>> UpdateAsync(Guid id, UserAccountInput input, CancellationToken cancellationToken);

    Task<ServiceResult<UserAccount>> SignInAsync(string username, string password, CancellationToken cancellationToken);
}
