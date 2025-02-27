namespace BookShop.Models
{
    using System;
    using System.Collections.Generic;

    using Enums;

    public class Book
    {
        public Book()
        {
            this.BookCategories = new HashSet<BookCategory>();
        }

        public int BookId { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public EditionType EditionType { get; set; }

        public decimal Price { get; set; }

        public int Copies { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public AgeRestriction AgeRestriction { get; set; }

        public int AuthorId { get; set; }

        public Author Author { get; set; } = null!;

        public ICollection<BookCategory> BookCategories { get; set; }
    }
}
