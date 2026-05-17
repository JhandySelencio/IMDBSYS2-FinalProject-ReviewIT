using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReviewIT_FinalProject_IMDBSYS2.Data;

namespace ReviewIT_FinalProject_IMDBSYS2.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Home/Index
        public async Task<IActionResult> Index()
        {
            // Pulls top 3 published quizzes sorted by highest metrics
            var topQuizzes = await _context.Quizzes
                .Include(q => q.Category)
                .Include(q => q.Questions)
                .Where(q => q.StatusId == 1)
                .OrderByDescending(q => q.HeartRatio)
                .Take(3)
                .ToListAsync();

            return View(topQuizzes);
        }

        // GET: /Home/Browse?searchString=Arts
        public async Task<IActionResult> Browse(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var quizzesQuery = _context.Quizzes
                .Include(q => q.Category)
                .Include(q => q.Questions)
                .Where(q => q.StatusId == 1);

            if (!string.IsNullOrEmpty(searchString))
            {
                var cleanSearch = searchString.Trim().ToLower();
                quizzesQuery = quizzesQuery.Where(q =>
                    q.Title.ToLower().Contains(cleanSearch) ||
                    q.Category.CategoryType.ToLower().Contains(cleanSearch)
                );
            }

            var filteredQuizzes = await quizzesQuery.OrderByDescending(q => q.CreatedAt).ToListAsync();
            return View(filteredQuizzes);
        }
    }
}