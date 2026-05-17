using System;
using System.Collections.Generic;

namespace ReviewIT_FinalProject_IMDBSYS2.Models;

public partial class QuestionType
{
    public int QuestionTypeId { get; set; }

    public string QuestionTypeName { get; set; } = null!;

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
}
