using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserProfile> UserProfiles { get; set; } = null!;
    public DbSet<Expense> Expenses { get; set; } = null!;
    public DbSet<ExpenseItem> ExpenseItems { get; set; } = null!;
    public DbSet<ActivityLog> ActivityLogs { get; set; } = null!;
    public DbSet<UserCredential> UserCredentials { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // UserProfiles table configuration
        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.ToTable("UserProfiles");
            entity.HasKey(e => e.EmployeeId);
            entity.Property(e => e.EmployeeId).HasColumnType("varchar(50)");
            entity.Property(e => e.BudgetLimit).HasPrecision(18, 2);
            entity.Property(e => e.SpentAmount).HasPrecision(18, 2);
        });

        // Expenses table configuration
        modelBuilder.Entity<Expense>(entity =>
        {
            entity.ToTable("Expenses");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnType("varchar(50)");
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            
            // One-to-many relationship with ExpenseItems
            entity.HasMany(e => e.Items)
                  .WithOne()
                  .HasForeignKey(ei => ei.ExpenseId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ExpenseItems table configuration
        modelBuilder.Entity<ExpenseItem>(entity =>
        {
            entity.ToTable("ExpenseItems");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnType("varchar(50)");
            entity.Property(e => e.ExpenseId).HasColumnType("varchar(50)");
            entity.Property(e => e.Cost).HasPrecision(18, 2);
        });

        // ActivityLogs table configuration
        modelBuilder.Entity<ActivityLog>(entity =>
        {
            entity.ToTable("ActivityLogs");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnType("varchar(50)");
        });

        // UserCredentials table configuration
        modelBuilder.Entity<UserCredential>(entity =>
        {
            entity.ToTable("UserCredentials");
            entity.HasKey(e => e.Username);
            entity.Property(e => e.Username).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.DisplayName).HasMaxLength(100);
            entity.Property(e => e.Role).HasMaxLength(50);
        });
    }
}
