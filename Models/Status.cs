using System;
using System.Collections.Generic;

namespace ReviewIT_FinalProject_IMDBSYS2.Models;

public partial class Status
{
    public int StatusId { get; set; }

    public string StatusType { get; set; } = null!;

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

    public virtual ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
}
