using Microsoft.EntityFrameworkCore;

namespace BookRadar.App;

public class AppDbContext : DbContext
{
    public AppDbContext() { }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Book> Books { get; set;} = null!;
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured){
        optionsBuilder.UseSqlite("Data Source=books.db");
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>()
        .HasIndex(b => b.OpenLibraryKey)
        .IsUnique();
    }
}
