namespace MusicHub.Data
{
    using Microsoft.EntityFrameworkCore;

    using Configuration;
    using Models;

    using static Configuration.ConnectionConfiguration;

    public class MusicHubDbContext : DbContext
    {
        public MusicHubDbContext()
        {

        }

        public MusicHubDbContext(DbContextOptions options)
            : base(options)
        {

        }

        public virtual DbSet<Album> Albums { get; set; }

        public virtual DbSet<Performer> Performers { get; set; }

        public virtual DbSet<Producer> Producers { get; set; }

        public virtual DbSet<Song> Songs { get; set; }

        public virtual DbSet<SongPerformer> SongsPerformers { get; set; }

        public virtual DbSet<Writer> Writers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(ConnectionString);
            }
        }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Using this approach we can guarantee that as long as all configurations are placed in the same assembly as SongEntityConfiguration, they will be applied automatically
            base.OnModelCreating(builder);

            builder
                .ApplyConfigurationsFromAssembly(typeof(SongEntityConfiguration).Assembly);
        }
    }
}
