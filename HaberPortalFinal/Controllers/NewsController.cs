using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using AspNetCoreHero.ToastNotification.Abstractions;
using HaberPortalFinal.Models;
using HaberPortalFinal.ViewModels;

namespace HaberPortal.Controllers


{

    public class NewsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IFileProvider _fileProvider;
        private readonly INotyfService _notify;

        public NewsController(AppDbContext context, IFileProvider fileProvider, INotyfService notify)
        {
            _context = context;
            _fileProvider = fileProvider;
            _notify = notify;
        }


        public async Task<IActionResult> Scan(string searchString)
        {
            if (_context.News == null)
            {
                return Problem("News is null.");
            }

            var newsQuery = from n in _context.News
                            select n;

            if (!String.IsNullOrEmpty(searchString))
            {
                newsQuery = newsQuery.Where(s => s.Title!.Contains(searchString));
            }

            var sortedNews = await newsQuery.OrderByDescending(n => n.Id).ToListAsync();

            return View(sortedNews);
        }

        public async Task<IActionResult> Economy()
        {
            return _context.News != null ?
                        View(await _context.News.ToListAsync()) :
                        Problem("Entity set 'AppDbContext.News'  is null.");
        }



        public async Task<IActionResult> Politics()
        {
            return _context.News != null ?
                        View(await _context.News.ToListAsync()) :
                        Problem("Entity set 'AppDbContext.News'  is null.");
        }



        public async Task<IActionResult> Sport()
        {
            return _context.News != null ?
                        View(await _context.News.ToListAsync()) :
                        Problem("Entity set 'AppDbContext.News'  is null.");
        }


        public async Task<IActionResult> Global()
        {
            return _context.News != null ?
                        View(await _context.News.ToListAsync()) :
                        Problem("Entity set 'AppDbContext.News'  is null.");
        }

        public async Task<IActionResult> Nation()
        {
            return _context.News != null ?
                        View(await _context.News.ToListAsync()) :
                        Problem("Entity set 'AppDbContext.News'  is null.");
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

        
    }
}
