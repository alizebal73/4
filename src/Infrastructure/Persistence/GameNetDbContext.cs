using GameNet.Domain.Agents;
using GameNet.Domain.Identity;
using GameNet.Domain.Stations;
using Microsoft.EntityFrameworkCore;

namespace GameNet.Infrastructure.Persistence;

public sealed class GameNetDbContext(DbContextOptions<GameNetDbContext> options) : DbContext(options)
{
    public DbSet<Operator> Operators => Set<Operator>();
    public DbSet<OperatorSession> OperatorSessions => Set<OperatorSession>();
    public DbSet<Station> Stations => Set<Station>();
    public DbSet<AgentDevice> AgentDevices => Set<AgentDevice>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("gamenet");

        modelBuilder.Entity<Operator>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.UserName).HasMaxLength(64).IsRequired();
            entity.HasIndex(x => x.UserName).IsUnique();
            entity.Property(x => x.DisplayName).HasMaxLength(128).IsRequired();
            entity.Property(x => x.PasswordHash).HasMaxLength(512).IsRequired();
            entity.Property(x => x.Role).HasConversion<string>().HasMaxLength(32).IsRequired();
        });

        modelBuilder.Entity<OperatorSession>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.TokenHash).HasMaxLength(128).IsRequired();
            entity.HasIndex(x => x.TokenHash).IsUnique();
            entity.HasOne<Operator>()
                .WithMany()
                .HasForeignKey(x => x.OperatorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Station>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Number).IsRequired();
            entity.HasIndex(x => x.Number).IsUnique();
            entity.Property(x => x.Name).HasMaxLength(128).IsRequired();
            entity.Property(x => x.Type).HasConversion<string>().HasMaxLength(32).IsRequired();
            entity.Property(x => x.Lifecycle).HasConversion<string>().HasMaxLength(32).IsRequired();
        });

        modelBuilder.Entity<AgentDevice>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.DeviceId).HasMaxLength(128).IsRequired();
            entity.HasIndex(x => x.DeviceId).IsUnique();
            entity.Property(x => x.DisplayName).HasMaxLength(128).IsRequired();
            entity.Property(x => x.CredentialHash).HasMaxLength(128);
            entity.HasIndex(x => x.CredentialHash).IsUnique();
            entity.Property(x => x.PairingCodeHash).HasMaxLength(128);
            entity.Property(x => x.ConnectionId).HasMaxLength(256);
            entity.Property(x => x.State).HasConversion<string>().HasMaxLength(32).IsRequired();
            entity.Property(x => x.Version).IsConcurrencyToken().IsRequired();
            entity.HasIndex(x => x.StationId).IsUnique();
            entity.HasOne<Station>()
                .WithMany()
                .HasForeignKey(x => x.StationId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
