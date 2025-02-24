namespace MusicHub.Data.Configuration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using Models;

    using static EntityValidationConstants.Song;

    public class SongEntityConfiguration : IEntityTypeConfiguration<Song>
    {
        public void Configure(EntityTypeBuilder<Song> entity)
        {
            // Define the primary key in Fluent API <=> [Key]
            entity
                .HasKey(s => s.Id);

            // Define constraints about a property in Fluent API <=> [Required] [MaxLength()]
            entity
                .Property(s => s.Name)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(SongNameMaxLength);

            // Optional, just for description
            entity
                .Property(s => s.Duration)
                .IsRequired();

            entity
                .Property(s => s.CreatedOn)
                .IsRequired();

            entity
                .Property(s => s.Genre)
                .IsRequired();

            entity
                .Property(s => s.Price)
                .IsRequired();

            entity
                .Property(s => s.AlbumId)
                .IsRequired(false);

            entity
                .Property(s => s.WriterId)
                .IsRequired();

            // Relations in FluentAPI are described only from one of the sides
            // It is not needed to describe the relations from both sides!
            // We will describe them from the "One" side
            // One-To-Many relation -> HasOne(navProp) -> WithMany(navColl) -> HasForeignKey(fk)
            // One-To-One relation -> HasOne(navProp) -> WithOne(navProp) -> HasForeignKey(fk)
            // Many-To-Many relation -> 2x [HasOne() -> WithMany() -> HasForeignKey()]
            entity
                .HasOne(s => s.Album)
                .WithMany(a => a.Songs)
                .HasForeignKey(s => s.AlbumId);

            entity
                .HasOne(s => s.Writer)
                .WithMany(w => w.Songs)
                .HasForeignKey(s => s.WriterId);
        }
    }
}
