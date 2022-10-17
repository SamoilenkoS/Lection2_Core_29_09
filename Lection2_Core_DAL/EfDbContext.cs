using Lection2_Core_DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lection2_Core_DAL;

public class EfDbContext : DbContext
{
    private readonly RolesHelper _rolesHelper;
    public DbSet<Good> Goods { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRoles> UserRolesList { get; set; }
    public DbSet<EmailStatus> EmailStatuses { get; set; }

    public EfDbContext(
        DbContextOptions<EfDbContext> options,
        RolesHelper rolesHelper)
        : base(options)
    {
        _rolesHelper = rolesHelper;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRoles>()
            .HasKey(nameof(UserRoles.UserId), nameof(UserRoles.RoleId));

        foreach (var role in new RolesList())
        {
            modelBuilder.Entity<Role>().HasData
                (new Role { Id = _rolesHelper[role], Title = role });
        }
    }
}
