namespace MusicHub.Data.Configuration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using Models;

    using static EntityValidationConstants.Performer;

    public class PerformerEntityConfiguration : IEntityTypeConfiguration<Performer>
    {
        public void Configure(EntityTypeBuilder<Performer> entity)
        {
            entity
                .HasKey(p => p.Id);

            entity
                .Property(p => p.FirstName)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(PerformerFirstNameMaxLength);

            entity
                .Property(p => p.LastName)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(PerformerLastNameMaxLength);

            entity
                .Property(p => p.Age)
                .IsRequired();

            entity
                .Property(p => p.NetWorth)
                .IsRequired();
        }
    }
}
