using ST10538419_CLDV7112_Practicium.Application.Common;
using ST10538419_CLDV7112_Practicium.Domain;
using ST10538419_CLDV7112_Practicium.Domain.Entities;

namespace ST10538419_CLDV7112_Practicium.Application.Accounts;

public sealed class UserAccountService(IUserAccountRepository repository) : IUserAccountService
{
    public const int UsernameMaxLength = 40;

    public const int PasswordMinLength = 8;

    public const int PasswordMaxLength = 80;

    public Task<IReadOnlyList<UserAccount>> ListAsync(string? search, CancellationToken cancellationToken) =>
        repository.ListAsync(search, cancellationToken);

    public async Task<int> CountAsync(CancellationToken cancellationToken) =>
        (await repository.ListAsync(null, cancellationToken)).Count;

    public Task<UserAccount?> GetAsync(Guid id, CancellationToken cancellationToken) =>
        repository.GetAsync(id, cancellationToken);

    public async Task<ServiceResult<UserAccount>> CreateAsync(UserAccountInput input, CancellationToken cancellationToken)
    {
        var error = Validate(input, passwordRequired: true);
        if (error is not null)
        {
            return ServiceResult<UserAccount>.Fail(error);
        }

        var username = input.Username.Trim();
        if (await repository.UsernameExistsAsync(username, null, cancellationToken))
        {
            return ServiceResult<UserAccount>.Fail("That username is already in use.");
        }

        var account = new UserAccount
        {
            Username = username,
            PasswordHash = PasswordHasher.Hash(input.Password!.Trim()),
            Role = input.Role.Trim()
        };

        await repository.AddAsync(account, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return ServiceResult<UserAccount>.Ok(account);
    }

    public async Task<ServiceResult<UserAccount>> UpdateAsync(
        Guid id,
        UserAccountInput input,
        CancellationToken cancellationToken)
    {
        var account = await repository.GetAsync(id, cancellationToken);
        if (account is null)
        {
            return ServiceResult<UserAccount>.Fail("Account was not found.");
        }

        var error = Validate(input, passwordRequired: false);
        if (error is not null)
        {
            return ServiceResult<UserAccount>.Fail(error);
        }

        var username = input.Username.Trim();
        if (await repository.UsernameExistsAsync(username, id, cancellationToken))
        {
            return ServiceResult<UserAccount>.Fail("That username is already in use.");
        }

        account.Username = username;
        account.Role = input.Role.Trim();
        if (!string.IsNullOrWhiteSpace(input.Password))
        {
            account.PasswordHash = PasswordHasher.Hash(input.Password.Trim());
        }

        account.UpdatedAt = DateTime.UtcNow;
        await repository.SaveChangesAsync(cancellationToken);
        return ServiceResult<UserAccount>.Ok(account);
    }

    public async Task<ServiceResult<UserAccount>> SignInAsync(
        string username,
        string password,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return ServiceResult<UserAccount>.Fail("Username or password is incorrect.");
        }

        var account = await repository.FindByUsernameAsync(username.Trim(), cancellationToken);
        if (account is null || !PasswordHasher.Verify(password, account.PasswordHash))
        {
            return ServiceResult<UserAccount>.Fail("Username or password is incorrect.");
        }

        return ServiceResult<UserAccount>.Ok(account);
    }

    private static string? Validate(UserAccountInput input, bool passwordRequired)
    {
        if (string.IsNullOrWhiteSpace(input.Username))
        {
            return "Username is required.";
        }

        var username = input.Username.Trim();
        if (username.Length < 3 || username.Length > UsernameMaxLength)
        {
            return $"Username must be 3 to {UsernameMaxLength} characters.";
        }

        if (username.Any(character => !char.IsLetterOrDigit(character)))
        {
            return "Username can use letters and digits only.";
        }

        if (!UserRoles.IsKnown(input.Role))
        {
            return "Role is not recognised.";
        }

        var password = input.Password?.Trim() ?? string.Empty;
        if (passwordRequired && password.Length == 0)
        {
            return "Password is required.";
        }

        if (password.Length > 0 && password.Length < PasswordMinLength)
        {
            return $"Password must be at least {PasswordMinLength} characters.";
        }

        if (password.Length > PasswordMaxLength)
        {
            return $"Password must be {PasswordMaxLength} characters or fewer.";
        }

        return null;
    }
}
