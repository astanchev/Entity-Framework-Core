namespace BookShop
{
    using Data;

    using Microsoft.EntityFrameworkCore;

    using Models;
    using Models.Enums;

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using (var db = new BookShopContext())
            {
               //DbInitializer.ResetDatabase(db);

               var result = GetMostRecentBooks(db);

               //Console.WriteLine(result.Length);

               Console.WriteLine(result);

            }
        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            var ageRestriction = Enum.Parse<AgeRestriction>(command, true);

            var bookTitles = context.Books
                .Where(b => b.AgeRestriction == ageRestriction)
                .Select(b => b.Title)
                .OrderBy(t => t)
                .ToList();

            return string.Join(Environment.NewLine, bookTitles);
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            var bookTitles = context.Books
                .Where(b => b.EditionType == EditionType.Gold && b.Copies < 5000)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToList();

            return string.Join(Environment.NewLine, bookTitles);
        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var resultBooks = context.Books
                .Where(b => b.Price > 40)
                .Select(b => new
                {
                    Title = b.Title,
                    Price = b.Price
                })
                .OrderByDescending(rb => rb.Price)
                .ToList();

            foreach (var book in resultBooks)
            {
                sb.AppendLine($"{book.Title} - ${book.Price:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var bookTitles = context.Books
                .Where(b => b.ReleaseDate.Value.Year != year)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToList();

            return string.Join(Environment.NewLine, bookTitles);
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            List<string> categories = input.ToLower().Split(" ", StringSplitOptions.RemoveEmptyEntries).ToList();

            var bookTitles = context.Books
                .Where(bc => bc.BookCategories.Any(c => categories.Contains(c.Category.Name.ToLower())))
                .Select(b => b.Title)
                .OrderBy(t => t)
                .ToList();

            return string.Join(Environment.NewLine, bookTitles);
        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var checkDate = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            StringBuilder sb = new StringBuilder();

            var resultBooks = context.Books
                .Where(b => b.ReleaseDate < checkDate)
                .OrderByDescending(b => b.ReleaseDate)
                .Select(b => new
                {
                    Title = b.Title,
                    EditionType = b.EditionType,
                    Price = b.Price
                })
                .ToList();

            foreach (var book in resultBooks)
            {
                sb.AppendLine($"{book.Title} - {book.EditionType} - ${book.Price:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context.Authors
                .Where(a => EF.Functions.Like(a.FirstName, $"%{input}"))
                .Select(a => a.FirstName + " " + a.LastName)
                .OrderBy(a => a)
                .ToList();

            return string.Join(Environment.NewLine, authors);
        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var bookTitles = context.Books
                .Where(b => EF.Functions.Like(b.Title, $"%{input}%"))
                .Select(b => b.Title)
                .OrderBy(t => t)
                .ToList();

            return string.Join(Environment.NewLine, bookTitles);
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            var books = context.Books
                .Where(b => EF.Functions.Like(b.Author.LastName, $"{input}%"))
                .OrderBy(b => b.BookId)
                .Select(b => new
                {
                    Title = b.Title,
                    Author = b.Author.FirstName + " " + b.Author.LastName
                })
                .ToList();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} ({book.Author})");
            }

            return sb.ToString().TrimEnd();
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            return context.Books.Count(b => b.Title.Length > lengthCheck);
        }

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var resultAuthors = context.Authors
                .Select(a => new
                {
                    Name = a.FirstName + " " + a.LastName,
                    TotalCopies = a.Books.Sum(b => b.Copies)
                })
                .OrderByDescending(b => b.TotalCopies)
                .ToList();

            foreach (var author in resultAuthors)
            {
                sb.AppendLine($"{author.Name} - {author.TotalCopies}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var categoryProfit = context.Categories
                .Select(c => new
                {
                    CategoryName = c.Name,
                    Profit = c.CategoryBooks.Sum(cb => cb.Book.Copies * cb.Book.Price)
                })
                .OrderByDescending(cp => cp.Profit)
                .ThenBy(cp => cp.CategoryName)
                .ToList();

            foreach (var cp in categoryProfit)
            {
                sb.AppendLine($"{cp.CategoryName} ${cp.Profit:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetMostRecentBooks(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var categoryBooks = context.Categories
                .Select(c => new
                {
                    Name = c.Name,
                    Books = c.CategoryBooks
                        .OrderByDescending(cb => cb.Book.ReleaseDate)
                        .Select(cb => new
                        {
                            Title = cb.Book.Title,
                            Date = cb.Book.ReleaseDate.Value.Year
                        })
                        .Take(3)
                        .ToList()
                })
                .ToList();

            foreach (var cb in categoryBooks.OrderBy(cb => cb.Name))
            {
                sb.AppendLine($"--{cb.Name}");

                foreach (var b in cb.Books)
                {
                    sb.AppendLine($"{b.Title} ({b.Date})");
                }
            }

            return sb.ToString().TrimEnd();
        }

        public static void IncreasePrices(BookShopContext context)
        {
            var books = context.Books
                .Where(b => b.ReleaseDate.Value.Year < 2015);

            foreach (var b in books)
            {
                b.Price += 5;
            }

            context.SaveChanges();
        }

        public static int RemoveBooks(BookShopContext context)
        {
            var books = context.Books
                .Where(b => b.Copies < 4200)
                .ToList();

            context.Books.RemoveRange(books);
            
            context.SaveChanges();

            return books.Count;
        }
    }
}
