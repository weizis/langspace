using System;
using System.Collections.Generic;

namespace LangSpace.Models;

public partial class Resource
{
    public int IdResource { get; set; }

    public string ResourceType { get; set; } = null!;

    public string ResourceLink { get; set; } = null!;
    public string? ResourceText { get; set; }

    public int IdNote { get; set; }

    public virtual Note IdNoteNavigation { get; set; } = null!;
}
