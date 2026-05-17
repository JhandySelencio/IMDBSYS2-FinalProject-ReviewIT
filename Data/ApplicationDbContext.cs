using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReviewIT_FinalProject_IMDBSYS2.Models;

namespace ReviewIT_FinalProject_IMDBSYS2.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<Choice> Choices { get; set; }

        public virtual DbSet<Question> Questions { get; set; }

        public virtual DbSet<QuestionType> QuestionTypes { get; set; }

        public virtual DbSet<Quiz> Quizzes { get; set; }

        public virtual DbSet<Status> Statuses { get; set; }
    }
}
