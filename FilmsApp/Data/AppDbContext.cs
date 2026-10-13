using FilmsApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FilmsApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Film> Films { get; set; }
    }
}
