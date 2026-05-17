using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ReviewIT_FinalProject_IMDBSYS2.Models;

public partial class Quiz
{
    public int QuizId { get; set; }

    public string Title { get; set; } = null!;

    public int StatusId { get; set; }

    public int CategoryId { get; set; }

    public DateOnly CreatedAt { get; set; }

    public DateOnly UpdatedAt { get; set; }

    public int HeartRatio { get; set; }
    [ValidateNever]
    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
    [ValidateNever]
    public virtual Status Status { get; set; } = null!;
}
