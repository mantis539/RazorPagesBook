using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesBook.Models;
using RazorPagesBook.Data;
using System.ComponentModel.DataAnnotations;


namespace RazorPagesMovie.Pages.Admin.Reviews
{
    public class ReplyModel : PageModel
    {
        private readonly RazorPagesBookContext _context;
        private readonly UserManager<IdentityUser> _userManager;


        public ReplyModel(RazorPagesBookContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public Review? Review { get; private set; }
        public string BookTitle { get; private set; } = string.Empty;


        [BindProperty(SupportsGet = true)]
        public int id { get; set; }


        public class ReplyInput
        {
            [Display(Name = "Відповідь адміністратора")]
            [StringLength(2000)]
            public string? AdminReply { get; set; }
        }


        [BindProperty]
        public ReplyInput Input { get; set; } = new();


        public async Task<IActionResult> OnGetAsync(int id)
        {
            this.id = id;
            Review = await _context.Reviews
                .Include(r => r.Book)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (Review == null) return NotFound();


            BookTitle = Review.Book?.Title ?? "";
            Input.AdminReply = Review.AdminReply;
            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            var review = await _context.Reviews.Include(r => r.Book).FirstOrDefaultAsync(r => r.Id == id);
            if (review == null) return NotFound();


            var text = string.IsNullOrWhiteSpace(Input.AdminReply) ? null : Input.AdminReply!.Trim();


            var admin = await _userManager.GetUserAsync(User);
            var adminName = admin?.Email ?? admin?.UserName ?? "admin";


            review.AdminReply = text;
            review.AdminReplyBy = text == null ? null : adminName;
            review.AdminRepliedAt = text == null ? (DateTime?)null : DateTime.UtcNow;


            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}

