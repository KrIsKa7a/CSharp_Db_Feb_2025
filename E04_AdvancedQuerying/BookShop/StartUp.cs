namespace BookShop
{
    using System.Text;

    using Microsoft.EntityFrameworkCore;
    using Z.EntityFramework.Plus;

    using Data;
    using Initializer;
    using Models;
    using Models.Enums;

    public class StartUp
    {
        public static void Main()
        {
            using var dbContext = new BookShopContext();
            DbInitializer.ResetDatabase(dbContext);

            string input = Console.ReadLine();
            string result = GetTotalProfitByCategory(dbContext);

            IncreasePricesBulk(dbContext);

            Console.WriteLine(result);
        }

        // Problem 02
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            string result = String.Empty;

            bool isEnumValid = Enum
                .TryParse(command, true, out AgeRestriction ageRestriction);
            if (!isEnumValid)
            {
                return result;
            }

            string[] bookTitles = context
                .Books
                .Where(b => b.AgeRestriction == ageRestriction)
                .OrderBy(b => b.Title)
                .Select(b => b.Title)
                .ToArray();
            result = String.Join(Environment.NewLine, bookTitles);

            return result;
        }

        // Problem 03
        public static string GetGoldenBooks(BookShopContext context)
        {
            string[] titles = context
                .Books
                .Where(b => b.EditionType == EditionType.Gold &&
                            b.Copies < 5000)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToArray();

            return String.Join(Environment.NewLine, titles);
        }

        // Problem 06
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            string[] searchCategories = input
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.ToLowerInvariant())
                .ToArray();

            // Bulk solution is harder to implement
            // When lazy loading is disabled, navigation properties and collections are "null" until you load them (.Include(), .Load())
            string[] bookTitles = context
                .Books
                //.Include(b => b.BookCategories) // In case we need Eager loading
                //.ThenInclude(bc => bc.Category) // Eager loading stepping inside
                .Where(b => b.BookCategories
                    .Any(bc => searchCategories.Contains(bc.Category.Name.ToLower())))
                .OrderBy(b => b.Title)
                .Select(b => b.Title)
                .ToArray();
            
            return String.Join(Environment.NewLine, bookTitles);
        }

        // Problem 08
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            var authors = context
                .Authors
                .Where(a => a.FirstName != null &&
                            a.FirstName.EndsWith(input))
                .Select(a => new
                {
                    a.FirstName,
                    a.LastName
                }) // Entities are detached but the query is not materialized!
                .OrderBy(a => a.FirstName)
                .ThenBy(a => a.LastName)
                .ToArray(); // Here we materialize the query!

            foreach (var author in authors)
            {
                sb
                    .AppendLine($"{author.FirstName} {author.LastName}");
            }

            return sb.ToString().TrimEnd();
        }

        // Problem 12
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            // In some cases, where complex queries are performed
            // we can materialize the query at some point and the other logic to be executed in program memory
            var authorCopies = context
                .Authors
                .Include(a => a.Books)
                .Select(a => new
                {
                    a.FirstName,
                    a.LastName,
                    TotalCopies = a
                        .Books
                        .Sum(b => b.Copies)
                })
                .OrderByDescending(a => a.TotalCopies)
                .ToArray();

            foreach (var author in authorCopies)
            {
                sb
                    .AppendLine($"{author.FirstName} {author.LastName} - {author.TotalCopies}");
            }

            return sb.ToString().TrimEnd();
        }

        // Problem 13
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            // We can materialize before ordering, since we have at all max 20 categories and this will not cause any overhead for the client App
            var categoriesProfit = context
                .Categories
                .Include(c => c.CategoryBooks)
                .ThenInclude(cb => cb.Book)
                .Select(c => new
                {
                    c.Name,
                    TotalProfit = c
                        .CategoryBooks
                        .Sum(cb => cb.Book.Price * cb.Book.Copies)
                })
                .ToArray() // Materialize earlier to reduce SQL Query complexity
                .OrderByDescending(c => c.TotalProfit)
                .ThenBy(c => c.Name)
                .ToArray(); // This no longer acts as a materialization!!! IOrderedEnumerable<T> -> Array

            foreach (var category in categoriesProfit)
            {
                sb
                    .AppendLine($"{category.Name} ${category.TotalProfit.ToString("F2")}");
            }

            return sb.ToString().TrimEnd();
        }

        // Problem 15
        public static void IncreasePrices(BookShopContext context)
        {
            const int bookPriceIncrement = 5;

            // We will not materialize the query => this ensures that EF can generate more efficient SQL Query
            IQueryable<Book> booksToModify = context
                .Books
                .Where(b => b.ReleaseDate.HasValue &&
                            b.ReleaseDate.Value.Year < 2010);

            // Standard update process -> No bulk operations
            // SQL Query will be generated and executed for each modified entity!
            // foreach will trigger the SQL Query execution of the query built up-to here
            foreach (var book in booksToModify)
            {
                // This will not trigger execution of SQL Query
                book.Price += bookPriceIncrement;
            }

            // Up to here, no Update SQL Query will be executed! SaveChanges() will persist the changes of the attached entities
            context.SaveChanges();
        }

        // Problem 15 -> Improvement
        public static void IncreasePricesBulk(BookShopContext context)
        {
            context
                .Books
                .Where(b => b.ReleaseDate.HasValue &&
                            b.ReleaseDate.Value.Year < 2010)
                .Update(b => new Book()
                {
                    Price = b.Price + 5,
                });
        }
    }
}


