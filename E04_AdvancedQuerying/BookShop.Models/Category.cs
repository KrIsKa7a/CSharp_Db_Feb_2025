namespace BookShop.Models
{
    using System.Collections.Generic;

    public class Category
    {
        public Category()
        {
            this.CategoryBooks = new HashSet<BookCategory>();
        }

        public int CategoryId { get; set; }

        public string Name { get; set; } = null!;

        public ICollection<BookCategory> CategoryBooks { get; set; }
    }
}