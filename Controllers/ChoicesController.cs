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
    public class ChoicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChoicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Choices
        public async Task<IActionResult> Index(int? questionId)
        {
            if (questionId != null)
            {
                var question = await _context.Questions
                    .FirstOrDefaultAsync(q => q.QuestionId == questionId);

                if (question != null)
                {
                    ViewBag.QuestionText = question.QuestionText;  
                    ViewBag.QuestionId = question.QuestionId;
                    ViewBag.QuizId = question.QuizId;
                }

                var filterChoices = _context.Choices
                    .Include(c => c.Question)
                    .Where(c => c.QuestionId == questionId);

                return View(await filterChoices.ToListAsync());
            }

            var applicationDbContext = _context.Choices.Include(c => c.Question);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Choices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var choice = await _context.Choices.FindAsync(id);
            if (choice == null)
            {
                return NotFound();
            }
            ViewBag.QuestionId = choice.QuestionId;
            return View(choice);
        }

        // POST: Choices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ChoiceId,ChoiceText")] Choice choice)
        {
            if (id != choice.ChoiceId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existChoice = await _context.Choices.FindAsync(id);
                    if(existChoice == null)
                    {
                        return NotFound();
                    }

                    existChoice.ChoiceText = choice.ChoiceText;

                    var parentQuestion = await _context.Questions.FindAsync(existChoice.QuestionId);
                    if(parentQuestion != null)
                    {
                        parentQuestion.StatusId = 1;
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index), new { questionId = existChoice.QuestionId });

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChoiceExists(choice.ChoiceId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewBag.QuestionId = choice.QuestionId;
            return View(choice);
        }
        private bool ChoiceExists(int id)
        {
            return _context.Choices.Any(e => e.ChoiceId == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetAsCorrect(int id)
        {
            var targetChoice = await _context.Choices
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ChoiceId == id);

            if (targetChoice == null)
            {
                return NotFound();
            }

            var allChoices = await _context.Choices
                .Where(c => c.QuestionId == targetChoice.QuestionId)
                .ToListAsync();

            foreach (var choice in allChoices)
            {
                choice.IsCorrect = (choice.ChoiceId == id);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { questionId = targetChoice.QuestionId });
        }
    }
}
