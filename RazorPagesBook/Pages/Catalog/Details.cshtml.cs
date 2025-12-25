using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesBook.Models;
using RazorPagesBook.Data;
using System.Security.Claims;

namespace RazorPagesBook.Pages.Catalog
{
    [AllowAnonymous]
    public class DetailsModel : PageModel
    {
        private readonly RazorPagesBookContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DetailsModel(RazorPagesBookContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Book? Book { get; private set; }
        public IList<Review> Reviews { get; private set; } = new List<Review>();
        public double AverageRating { get; private set; }
        public int ReviewsCount => Reviews.Count;

        [BindProperty(SupportsGet = true)]
        public int id { get; set; }

        [BindProperty]
        public int? MyRating { get; set; }

        [BindProperty]
        public string? MyComment { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            this.id = id;
            Book = await _context.Book.AsNoTracking()
                .Include(m => m.Reviews)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Book == null) return NotFound();

            Reviews = Book.Reviews
                .OrderByDescending(r => r.UpdatedAt ?? r.CreatedAt)
                .ToList();

            AverageRating = Reviews.Count > 0 ? Reviews.Average(r => r.Rating) : 0;

            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = _userManager.GetUserId(User);
                var mine = Reviews.FirstOrDefault(r => r.UserId == userId);
                if (mine != null)
                {
                    MyRating = mine.Rating;
                    MyComment = mine.Comment;
                }
            }
            return Page();
        }

        
        public async Task<IActionResult> OnPostSaveReviewAsync()
        {
            if (!User.Identity?.IsAuthenticated ?? false) return Challenge();
            if (id <= 0) return NotFound();

            if (MyRating is null || MyRating < 1 || MyRating > 5)
            {
                ModelState.AddModelError(nameof(MyRating), "Оцінка повинна бути від 1 до 5.");
                await OnGetAsync(id);
                return Page();
            }

            var book = await _context.Book.FirstOrDefaultAsync(m => m.Id == id);
            if (book == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var userId = user.Id;
            var displayName = user.Email ?? user.UserName ?? "user";

            var existing = await _context.Reviews.FirstOrDefaultAsync(r => r.BookId == id && r.UserId == userId);

            if (existing == null)
            {
                _context.Reviews.Add(new Review
                {
                    BookId = id,
                    UserId = userId,
                    ReviewerName = displayName,
                    Rating = MyRating.Value,
                    Comment = string.IsNullOrWhiteSpace(MyComment) ? null : MyComment.Trim(),
                    CreatedAt = DateTime.UtcNow
                });
            }
            else
            {
                existing.Rating = MyRating.Value;
                existing.Comment = string.IsNullOrWhiteSpace(MyComment) ? null : MyComment.Trim();
                existing.ReviewerName = displayName;
                existing.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("./Details", new { id });
        }

        // Авторизація перевіряється всередині методу
        public async Task<IActionResult> OnPostDeleteReviewAsync(int reviewId)
        {
            if (!User.Identity?.IsAuthenticated ?? false) return Challenge();

            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId);
            if (review == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");

            if (review.UserId != userId && !isAdmin)
            {
                return Forbid();
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Details", new { id = review.BookId });
        }
    }
}
