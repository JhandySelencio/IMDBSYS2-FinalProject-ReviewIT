using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ReviewIT_FinalProject_IMDBSYS2.Models;

public partial class Question
{
    public int QuestionId { get; set; }

    public int QuizId { get; set; }

    public string QuestionText { get; set; } = null!;

    public int StatusId { get; set; }

    public int QuestionTypeId { get; set; }

    public virtual ICollection<Choice> Choices { get; set; } = new List<Choice>();
    [ValidateNever]
    public virtual QuestionType QuestionType { get; set; } = null!;
    [ValidateNever]
    public virtual Quiz Quiz { get; set; } = null!;
    [ValidateNever]
    public virtual Status Status { get; set; } = null!;
}
