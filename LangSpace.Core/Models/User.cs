using System;
using System.Collections.Generic;


namespace LangSpace.Models;

public partial class User
{
    public int IdUser { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;
    public string? FirstName { get; set; } = null!;  

    public string? LastName { get; set; } = null!;
    public string? Avatar { get; set; }

    public DateOnly RegistrationDate { get; set; }

    public int IdRole { get; set; }
    public string AvatarPath
    {
        get
        {
            if (string.IsNullOrEmpty(Avatar))
                return "/Images/user.png";
            return $"/Images/{Avatar}";
        }
    }



    public virtual ICollection<Goal> Goals { get; set; } = new List<Goal>();


    public virtual Role IdRoleNavigation { get; set; } = null!;

    public virtual ICollection<Note> Notes { get; set; } = new List<Note>();


  
    public virtual ICollection<UserLanguage> UserLanguages { get; set; } = new List<UserLanguage>();
    public virtual ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();
    public bool IsTeacher => UserSession.CurrentUser?.IdRole == 2;
}
