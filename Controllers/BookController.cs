using Microsoft.AspNetCore.Mvc;
using LibraryBookManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryBookManagement.Controllers
{
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString, string sortOrder)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentSort"] = sortOrder;

            var books = from b in _context.Books select b;

            if (!string.IsNullOrEmpty(searchString))
            {
                books = books.Where(b =>
                    b.Title.ToLower().Contains(searchString.ToLower()) ||
                    b.Author.ToLower().Contains(searchString.ToLower()));
            }

            books = sortOrder switch
            {
                "title" => books.OrderBy(b => b.Title),
                "author" => books.OrderBy(b => b.Author),
                _ => books.OrderByDescending(b => b.CreatedOn)
            };

            return View(await books.AsNoTracking().ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books.FirstOrDefaultAsync(m => m.Id == id);
            if (book == null) return NotFound();

            return View(book);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Author,ISBN,PublishedYear,Genre,Detail")] Book book)
        {
            if (ModelState.IsValid)
            {
                // 🔍 Check for duplicate title (case-insensitive)
                bool exists = await _context.Books
                    .AnyAsync(b => b.Title.ToLower() == book.Title.ToLower());

                if (exists)
                {
                    ModelState.AddModelError("Title", "A book with this title already exists.");
                    return View(book);
                }

                book.CreatedOn = DateTime.Now;
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Author,ISBN,PublishedYear,Genre,Detail,CreatedOn")] Book book)
        {
            if (id != book.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // 🔍 Check for duplicate title (case-insensitive) excluding current book
                    bool exists = await _context.Books
                        .AnyAsync(b => b.Title.ToLower() == book.Title.ToLower() && b.Id != book.Id);

                    if (exists)
                    {
                        ModelState.AddModelError("Title", "A book with this title already exists.");
                        return View(book);
                    }

                    book.UpdatedOn = DateTime.Now;
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Books.Any(e => e.Id == book.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books.FirstOrDefaultAsync(m => m.Id == id);
            if (book == null) return NotFound();

            return View(book);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
