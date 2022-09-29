using Lection2_Core_29_09_API;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lection2_Core_BL
{
    public class EfDbContext : DbContext
    {
        public DbSet<Good> Goods { get; set; }

        public EfDbContext(DbContextOptions<EfDbContext> options)
            : base(options)
        {
        }
    }
}
