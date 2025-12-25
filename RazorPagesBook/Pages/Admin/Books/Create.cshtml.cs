using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Tasks;
using RazorPagesBook.Data;
using RazorPagesBook.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
namespace RazorPagesBook.Pages.Admin.Books
{
    public class CreateModel : PageModel
    {
        private readonly RazorPagesBook.Data.RazorPagesBookContext _context;
        private readonly IWebHostEnvironment _env;
        public CreateModel(RazorPagesBookContext context, IWebHostEnvironment env) 

        {
            _context = context;
            _env = env;
        }
        public IActionResult OnGet()
        {

            return Page();
        }
        [BindProperty]
        public Book Book { get; set; } = default!;
        [BindProperty]
        public IFormFile? CoverFile { get; set; }
        private static readonly string[] _permitted = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long _maxSize = 2 * 1024 * 1024;
        
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            _context.Book.Add(Book);
            await _context.SaveChangesAsync();
            
            if (CoverFile is not null && CoverFile.Length > 0)
            {
                try
                {
                    Book.CoverPath = await SavePosterAsync(CoverFile, Book.Id);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    
                    ModelState.AddModelError(string.Empty, $"Помилка завантаження постера: { ex.Message}");
                return Page();
                }
            }
            return RedirectToPage("./Index");
        }
        private async Task<string> SavePosterAsync(IFormFile file, int bookId)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_permitted.Contains(ext))
                throw new InvalidOperationException("Неприпустимий формат зображення.");
        if (file.Length <= 0 || file.Length > _maxSize)
                throw new InvalidOperationException("Файл порожній або перевищує 2 МБ.");
                
                var destDir = Path.Combine(_env.WebRootPath, "uploads", "books",
                bookId.ToString());
            Directory.CreateDirectory(destDir);
            var fileName = $"cover-{Guid.NewGuid():N}{ext}";
            var filePath = Path.Combine(destDir, fileName);
            using (var stream = System.IO.File.Create(filePath))
                await file.CopyToAsync(stream);
            
            return $"/uploads/books/{bookId}/{fileName}";
        }
    }
}
