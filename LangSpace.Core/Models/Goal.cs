using System;
using System.Collections.Generic;

namespace LangSpace.Models;

public partial class Goal
{
    public int IdGoal { get; set; }

    public int IdUser { get; set; }
    public int CardsPerDay { get; set; } = 20;  
    public int TestsPerDay { get; set; } = 1;     
    public int DaysStreak { get; set; } = 7;

    public virtual User IdUserNavigation { get; set; } = null!;
}
