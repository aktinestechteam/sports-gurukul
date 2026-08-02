using Microsoft.EntityFrameworkCore;
using SportsGurukul.Infrastructure.Persistence;

namespace AI.Infrastructure.Tests;

public static class InMemoryDbContextFactory
{
    public static ApplicationDbContext Create()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }
}
