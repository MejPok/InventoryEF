using Microsoft.EntityFrameworkCore;
using System.IO;


namespace InventoryEF
{
    public class LibraryContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbPath = Path.Combine(AppContext.BaseDirectory, "library.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
            
        }
    }
}