namespace MusicHub.Data.Configuration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using Models;

    public class SongPerformerEntityConfiguration : IEntityTypeConfiguration<SongPerformer>
    {
        public void Configure(EntityTypeBuilder<SongPerformer> entity)
        {
            // Composite PK in EF Core is always represented as an object
            entity
                .HasKey(sp => new { sp.SongId, sp.PerformerId });

            entity
                .Property(sp => sp.SongId)
                .IsRequired();

            entity
                .Property(sp => sp.PerformerId)
                .IsRequired();

            // 2 x One-To-Many <=> Many-To-Many
            entity
                .HasOne(sp => sp.Song)
                .WithMany(s => s.SongPerformers)
                .HasForeignKey(sp => sp.SongId);

            entity
                .HasOne(sp => sp.Performer)
                .WithMany(p => p.PerformerSongs)
                .HasForeignKey(sp => sp.PerformerId);
        }
    }
}
