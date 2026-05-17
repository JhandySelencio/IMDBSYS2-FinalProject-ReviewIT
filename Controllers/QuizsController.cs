using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReviewIT_FinalProject_IMDBSYS2.Data;
using ReviewIT_FinalProject_IMDBSYS2.Models;

namespace ReviewIT_FinalProject_IMDBSYS2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class QuizsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QuizsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Quizs
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Quizzes
                .Include(q => q.Category)
                .Include(q => q.Status);

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Quizs/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryType");
            ViewData["StatusId"] = new SelectList(_context.Statuses, "StatusId", "StatusType");
            return View();
        }

        // POST: Quizs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,CategoryId")] Quiz quiz)
        {
            if (ModelState.IsValid)
            {
                quiz.CreatedAt = DateOnly.FromDateTime(DateTime.Now);
                quiz.UpdatedAt = DateOnly.FromDateTime(DateTime.Now);
                quiz.HeartRatio = 0;
                quiz.StatusId = 3;

                _context.Add(quiz);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryType", quiz.CategoryId);
            return View(quiz);
        }

        // GET: Quizs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryType", quiz.CategoryId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "StatusId", "StatusType", quiz.StatusId);
            return View(quiz);
        }

        // POST: Quizs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("QuizId,Title,StatusId,CategoryId")] Quiz quiz)
        {
            if (id != quiz.QuizId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existQuiz = await _context.Quizzes.FindAsync(id);

                    if (existQuiz == null)
                    {
                        return NotFound();  
                    }

                    existQuiz.Title = quiz.Title;
                    existQuiz.StatusId = quiz.StatusId;
                    existQuiz.CategoryId = quiz.CategoryId;
                    existQuiz.UpdatedAt = DateOnly.FromDateTime(DateTime.Now);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuizExists(quiz.QuizId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", quiz.CategoryId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "StatusId", "StatusId", quiz.StatusId);
            return View(quiz);
        }

        // GET: Quizs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quiz = await _context.Quizzes
                .Include(q => q.Category)
                .FirstOrDefaultAsync(m => m.QuizId == id);
            if (quiz == null)
            {
                return NotFound();
            }

            return View(quiz);
        }

        // POST: Quizs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz != null)
            {
                quiz.StatusId = 2;

                _context.Quizzes.Update(quiz);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool QuizExists(int id)
        {
            return _context.Quizzes.Any(e => e.QuizId == id);
        }
    }
}
