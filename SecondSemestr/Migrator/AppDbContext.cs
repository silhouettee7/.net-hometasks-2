using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Migrator;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public AppDbContext() { }
    
    public DbSet<User> Users { get; set; }
    public DbSet<EmailMessage> EmailMessages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        optionsBuilder.UseNpgsql(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.HasIndex(e => e.Email).IsUnique();
        });
        
        modelBuilder.Entity<EmailMessage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Subject).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Body).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Status).IsRequired().HasConversion<int>();
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.EmailMessages)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}