using System;
using System.Collections.Generic;

namespace LangSpace.Models;

public partial class GrammarQuestion
{
    public int IdGrammarQuestion { get; set; }
    public string QuestionText { get; set; } = null!;
    public string Option1 { get; set; } = null!;
    public string Option2 { get; set; } = null!;
    public string Option3 { get; set; } = null!;
    public string Option4 { get; set; } = null!;
    public string CorrectAnswer { get; set; } = null!;
    public int IdGrammarRule { get; set; }

    public virtual GrammarRule IdGrammarRuleNavigation { get; set; } = null!;
}