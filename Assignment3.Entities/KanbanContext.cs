using Microsoft.EntityFrameworkCore;
//using System.ComponentModel.DataAnnotations;

namespace Assignment3.Entities;

public partial class KanbanContext : DbContext
{
    public KanbanContext(DbContextOptions<KanbanContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Task> Tasks => Set<Task>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Tag> Tags => Set<Tag>();  

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Task>()
                .Property(c => c.State)
                .HasConversion(v => v.ToString(),
                v => (State)Enum.Parse(typeof(State), v));
            modelBuilder.Entity<Task>()
                .Property(c => c.Title)
                .HasMaxLength(100);

            modelBuilder.Entity<Tag>()
                .HasKey(c => c.Id);
            modelBuilder.Entity<Tag>()
                .Property(c => c.Name)
                .HasMaxLength(50);

            modelBuilder.Entity<User>()
                .Property(c => c.Name)
                .HasMaxLength(100);
            modelBuilder.Entity<User>()
                .Property(c => c.Email)
                .HasMaxLength(100);
        
        }
}
