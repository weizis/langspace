using System;
using System.Collections.Generic;

namespace LangSpace.Models;

public partial class GrammarRule
{
    public int IdRule { get; set; }

    public string RuleName { get; set; } = null!;

    public string RuleText { get; set; } = null!;

    public int IdLevel { get; set; }

    public int IdLanguage { get; set; }

    public virtual Language IdLanguageNavigation { get; set; } = null!;

    public virtual Level IdLevelNavigation { get; set; } = null!;

    public virtual ICollection<GrammarQuestion> GrammarQuestions { get; set; } = new List<GrammarQuestion>();

}
