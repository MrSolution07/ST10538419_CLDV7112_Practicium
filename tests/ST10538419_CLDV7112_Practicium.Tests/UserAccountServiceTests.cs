using ST10538419_CLDV7112_Practicium.Application.Accounts;
using ST10538419_CLDV7112_Practicium.Domain;
using ST10538419_CLDV7112_Practicium.Domain.Entities;

namespace ST10538419_CLDV7112_Practicium.Tests;

public class UserAccountServiceTests
{
    private readonly InMemoryUserAccountRepository repository = new();
    private readonly UserAccountService service;

    public UserAccountServiceTests()
    {
        service = new UserAccountService(repository);
    }

    [Fact]
    public async Task Create_rejects_blank_username()
    {
        var result = await service.CreateAsync(new UserAccountInput("  ", "Farmer-field-1", UserRoles.Farmer), CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal("Username is required.", result.Error);
    }

    [Fact]
    public async Task Create_rejects_short_password()
    {
        var result = await service.CreateAsync(new UserAccountInput("fieldhand", "short", UserRoles.Farmer), CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal("Password must be at least 8 characters.", result.Error);
    }

    [Fact]
    public async Task Create_rejects_unknown_role()
    {
        var result = await service.CreateAsync(new UserAccountInput("fieldhand", "Farmer-field-1", "Guest"), CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal("Role is not recognised.", result.Error);
    }

    [Fact]
    public async Task Create_rejects_duplicate_username()
    {
        await service.CreateAsync(new UserAccountInput("farmer", "Farmer-field-1", UserRoles.Farmer), CancellationToken.None);

        var result = await service.CreateAsync(new UserAccountInput("Farmer", "Agronomist-field-1", UserRoles.Admin), CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal("That username is already in use.", result.Error);
    }

    [Fact]
    public async Task Sign_in_accepts_the_seed_password_and_rejects_a_wrong_one()
    {
        await service.CreateAsync(new UserAccountInput("farmer", "Farmer-field-1", UserRoles.Farmer), CancellationToken.None);

        var success = await service.SignInAsync("farmer", "Farmer-field-1", CancellationToken.None);
        var failure = await service.SignInAsync("farmer", "wrong-password", CancellationToken.None);

        Assert.True(success.Succeeded);
        Assert.Equal(UserRoles.Farmer, success.Value!.Role);
        Assert.False(failure.Succeeded);
        Assert.Equal("Username or password is incorrect.", failure.Error);
    }

    private sealed class InMemoryUserAccountRepository : IUserAccountRepository
    {
        private readonly List<UserAccount> accounts = [];

        public Task<IReadOnlyList<UserAccount>> ListAsync(string? search, CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<UserAccount>>(accounts);

        public Task<UserAccount?> GetAsync(Guid id, CancellationToken cancellationToken) =>
            Task.FromResult(accounts.FirstOrDefault(account => account.Id == id));

        public Task<UserAccount?> FindByUsernameAsync(string username, CancellationToken cancellationToken) =>
            Task.FromResult(accounts.FirstOrDefault(account => account.Username.Equals(username, StringComparison.OrdinalIgnoreCase)));

        public Task<bool> UsernameExistsAsync(string username, Guid? excludingId, CancellationToken cancellationToken) =>
            Task.FromResult(accounts.Any(account =>
                account.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && account.Id != excludingId));

        public Task AddAsync(UserAccount account, CancellationToken cancellationToken)
        {
            accounts.Add(account);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
