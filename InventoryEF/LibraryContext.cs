using Microsoft.EntityFrameworkCore;


namespace InventoryEF
{
    public class LibraryContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=library.db");  // SQLite database
        }
    }
}