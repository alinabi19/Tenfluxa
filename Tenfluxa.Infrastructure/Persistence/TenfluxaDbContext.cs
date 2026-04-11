using Microsoft.EntityFrameworkCore;
using Tenfluxa.Domain.Entities;

namespace Tenfluxa.Infrastructure.Persistence;

public class TenfluxaDbContext : DbContext
{
    public TenfluxaDbContext(DbContextOptions<TenfluxaDbContext> options)
        : base(options)
    {
    }

    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<Worker> Workers => Set<Worker>();
    public DbSet<Tenant> Tenants => Set<Tenant>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Example config (important for multi-tenant system)
        modelBuilder.Entity<Job>()
            .HasOne(j => j.AssignedWorker)
            .WithMany()
            .HasForeignKey(j => j.AssignedWorkerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}