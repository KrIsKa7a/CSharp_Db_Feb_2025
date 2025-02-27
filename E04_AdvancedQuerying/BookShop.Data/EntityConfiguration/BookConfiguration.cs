namespace BookShop.Data.EntityConfiguration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using Models;

    using static Common.EntityValidationConstants.Book;

    internal class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder
                .HasKey(e => e.BookId);

            builder
                .Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(BookTitleMaxLength);

            builder
                .Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(BookDescriptionMaxLength);

            builder
                .Property(e => e.ReleaseDate)
                .IsRequired(false);

            builder
                .HasOne(e => e.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(e => e.AuthorId);
        }
    }
}
