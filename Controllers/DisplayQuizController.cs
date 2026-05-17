using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReviewIT_FinalProject_IMDBSYS2.Data;
using ReviewIT_FinalProject_IMDBSYS2.Models;

namespace ReviewIT_FinalProject_IMDBSYS2.Controllers
{
    [Authorize]
    public class DisplayQuizController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DisplayQuizController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int? quizId)
        {
            if (quizId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var quiz = await _context.Quizzes
                .Include(q => q.Category)
                .Include(q => q.Questions.Where(question => question.StatusId == 1)) 
                    .ThenInclude(question => question.Choices) 
                .FirstOrDefaultAsync(q => q.QuizId == quizId);

            if (quiz == null)
            {
                return NotFound();
            }

            ViewBag.QuizId = quizId;
            ViewBag.IsHearted = HttpContext.Session.GetString($"Hearted_{quizId}") == "true";
            return View(quiz);
        }

        // POST: /DisplayQuiz/ToggleHeart API Endpoint
        [HttpPost]
        public async Task<IActionResult> ToggleHeart(int quizId, bool isHearted)
        {
            var quiz = await _context.Quizzes.FindAsync(quizId);
            if (quiz == null)
            {
                return NotFound(new { success = false });
            }

            if (isHearted)
            {
                quiz.HeartRatio = Math.Max(0, quiz.HeartRatio - 1);
                HttpContext.Session.SetString($"Hearted_{quizId}", "false"); 
            }
            else
            {
                quiz.HeartRatio += 1;
                HttpContext.Session.SetString($"Hearted_{quizId}", "true"); 
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, newCount = quiz.HeartRatio });
        }

        // GET: /DisplayQuiz/Flashcards?quizId=5
        public async Task<IActionResult> Flashcards(int? quizId)
        {
            if (quizId == null)
            {
                return RedirectToAction("Index", "Home");
            }

                var quiz = await _context.Quizzes
                .Include(q => q.Questions.Where(question => question.StatusId == 1))
                    .ThenInclude(question => question.Choices)
                .FirstOrDefaultAsync(q => q.QuizId == quizId);

            if (quiz == null) return NotFound();

            if (!quiz.Questions.Any())
            {
                TempData["Message"] = "This quiz has no active questions to study yet.";
                return RedirectToAction("Index", new { quizId = quizId });
            }

            ViewBag.QuizId = quizId;
            return View(quiz);
        }

        // GET: /DisplayQuiz/TestQuiz?quizId=5
        public async Task<IActionResult> TestQuiz(int? quizId)
        {
            if (quizId == null) return RedirectToAction("Index", "Home");

            var quiz = await _context.Quizzes
                .Include(q => q.Category)
                .Include(q => q.Questions.Where(question => question.StatusId == 1))
                    .ThenInclude(question => question.Choices)
                .FirstOrDefaultAsync(q => q.QuizId == quizId);

            if (quiz == null) return NotFound();

            // Prevent users from taking an empty test
            if (!quiz.Questions.Any())
            {
                TempData["Message"] = "This quiz has no active questions yet.";
                return RedirectToAction("Index", new { quizId = quizId });
            }

            return View(quiz);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitTest(int QuizId, IFormCollection form)
        {
            // 1. Fetch the quiz and all the correct answers
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                    .ThenInclude(q => q.Choices)
                .FirstOrDefaultAsync(q => q.QuizId == QuizId);

            if (quiz == null) return NotFound();

            int score = 0;
            int totalQuestions = quiz.Questions.Count;

            // 2. Loop through every question to grade it
            foreach (var question in quiz.Questions)
            {
                // Find the correct choice from the database for this specific question
                var correctChoice = question.Choices.FirstOrDefault(c => c.IsCorrect == true);
                if (correctChoice == null) continue; // Skip if creator forgot to set a correct answer

                // Get what the user submitted from the HTML form
                string userResponse = form[$"question_{question.QuestionId}"].ToString();

                if (string.IsNullOrWhiteSpace(userResponse)) continue;

                // 3. Evaluate based on Question Type
                if (question.QuestionTypeId == 1)
                {
                    // IDENTIFICATION: Compare strings. We use Trim() and lowercasing 
                    // so if they type " Apple " instead of "apple", they still get it right.
                    if (userResponse.Trim().Equals(correctChoice.ChoiceText.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        score++;
                    }
                }
                else
                {
                    // MULTIPLE CHOICE & TRUE/FALSE: Compare Choice IDs
                    if (int.TryParse(userResponse, out int selectedChoiceId))
                    {
                        if (selectedChoiceId == correctChoice.ChoiceId)
                        {
                            score++;
                        }
                    }
                }
            }

            // 4. Send the final grade to a new Results view using ViewBag
            ViewBag.Score = score;
            ViewBag.Total = totalQuestions;
            ViewBag.QuizTitle = quiz.Title;
            ViewBag.QuizId = quiz.QuizId;

            return View("TestResult");
        }
    }
}