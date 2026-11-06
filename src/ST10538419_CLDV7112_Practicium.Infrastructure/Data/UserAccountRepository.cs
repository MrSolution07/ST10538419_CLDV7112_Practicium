using ST10538419_CLDV7112_Practicium.Application.Accounts;
using ST10538419_CLDV7112_Practicium.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ST10538419_CLDV7112_Practicium.Infrastructure.Data;

public sealed class UserAccountRepository(AppDbContext db) : IUserAccountRepository
{
    public async Task<IReadOnlyList<UserAccount>> ListAsync(string? search, CancellationToken cancellationToken)
    {
        var query = db.UserAccounts.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(account => account.Username.Contains(term) || account.Role.Contains(term));
        }

        return await query
            .OrderBy(account => account.Username)
            .ToListAsync(cancellationToken);
    }

    public Task<UserAccount?> GetAsync(Guid id, CancellationToken cancellationToken) =>
        db.UserAccounts.FirstOrDefaultAsync(account => account.Id == id, cancellationToken);

    public Task<UserAccount?> FindByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        var normalised = username.Trim().ToLower();
        return db.UserAccounts.FirstOrDefaultAsync(
            account => account.Username.ToLower() == normalised,
            cancellationToken);
    }

    public Task<bool> UsernameExistsAsync(string username, Guid? excludingId, CancellationToken cancellationToken)
    {
        var normalised = username.Trim().ToLower();
        if (excludingId is null)
        {
            return db.UserAccounts.AnyAsync(account => account.Username.ToLower() == normalised, cancellationToken);
        }

        return db.UserAccounts.AnyAsync(
            account => account.Username.ToLower() == normalised && account.Id != excludingId,
            cancellationToken);
    }

    public async Task AddAsync(UserAccount account, CancellationToken cancellationToken) =>
        await db.UserAccounts.AddAsync(account, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        db.SaveChangesAsync(cancellationToken);
}
