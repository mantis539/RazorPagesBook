using Microsoft.Build.Tasks;
using Microsoft.EntityFrameworkCore;
using RazorPagesBook.Data;
namespace RazorPagesBook.Models;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var context = new RazorPagesBookContext(
        serviceProvider.GetRequiredService<
        DbContextOptions<RazorPagesBookContext>>()))
        {
            if (context == null || context.Book == null)
            {
                throw new ArgumentNullException("Null RazorPagesBookContext");
            }
            
            if (context.Book.Any())
            {
                return; 
            }
            context.Book.AddRange(
            new Book
            {
                Title = "Project Hail Mary",
                Author = "Andy Weir",
                Genre = "Hard Science Fiction",
                Price = 15,
                Rating = 4.4
            },
            new Book
            {
                Title = "Flowers for Argernon",
                Author = "Daniel Kayes",
                Genre = "Psyhological Drama",
                Price = 19,
                Rating = 4.7
            },
            new Book
            {
                Title = "A Man Called Ove",
                Author = "Fredrik Backman",
                Genre = "Contemporary Fiction",
                Price = 25,
                Rating = 4.2
            },
            new Book
            {
                Title = "Dune",
                Author = "Frank Herbert",
                Genre = "Epic Space Opera",
                Price = 15,
                Rating = 4.8
            }
            );
            context.SaveChanges();
        }
    }
}
