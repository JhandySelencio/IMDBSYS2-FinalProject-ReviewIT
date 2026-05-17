using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ReviewIT_FinalProject_IMDBSYS2.Models;

public partial class Choice
{
    public int ChoiceId { get; set; }

    public int QuestionId { get; set; }

    public string ChoiceText { get; set; } = null!;

    public bool IsCorrect { get; set; }
    [ValidateNever]
    public virtual Question Question { get; set; } = null!;
}
