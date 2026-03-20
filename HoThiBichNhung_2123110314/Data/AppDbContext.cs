using HoThiBichNhung_2123110314.Models;
using Microsoft.EntityFrameworkCore;

namespace HoThiBichNhung_2123110314.Data
{

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
    }
}
