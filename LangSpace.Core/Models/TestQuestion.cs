using System;
using System.Collections.Generic;

namespace LangSpace.Models;

public partial class TestQuestion
{
    public int IdQuestion { get; set; }

    public string QuestionText { get; set; } = null!;

    public string OptionA { get; set; } = null!;

    public string OptionB { get; set; } = null!;

    public string OptionC { get; set; } = null!;

    public string OptionD { get; set; } = null!;

    public char CorrectOption { get; set; }

    public int? IdLevel { get; set; }

    public string QuestionType { get; set; } = null!;

    public virtual Level? IdLevelNavigation { get; set; }
}
