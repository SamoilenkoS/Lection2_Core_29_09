using Lection2_Core_DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lection2_Core_DAL;

public class EfDbContext : DbContext
{
    public DbSet<Good> Goods { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRoles> UserRolesList { get; set; }
    public DbSet<EmailStatus> EmailStatuses { get; set; }

    public EfDbContext(DbContextOptions<EfDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRoles>()
            .HasKey(nameof(UserRoles.UserId), nameof(UserRoles.RoleId));

        modelBuilder.Entity<Role>().HasData
            (new Role { Id = Guid.Parse("9d25f40b-88de-4e7f-b76b-74f87f26f654"), Title = "Admin" });

        modelBuilder.Entity<Role>().HasData
            (new Role { Id = Guid.Parse("a2a9a6ba-cc43-4251-bfc9-34791264a417"), Title = "User" });
    }
}
