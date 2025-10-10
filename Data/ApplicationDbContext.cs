using AgriEnergyConnectPrototype.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>

{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<FarmerProfile> FarmerProfiles { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Product>()
            .HasOne(p => p.FarmerProfile)
            .WithMany(f => f.Products)
            .HasForeignKey(p => p.FarmerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
