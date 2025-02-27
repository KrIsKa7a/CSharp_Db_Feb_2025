namespace BookShop.Common
{
    public static class EntityValidationConstants
    {
        public static class Author
        {
            public const int AuthorFirstNameMaxLength = 50;
            public const int AuthorLastNameMaxLength = 50;
        }

        public static class Book
        {
            public const int BookTitleMaxLength = 50;
            public const int BookDescriptionMaxLength = 1000;
        }

        public static class Category
        {
            public const int CategoryNameMaxLength = 50;
        }
    }
}
