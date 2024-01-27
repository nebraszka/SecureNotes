using Microsoft.EntityFrameworkCore;

using SecureNotes.Shared.Models;

namespace SecureNotes.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext() { }
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Note> Notes { get; set; }

        public DbSet<LoginAttempt> LoginAttempts { get; set; }

        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     modelBuilder.Entity<User>()
        //         .HasMany(u => u.Notes)
        //         .WithOne(n => n.User)
        //         .HasForeignKey(n => n.UserId)
        //         .OnDelete(DeleteBehavior.Cascade);
        // }
    }
}