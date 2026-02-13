using Microsoft.EntityFrameworkCore;
using Red_Code.Domain.Entities;

namespace Red_Code.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // Ensure database is created
            await context.Database.MigrateAsync();

            // Check if data already exists
            if (await context.Books.AnyAsync() || await context.Quotations.AnyAsync())
            {
                return; // Database has been seeded
            }

            // Seed Books
            var books = new List<Book>
            {
                new Book
                {
                    Title = "Clean Code: A Handbook of Agile Software Craftsmanship",
                    Author = "Robert C. Martin"
                },
                new Book
                {
                    Title = "The Pragmatic Programmer: Your Journey to Mastery",
                    Author = "Andrew Hunt and David Thomas"
                },
                new Book
                {
                    Title = "Design Patterns: Elements of Reusable Object-Oriented Software",
                    Author = "Erich Gamma, Richard Helm, Ralph Johnson, John Vlissides"
                },
                new Book
                {
                    Title = "Refactoring: Improving the Design of Existing Code",
                    Author = "Martin Fowler"
                },
                new Book
                {
                    Title = "Domain-Driven Design: Tackling Complexity in the Heart of Software",
                    Author = "Eric Evans"
                }
            };

            await context.Books.AddRangeAsync(books);

            // Seed Quotations
            var quotations = new List<Quotation>
            {
                new Quotation
                {
                    QuotationText = "Any fool can write code that a computer can understand. Good programmers write code that humans can understand.",
                    Author = "Martin Fowler"
                },
                new Quotation
                {
                    QuotationText = "First, solve the problem. Then, write the code.",
                    Author = "John Johnson"
                },
                new Quotation
                {
                    QuotationText = "Code is like humor. When you have to explain it, it's bad.",
                    Author = "Cory House"
                },
                new Quotation
                {
                    QuotationText = "Clean code always looks like it was written by someone who cares.",
                    Author = "Robert C. Martin"
                },
                new Quotation
                {
                    QuotationText = "The best error message is the one that never shows up.",
                    Author = "Thomas Fuchs"
                }
            };

            await context.Quotations.AddRangeAsync(quotations);

            // Save all changes
            await context.SaveChangesAsync();
        }
    }
}
