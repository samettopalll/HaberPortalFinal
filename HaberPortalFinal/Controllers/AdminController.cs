using AspNetCoreHero.ToastNotification.Abstractions;
using HaberPortalFinal.Models;
using HaberPortalFinal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

namespace HaberPortalFinal.Controllers
{
    [Authorize(Roles = "admin")]

    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IFileProvider _fileProvider;
        private readonly INotyfService _notify;

        public AdminController(AppDbContext context, IFileProvider fileProvider, INotyfService notify)
        {
            _context = context;
            _fileProvider = fileProvider;
            _notify = notify;
        }
        public IActionResult Index()
        {
            return View();
        }


        [Authorize(Roles = "admin")]
        public async Task<IActionResult> List()
        {

            return _context.News != null ?
                        View(await _context.News.ToListAsync()) :
                        Problem("Entity set 'AppDbContext.News'  is null.");
        }

        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(NewsModel newNews)
        {
            var rootFolder = _fileProvider.GetDirectoryContents("wwwroot");
            var photoUrl = "-";
            if (newNews.PhotoFile != null)
            {
                var filename = Path.GetFileName(newNews.PhotoFile.FileName);
                var photoPath = Path.Combine(rootFolder.First(x => x.Name == "newsPhotos").PhysicalPath, filename);
                using var stream = new FileStream(photoPath, FileMode.Create);
                newNews.PhotoFile.CopyTo(stream);
                photoUrl = filename;
            }

            var news = new News
            {
                Title = newNews.Title,
                Content = newNews.Content,
                Description = newNews.Description,
                Tag = newNews.Tag,
                PhotoUrl = photoUrl
            };

            _context.News.Add(news);
            _context.SaveChanges();
            _notify.Success("Haber Başarıyla Eklendi.");
            return View(newNews);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.News == null)
            {
                return NotFound();
            }

            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                return NotFound();
            }

            NewsModel newNews = new()
            {
                Id = news.Id,
                Content = news.Content,
                Tag = news.Tag,
                Description = news.Description,
                Title = news.Title,
                PhotoFile = null
            };

            return View(newNews);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(NewsModel newNews)
        {
            try
            {
                var rootFolder = _fileProvider.GetDirectoryContents("wwwroot");
                var photoUrl = "-";

                if (newNews.PhotoFile != null)
                {
                    var filename = Path.GetFileName(newNews.PhotoFile.FileName);
                    var photoPath = Path.Combine(rootFolder.First(x => x.Name == "newsPhotos").PhysicalPath, filename);
                    using var stream = new FileStream(photoPath, FileMode.Create);
                    newNews.PhotoFile.CopyTo(stream);
                    photoUrl = filename;
                }

                var news = _context.News.FirstOrDefault(n => n.Id == newNews.Id);
                news.Title = newNews.Title;
                news.Content = newNews.Content;
                news.Description = newNews.Description;
                news.Tag = newNews.Tag;
                news.PhotoUrl = photoUrl;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NewsExists(newNews.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.News == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .FirstOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.News == null)
            {
                return Problem("Entity set 'AppDbContext.News' is null.");
            }

            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                return Problem("Data not found.");
            }

            var rootFolder = _fileProvider.GetDirectoryContents("wwwroot");
            if (news.PhotoUrl != null && news.PhotoUrl.Length > 1)
            {
                var filename = Path.GetFileName(news.PhotoUrl);
                var photoPath = Path.Combine(rootFolder.First(x => x.Name == "newsPhotos").PhysicalPath, filename);
                System.IO.File.Delete(photoPath);
            }

            _context.News.Remove(news);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.News == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .FirstOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        private bool NewsExists(int id)
        {
            return (_context.News?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

