using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions dbContextOptions)
            :base(dbContextOptions)
        {
            
        }

        public DbSet<Models.Stock> Stocks { get; set; } 
        public DbSet<Models.Comment> Comments { get; set; }
    }
}
