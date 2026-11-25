using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;


namespace LangSpace.Models;

public partial class Card : INotifyPropertyChanged
{
    private bool _showTranslation = false;
    [NotMapped]
    public bool ShowTranslation
    {
        get => _showTranslation;
        set
        {
            _showTranslation = value;
            OnPropertyChanged(); 
            OnPropertyChanged(nameof(DisplayText));
        }
    }
    [NotMapped]
    public bool IsOwner
    {
        get
        {
            var currentUser = UserSession.CurrentUser;
            if (currentUser == null) return false;
            if (IdUser == currentUser.IdUser) return true;
            if (currentUser.IdRole == 2) return true;

            return false;
        }
    }
    [NotMapped]
    public string DisplayText => ShowTranslation ? BackText : FrontText;
    public int IdCard { get; set; }

    public string FrontText { get; set; } = null!;

    public string BackText { get; set; } = null!;

    public bool IsPrivate { get; set; }

    public int IdTheme { get; set; }

    public int IdLevel { get; set; }

    public int IdLanguage { get; set; }
    public int? IdUser { get; set; }

    public virtual Language IdLanguageNavigation { get; set; } = null!;

    public virtual Level IdLevelNavigation { get; set; } = null!;

    public virtual Theme IdThemeNavigation { get; set; } = null!;
    public virtual User? IdUserNavigation { get; set; }


    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
