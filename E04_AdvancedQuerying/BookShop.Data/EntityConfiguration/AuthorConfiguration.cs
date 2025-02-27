namespace BookShop.Data.EntityConfiguration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using Models;

    using static Common.EntityValidationConstants.Author;

    internal class AuthorConfiguration : IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author> builder)
        {
            builder
                .HasKey(e => e.AuthorId);

            builder
                .Property(e => e.FirstName)
                .IsRequired(false)
                .HasMaxLength(AuthorFirstNameMaxLength);

            builder
                .Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(AuthorLastNameMaxLength);
        }
    }
}
