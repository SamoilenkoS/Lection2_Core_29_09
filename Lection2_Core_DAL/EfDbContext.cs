using Microsoft.EntityFrameworkCore;

namespace Lection2_Core_DAL;

public class EfDbContext : DbContext
{
    public DbSet<Good> Goods { get; set; }

    public EfDbContext(DbContextOptions<EfDbContext> options)
        : base(options)
    {
    }
}
