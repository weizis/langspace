using System;
using System.Collections.Generic;

namespace LangSpace.Models;

public partial class UserLanguage
{
    public int IdUser { get; set; }

    public int IdLanguage { get; set; }

    public int? IdLevel { get; set; }

    public virtual Language IdLanguageNavigation { get; set; } = null!;

    public virtual Level? IdLevelNavigation { get; set; }

    public virtual User IdUserNavigation { get; set; } = null!;
}
