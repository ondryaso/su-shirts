using Microsoft.EntityFrameworkCore;
using SUShirts.Data.Entities;

namespace SUShirts.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Color> Colors { get; set; }
        public DbSet<Shirt> Shirts { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ShirtVariant> ShirtVariants { get; set; }

        public AppDbContext() : base()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Shirt>()
                .HasMany(s => s.Variants)
                .WithOne(sv => sv.Shirt)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ShirtVariant>();

            modelBuilder.Entity<Reservation>()
                .HasMany(r => r.Items)
                .WithOne(ri => ri.Reservation)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReservationItem>()
                .HasKey(ri => new {ri.ReservationId, ri.ShirtVariantId});

            modelBuilder.Entity<ReservationItem>()
                .HasOne(ri => ri.ShirtVariant)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
