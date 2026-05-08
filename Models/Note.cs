using System;
using System.Collections.Generic;

namespace LangSpace.Models;

public partial class Note
{
    public int IdNote { get; set; }

    public string NoteText { get; set; } = null!;

    public int IdUser { get; set; }

    public DateOnly DateCreated { get; set; }

    public virtual User IdUserNavigation { get; set; } = null!;

    public virtual ICollection<Resource> Resources { get; set; } = new List<Resource>();
    public bool HasFile => Resources != null && Resources.Any() &&
                          Resources.FirstOrDefault()?.ResourceType != "Текстовая заметка";
}
