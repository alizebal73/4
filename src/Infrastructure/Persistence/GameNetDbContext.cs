using Microsoft.EntityFrameworkCore;

namespace GameNet.Infrastructure.Persistence;

public sealed class GameNetDbContext(DbContextOptions<GameNetDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("gamenet");
        base.OnModelCreating(modelBuilder);
    }
}
