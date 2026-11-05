using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ST10538419_CLDV7112_Practicium.Infrastructure.Data;

public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer("Server=localhost;Database=ST10538419_CLDV7112_Practicium;User Id=design;Password=design;TrustServerCertificate=True")
            .Options;

        return new AppDbContext(options);
    }
}
