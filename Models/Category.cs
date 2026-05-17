using System;
using System.Collections.Generic;

namespace ReviewIT_FinalProject_IMDBSYS2.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string CategoryType { get; set; } = null!;

    public virtual ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
}
