using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesBook.Data;
using RazorPagesBook.Models;
using System.ComponentModel.DataAnnotations;
using RazorPagesBook.Services;

namespace RazorPagesBook.Pages.Admin.Feedback
{
    public class ReplyModel : PageModel
    {
        private readonly RazorPagesBookContext _ctx;
        private readonly IEmailSender _email;
        private readonly UserManager<IdentityUser> _users;

        public ReplyModel(RazorPagesBookContext ctx, IEmailSender email, UserManager<IdentityUser> users)
        {
            _ctx = ctx;
            _email = email;
            _users = users;
        }

        public Models.Feedback? Item { get; private set; }

        [BindProperty(SupportsGet = true)]
        public int id { get; set; }

        public class ReplyInput
        {
            [Required, StringLength(200)]
            [Display(Name = "Тема листа")]
            public string Subject { get; set; } = string.Empty;

            [Required, StringLength(4000)]
            [Display(Name = "Текст відповіді")]
            public string Body { get; set; } = string.Empty;
        }

        [BindProperty]
        public ReplyInput Input { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            this.id = id;
            Item = await _ctx.Feedbacks.FirstOrDefaultAsync(f => f.Id == id);
            if (Item == null) return NotFound();

            Input.Subject = Item.AdminReplySubject ?? $"Re: {Item.Subject}";
            Input.Body = Item.AdminReplyBody ?? $"Вітаю, {Item.Name}!\n\n...";

            return Page();
        }

        public async Task<IActionResult> OnPostSendAsync()
        {
            var f = await _ctx.Feedbacks.FirstOrDefaultAsync(x => x.Id == id);
            if (f == null) return NotFound();
            if (!ModelState.IsValid) { Item = f; return Page(); }

            var html = $"<p>Вітаю, {System.Net.WebUtility.HtmlEncode(f.Name)}!</p>" +
                       $"<p>{Input.Body.Replace("\n", "<br/>")}</p>" +
                       $"<hr/><p>Тема звернення: <b>{System.Net.WebUtility.HtmlEncode(f.Subject)}</b></p>";

            await _email.SendAsync(f.Email, Input.Subject, html, Input.Body);

            var admin = await _users.GetUserAsync(User);
            f.AdminReplySubject = Input.Subject;
            f.AdminReplyBody = Input.Body;
            f.AdminRepliedBy = admin?.Email ?? admin?.UserName ?? "admin";
            f.AdminRepliedAt = DateTime.UtcNow;

            await _ctx.SaveChangesAsync();

            TempData["Ok"] = "Відповідь надіслано.";
            return RedirectToPage("./Index");
        }
    }
}

