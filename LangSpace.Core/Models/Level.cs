using System;
using System.Collections.Generic;

namespace LangSpace.Models;

public partial class Level
{
    public int IdLevel { get; set; }

    public string LevelName { get; set; } = null!;

    public virtual ICollection<Card> Cards { get; set; } = new List<Card>();

    public virtual ICollection<GrammarRule> GrammarRules { get; set; } = new List<GrammarRule>();

    public virtual ICollection<TestQuestion> TestQuestions { get; set; } = new List<TestQuestion>();

    public virtual ICollection<UserLanguage> UserLanguages { get; set; } = new List<UserLanguage>();
}
