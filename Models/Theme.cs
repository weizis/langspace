using System;
using System.Collections.Generic;

namespace LangSpace.Models;

public partial class Theme
{
    public int IdTheme { get; set; }

    public string ThemeName { get; set; } = null!;

    public virtual ICollection<Card> Cards { get; set; } = new List<Card>();
}
