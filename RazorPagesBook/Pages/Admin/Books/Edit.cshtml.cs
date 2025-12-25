using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Tasks;
using Microsoft.EntityFrameworkCore;
using RazorPagesBook.Data;
using RazorPagesBook.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPagesBook.Pages.Admin.Books
{
    public class EditModel : PageModel
    {
        private readonly RazorPagesBook.Data.RazorPagesBookContext _context;
        private readonly IWebHostEnvironment _env;

        public EditModel(RazorPagesBook.Data.RazorPagesBookContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [BindProperty]
        public Book Book { get; set; } = default!;

        [BindProperty]
        public IFormFile? CoverFile { get; set; }
        private static readonly string[] _permitted = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long _maxSize = 2 * 1024 * 1024;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book =  await _context.Book.FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }
            Book = book;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var dbBook = await _context.Book.FirstOrDefaultAsync(m => m.Id == Book.Id);
            if (dbBook == null) return NotFound();
            dbBook.Title = Book.Title;
            dbBook.Author = Book.Author;
            dbBook.Genre = Book.Genre;
            dbBook.Price = Book.Price;
            dbBook.Rating = Book.Rating;
            if (CoverFile is not null && CoverFile.Length > 0)
            {
                var newPath = await SaveCoverAsync(CoverFile, dbBook.Id);
                TryDeleteFile(dbBook.CoverPath);
                dbBook.CoverPath = newPath;
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(Book.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool BookExists(int id)
        {
            return _context.Book.Any(e => e.Id == id);
        }
        private async Task<string> SaveCoverAsync(IFormFile file, int bookId)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_permitted.Contains(ext))
                throw new InvalidOperationException("Неприпустимий формат зображення.");
            if (file.Length <= 0 || file.Length > _maxSize)
                throw new InvalidOperationException("Файл порожній або перевищує 2 МБ.");
            var destDir = Path.Combine(_env.WebRootPath, "uploads", "books",
            bookId.ToString());
            Directory.CreateDirectory(destDir);
            var fileName = $"poster-{Guid.NewGuid():N}{ext}";
            var filePath = Path.Combine(destDir, fileName);
            using (var stream = System.IO.File.Create(filePath))
                await file.CopyToAsync(stream);
            return $"/uploads/books/{bookId}/{fileName}";
        }
        private void TryDeleteFile(string? webPath)
        {
            if (string.IsNullOrWhiteSpace(webPath)) return;
            var fullPath = Path.Combine(_env.WebRootPath,
            webPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (System.IO.File.Exists(fullPath))
            {
                try { System.IO.File.Delete(fullPath); } catch { /* ігноруємо */ }
            }
        }
    }
}
