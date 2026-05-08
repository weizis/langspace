using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LangSpace.Service;
using LangSpace.Models;
using LangSpace.Views;
using System.Windows;

namespace LangSpace.ViewModel
{
    public class CardsPageViewModel : BaseViewModel
    {
        private readonly CardService cardService;
        private readonly LevelTestService levelTestService;
        private readonly int currentUserId;
        private ObservableCollection<Card> displayCards = null!;
        private List<Card> allCards = null!;
        private List<string> themeFilters = null!;
        private string selectedTheme = "Все темы";
        private string searchText = "";
        private List<Theme> allThemes = null!;
        private bool showOnlyPrivate;
        private string userLevel = "A1";
        private string cardsCountText = "0 из 0";
        public ObservableCollection<Card> DisplayCards
        {
            get { return displayCards; }
            set
            {
                displayCards = value;
                OnPropertyChanged("DisplayCards");
            }
        }
        public List<string> ThemeFilters
        {
            get { return themeFilters; }
            set
            {
                themeFilters = value;
                OnPropertyChanged("ThemeFilters");
            }
        }
        public string SelectedTheme
        {
            get { return selectedTheme; }
            set
            {
                selectedTheme = value;
                OnPropertyChanged("SelectedTheme");
                ApplyFilters();
            }
        }
        public string SearchText
        {
            get { return searchText; }
            set
            {
                searchText = value;
                OnPropertyChanged("SearchText");
                ApplyFilters();
            }
        }
        public bool ShowOnlyPrivate
        {
            get { return showOnlyPrivate; }
            set
            {
                showOnlyPrivate = value;
                OnPropertyChanged("ShowOnlyPrivate");
                ApplyFilters();
            }
        }
        public string UserLevel
        {
            get { return userLevel; }
            set
            {
                userLevel = value;
                OnPropertyChanged("UserLevel");
            }
        }
        public string CardsCountText
        {
            get { return cardsCountText; }
            set
            {
                cardsCountText = value;
                OnPropertyChanged("CardsCountText");
            }
        }
        public CardsPageViewModel()
        {
            cardService = new CardService();
            levelTestService = new LevelTestService();
            currentUserId = LoginWindow.currentUser?.IdUser ?? 0;
        }

        public void LoadData()
        {
            LoadUserLevel();
            LoadAllCards();
            LoadThemeFilters();
            ApplyFilters();
        }

        public void Reload()
        {
            LoadAllCards();
            ApplyFilters();
        }
        private void LoadUserLevel()
        {
            string level = levelTestService.GetUserLevel(currentUserId);
            if (string.IsNullOrEmpty(level) || level == "Уровень не установлен")
                UserLevel = "A1";
            else
                UserLevel = level;
        }

        private void LoadAllCards()
        {
            try
            {
                allCards = cardService.GetAllCardsByUserLevel(currentUserId);
                foreach (var card in allCards)
                {
                    card.ShowTranslation = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки карточек: {ex.Message}");
                allCards = new List<Card>();
            }
        }

        private void LoadThemeFilters()
        {
            allThemes = cardService.GetAllThemes();  
            var themes = allThemes.Select(t => t.ThemeName).ToList();
            themes.Insert(0, "Все темы");
            ThemeFilters = themes;
        }
        private void ApplyFilters()
        {
            if (currentUserId <= 0)
            {
                DisplayCards = new ObservableCollection<Card>();
                CardsCountText = $"0 из {allCards.Count}";
                return;
            }

            List<Card> filtered = allCards.ToList();

            
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                string searchLower = SearchText.ToLower();
                filtered = filtered.Where(c =>
                    (!string.IsNullOrEmpty(c.FrontText) && c.FrontText.ToLower().Contains(searchLower)) ||
                    (!string.IsNullOrEmpty(c.BackText) && c.BackText.ToLower().Contains(searchLower))
                ).ToList();
            }

            if (SelectedTheme != "Все темы" && !string.IsNullOrEmpty(SelectedTheme))
            {
                var theme = allThemes.FirstOrDefault(t => t.ThemeName == SelectedTheme);
                if (theme != null)
                {
                    filtered = filtered.Where(c => c.IdTheme == theme.IdTheme).ToList();
                }
            }

            
            if (ShowOnlyPrivate)
            {
                filtered = filtered.Where(c => c.IsPrivate && c.IdUser == currentUserId).ToList();
            }

            DisplayCards = new ObservableCollection<Card>(filtered);
            CardsCountText = $"{DisplayCards.Count} из {allCards.Count}";
        }
    }
}


