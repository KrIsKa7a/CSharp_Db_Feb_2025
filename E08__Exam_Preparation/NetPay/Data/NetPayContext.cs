namespace NetPay.Data
{
    using Microsoft.EntityFrameworkCore;

    using Models;
    using static Configuration;

    public class NetPayContext : DbContext
    {
        public NetPayContext()
        {
            
        }

        public NetPayContext(DbContextOptions options)
            : base(options)
        {
            
        }

        public virtual DbSet<Expense> Expenses { get; set; } = null!;

        public virtual DbSet<Household> Households { get; set; } = null!;

        public virtual DbSet<Service> Services { get; set; } = null!;

        public virtual DbSet<Supplier> Suppliers { get; set; } = null!;

        public virtual DbSet<SupplierService> SuppliersServices { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Fluent API here
            modelBuilder
                .Entity<SupplierService>(entity =>
                {
                    entity
                        .HasKey(ss => new { ss.SupplierId, ss.ServiceId });
                });

            //Uncomment the following lines to seed the database with data

            modelBuilder
                .Entity<Supplier>()
                .HasData(
                new Supplier { Id = 1, SupplierName = "E-Service" },
                new Supplier { Id = 2, SupplierName = "Light" },
                new Supplier { Id = 3, SupplierName = "Energy-PRO" },
                new Supplier { Id = 4, SupplierName = "ZEC" },
                new Supplier { Id = 5, SupplierName = "Cellular" },
                new Supplier { Id = 6, SupplierName = "A2one" },
                new Supplier { Id = 7, SupplierName = "Telecom" },
                new Supplier { Id = 8, SupplierName = "Cell2U" },
                new Supplier { Id = 9, SupplierName = "DigiTV" },
                new Supplier { Id = 10, SupplierName = "NetCom" },
                new Supplier { Id = 11, SupplierName = "Net1" },
                new Supplier { Id = 12, SupplierName = "MaxTel" },
                new Supplier { Id = 13, SupplierName = "WaterSupplyCentral" },
                new Supplier { Id = 14, SupplierName = "WaterSupplyNorth" },
                new Supplier { Id = 15, SupplierName = "WaterSupplySouth" },
                new Supplier { Id = 16, SupplierName = "FiberScreen" },
                new Supplier { Id = 17, SupplierName = "SpeedNet" },
                new Supplier { Id = 18, SupplierName = "GasGas" },
                new Supplier { Id = 19, SupplierName = "BlueHome" },
                new Supplier { Id = 20, SupplierName = "SecureHouse" },
                new Supplier { Id = 21, SupplierName = "HomeGuard" },
                new Supplier { Id = 22, SupplierName = "SafeHome" });

            modelBuilder
                .Entity<Service>()
                .HasData(
                new Service { Id = 1, ServiceName = "Electricity" },
                new Service { Id = 2, ServiceName = "Water" },
                new Service { Id = 3, ServiceName = "Internet" },
                new Service { Id = 4, ServiceName = "TV" },
                new Service { Id = 5, ServiceName = "Phone" },
                new Service { Id = 6, ServiceName = "Security" },
                new Service { Id = 7, ServiceName = "Gas" });

            modelBuilder
                .Entity<SupplierService>()
                .HasData(
                new SupplierService { SupplierId = 1, ServiceId = 1 },
                new SupplierService { SupplierId = 2, ServiceId = 1 },
                new SupplierService { SupplierId = 3, ServiceId = 1 },
                new SupplierService { SupplierId = 4, ServiceId = 1 },
                new SupplierService { SupplierId = 5, ServiceId = 3 },
                new SupplierService { SupplierId = 5, ServiceId = 4 },
                new SupplierService { SupplierId = 5, ServiceId = 5 },
                new SupplierService { SupplierId = 6, ServiceId = 3 },
                new SupplierService { SupplierId = 6, ServiceId = 4 },
                new SupplierService { SupplierId = 6, ServiceId = 5 },
                new SupplierService { SupplierId = 6, ServiceId = 6 },
                new SupplierService { SupplierId = 7, ServiceId = 3 },
                new SupplierService { SupplierId = 7, ServiceId = 4 },
                new SupplierService { SupplierId = 7, ServiceId = 5 },
                new SupplierService { SupplierId = 8, ServiceId = 3 },
                new SupplierService { SupplierId = 8, ServiceId = 4 },
                new SupplierService { SupplierId = 8, ServiceId = 5 },
                new SupplierService { SupplierId = 9, ServiceId = 3 },
                new SupplierService { SupplierId = 9, ServiceId = 4 },
                new SupplierService { SupplierId = 10, ServiceId = 3 },
                new SupplierService { SupplierId = 10, ServiceId = 4 },
                new SupplierService { SupplierId = 10, ServiceId = 6 },
                new SupplierService { SupplierId = 11, ServiceId = 3 },
                new SupplierService { SupplierId = 11, ServiceId = 4 },
                new SupplierService { SupplierId = 12, ServiceId = 3 },
                new SupplierService { SupplierId = 12, ServiceId = 4 },
                new SupplierService { SupplierId = 12, ServiceId = 5 },
                new SupplierService { SupplierId = 12, ServiceId = 6 },
                new SupplierService { SupplierId = 13, ServiceId = 2 },
                new SupplierService { SupplierId = 14, ServiceId = 2 },
                new SupplierService { SupplierId = 15, ServiceId = 2 },
                new SupplierService { SupplierId = 16, ServiceId = 4 },
                new SupplierService { SupplierId = 16, ServiceId = 3 },
                new SupplierService { SupplierId = 17, ServiceId = 3 },
                new SupplierService { SupplierId = 17, ServiceId = 4 },
                new SupplierService { SupplierId = 17, ServiceId = 6 },
                new SupplierService { SupplierId = 18, ServiceId = 7 },
                new SupplierService { SupplierId = 19, ServiceId = 7 },
                new SupplierService { SupplierId = 20, ServiceId = 6 },
                new SupplierService { SupplierId = 21, ServiceId = 6 },
                new SupplierService { SupplierId = 22, ServiceId = 6 });
        }
    }
}
