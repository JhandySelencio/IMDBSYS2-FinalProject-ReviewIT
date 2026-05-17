using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.XPath;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReviewIT_FinalProject_IMDBSYS2.Data;
using ReviewIT_FinalProject_IMDBSYS2.Models;

namespace ReviewIT_FinalProject_IMDBSYS2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class QuestionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QuestionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Questions
        public async Task<IActionResult> Index(int? quizId)
        {
            if(quizId == null)
            {
                return RedirectToAction("Index", "Quizzes");
            }

            var quiz = await _context.Quizzes
                .Include(q => q.Category)
                .FirstOrDefaultAsync(q => q.QuizId == quizId);

            if (quiz == null)
            {
                return RedirectToAction("Index", "Quizzes");
            }

            ViewBag.QuizTitle = quiz.Title;
            ViewBag.CategoryType = quiz.Category.CategoryType   ;
            ViewBag.QuizId = quiz.QuizId;

            var questions = await _context.Questions
              .Include(q => q.Quiz)
              .Include(q => q.Status)
              .Include(q => q.QuestionType)
              .Where(q => q.QuizId == quizId)
              .ToListAsync();

            return View(questions);
        }

        // GET: Questions/Create
        public IActionResult Create(int quizId)
        {
            ViewBag.QuizId = quizId;
            ViewData["QuestionTypeId"] = new SelectList(_context.QuestionTypes, "QuestionTypeId", "QuestionTypeName");
            return View();
        }

        // POST: Questions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int quizId, [Bind("QuestionText,QuestionTypeId")] Question question)
        {
            question.QuizId = quizId;
            question.StatusId = 3;

            if (ModelState.IsValid)
            {
                _context.Add(question);
                await _context.SaveChangesAsync();

                //Automatically adds choices base on the question type
                var choices = new List<Choice>();
                switch (question.QuestionTypeId)
                {
                    case 1:
                        choices.Add(new Choice { QuestionId = question.QuestionId, ChoiceText = "[Insert Answer]", IsCorrect = true });
                        break;
                    case 2:
                        choices.Add(new Choice { QuestionId = question.QuestionId, ChoiceText = "[Insert A]", IsCorrect = false });
                        choices.Add(new Choice { QuestionId = question.QuestionId, ChoiceText = "[Insert B]", IsCorrect = false });
                        choices.Add(new Choice { QuestionId = question.QuestionId, ChoiceText = "[Insert C]", IsCorrect = false });
                        choices.Add(new Choice { QuestionId = question.QuestionId, ChoiceText = "[Insert D]", IsCorrect = false });
                        break;
                    case 3:
                        choices.Add(new Choice { QuestionId = question.QuestionId, ChoiceText = "True", IsCorrect = true });
                        choices.Add(new Choice { QuestionId = question.QuestionId, ChoiceText = "False", IsCorrect = false });
                        question.StatusId = 1;
                        break;
                }

                if (choices.Any())
                {
                    _context.AddRange(choices);
                }
                var parentQuiz = await _context.Quizzes.FindAsync(quizId);

                if(parentQuiz != null)
                {
                    parentQuiz.StatusId = 1;
                    parentQuiz.UpdatedAt = DateOnly.FromDateTime(DateTime.Now);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { quizId = quizId });
            }

            ViewBag.QuizId = quizId;    
            ViewData["QuestionTypeId"] = new SelectList(_context.QuestionTypes, "QuestionTypeId", "QuestionTypeName", question.QuestionTypeId);
            return View(question);
        }

        // GET: Questions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await _context.Questions.FindAsync(id);
            if (question == null)
            {
                return NotFound();
            }
            ViewBag.QuizId = question.QuizId;
            ViewData["StatusId"] = new SelectList(_context.Statuses, "StatusId", "StatusType", question.StatusId);
            return View(question);
        }

        // POST: Questions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("QuizId,QuestionId,QuestionText,StatusId")] Question question)
        {
            if (id != question.QuestionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existQuestion = await _context.Questions.FindAsync(id);
                    if (existQuestion == null)
                    {
                        return NotFound();
                    }

                    existQuestion.QuestionText = question.QuestionText;
                    if (existQuestion.StatusId != 3)
                    {
                        existQuestion.StatusId = question.StatusId;
                    }


                        await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index), new { quizId = question.QuizId });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionExists(question.QuestionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewData["StatusId"] = new SelectList(_context.Statuses, "StatusId", "StatusId", question.StatusId);
            return View(question);
        }

        // GET: Questions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await _context.Questions
                .Include(q => q.QuestionType)
                .Include(q => q.Quiz)
                .Include(q => q.Status)
                .FirstOrDefaultAsync(m => m.QuestionId == id);
            if (question == null)
            {
                return NotFound();
            }
            ViewBag.QuizId = question.QuizId;
            return View(question);
        }

        // POST: Questions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question != null)
            {
                question.StatusId = 2;

                _context.Questions.Update(question);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index), new { quizId = question.QuizId });
        }

        private bool QuestionExists(int id)
        {
            return _context.Questions.Any(e => e.QuestionId == id);
        }
    }
}
