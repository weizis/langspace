using System;

namespace LangSpace.Models;

public partial class TestResult
{
    public int IdTestResult { get; set; }
    public int IdUser { get; set; }
    public string TestType { get; set; } = null!; 
    public int Score { get; set; }
    public int Total { get; set; }
    public DateTime Date { get; set; }

    public virtual User IdUserNavigation { get; set; } = null!;
}