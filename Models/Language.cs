using System;
using System.Collections.Generic;

namespace LangSpace.Models;

public partial class Language
{
    public int IdLanguage { get; set; }

    public string LanguageName { get; set; } = null!;

    public virtual ICollection<Card> Cards { get; set; } = new List<Card>();

    public virtual ICollection<GrammarRule> GrammarRules { get; set; } = new List<GrammarRule>();

    public virtual ICollection<UserLanguage> UserLanguages { get; set; } = new List<UserLanguage>();
}
