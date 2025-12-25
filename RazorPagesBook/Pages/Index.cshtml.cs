using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesBook.Data;
using RazorPagesBook.Models;

namespace RazorPagesBook.Pages
{
    public class IndexModel : PageModel
    {
        private readonly RazorPagesBookContext _context;

        public IndexModel(RazorPagesBookContext context)
        {
            _context = context;
        }

        public IList<Book> NewestBooks { get; set; } = default!;
        public IList<Review> RecentReviews { get; set; } = default!;

        public async Task OnGetAsync()
        {
            NewestBooks = await _context.Book
                .OrderByDescending(b => b.Id)
                .Take(4)
                .ToListAsync();

            RecentReviews = await _context.Reviews
                .Include(r => r.Book)
                .OrderByDescending(r => r.CreatedAt)
                .Take(3)
                .ToListAsync();
        }
    }
}
