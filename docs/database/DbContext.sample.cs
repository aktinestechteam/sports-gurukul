using Microsoft.EntityFrameworkCore;

public class SportsGurukulDbContext : DbContext
{
    public SportsGurukulDbContext(DbContextOptions<SportsGurukulDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
}
