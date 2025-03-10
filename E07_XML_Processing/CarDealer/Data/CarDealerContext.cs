namespace CarDealer.Data
{
    using Microsoft.EntityFrameworkCore;

    using Models;

    public class CarDealerContext : DbContext
    {
        public CarDealerContext()
        {

        }

        public CarDealerContext(DbContextOptions options)
            : base(options)
        {

        }

        public virtual DbSet<Car> Cars { get; set; } = null!;

        public virtual DbSet<Customer> Customers { get; set; } = null!;

        public virtual DbSet<Part> Parts { get; set; } = null!;

        public virtual DbSet<PartCar> PartsCars { get; set; } = null!;

        public virtual DbSet<Sale> Sales { get; set; } = null!;

        public virtual DbSet<Supplier> Suppliers { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PartCar>(e =>
            {
                e
                    .HasKey(pc => new { pc.CarId, pc.PartId });
            });
        }
    }
}
