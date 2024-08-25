using Microsoft.EntityFrameworkCore;
using Template.Backend.Entities;

namespace Template.Backend.NewFolder
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Todo> Todos { get; set; }
    }
}
