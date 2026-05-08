using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using LangSpace.Models;
using LangSpace.Service;
using LangSpace.Views;

namespace LangSpace.ViewModel
{
    public class GrammarPageViewModel : BaseViewModel
    {
        private readonly GrammarService grammarService;
        private readonly LevelTestService levelTestService;
        private readonly int currentUserId;

        private ObservableCollection<GrammarRule> displayRules = null!;
        private List<GrammarRule> allRules = null!;
        private List<string> levelFilters = null!;
        private string selectedLevel = "Все уровни";
        private string searchText = "";
        private string userLevel = "A1";
        private string rulesCountText = "0 из 0";
        private bool isTeacher;

        public ObservableCollection<GrammarRule> DisplayRules
        {
            get { return displayRules; }
            set
            {
                displayRules = value;
                OnPropertyChanged("DisplayRules");
            }
        }

        public List<string> LevelFilters
        {
            get { return levelFilters; }
            set
            {
                levelFilters = value;
                OnPropertyChanged("LevelFilters");
            }
        }

        public string SelectedLevel
        {
            get { return selectedLevel; }
            set
            {
                selectedLevel = value;
                OnPropertyChanged("SelectedLevel");
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

        public string UserLevel
        {
            get { return userLevel; }
            set
            {
                userLevel = value;
                OnPropertyChanged("UserLevel");
            }
        }

        public string RulesCountText
        {
            get { return rulesCountText; }
            set
            {
                rulesCountText = value;
                OnPropertyChanged("RulesCountText");
            }
        }

        public bool IsTeacher
        {
            get { return isTeacher; }
            set
            {
                isTeacher = value;
                OnPropertyChanged("IsTeacher");
            }
        }

        public GrammarPageViewModel()
        {
            grammarService = new GrammarService();
            levelTestService = new LevelTestService();
            currentUserId = LoginWindow.currentUser?.IdUser ?? 0;
            IsTeacher = LoginWindow.currentUser?.IdRole == 2;
        }

        public void LoadData()
        {
            LoadUserLevel();
            LoadAllRules();
            LoadLevelFilters(); 
            ApplyFilters();
        }

        public void Reload()
        {
            LoadAllRules();
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

        private void LoadAllRules()
        {
            try
            {
                allRules = grammarService.GetAllRulesByUserLevel(currentUserId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки правил: {ex.Message}");
                allRules = new List<GrammarRule>();
            }
        }

        private void LoadLevelFilters()
        {
            var availableLevelNames = CardService.GetAvailableLevelNames(UserLevel);

            var levels = grammarService.GetAllLevels()
                .Where(l => availableLevelNames.Contains(l.LevelName))
                .Select(l => l.LevelName)
                .ToList();

            levels.Insert(0, "Все уровни");
            LevelFilters = levels;
            if (SelectedLevel != "Все уровни" && !availableLevelNames.Contains(SelectedLevel))
            {
                SelectedLevel = "Все уровни";
            }
        }

        private void ApplyFilters()
        {
            if (currentUserId <= 0)
            {
                DisplayRules = new ObservableCollection<GrammarRule>();
                RulesCountText = $"0 из {allRules.Count}";
                return;
            }

            List<GrammarRule> filtered = allRules.ToList();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                string searchLower = SearchText.ToLower();
                filtered = filtered.Where(r =>
                    (!string.IsNullOrEmpty(r.RuleName) && r.RuleName.ToLower().Contains(searchLower)) ||
                    (!string.IsNullOrEmpty(r.RuleText) && r.RuleText.ToLower().Contains(searchLower))
                ).ToList();
            }

            if (SelectedLevel != "Все уровни" && !string.IsNullOrEmpty(SelectedLevel))
            {
                filtered = filtered.Where(r => r.IdLevelNavigation != null &&
                                               r.IdLevelNavigation.LevelName == SelectedLevel).ToList();
            }

            DisplayRules = new ObservableCollection<GrammarRule>(filtered);
            RulesCountText = $"{DisplayRules.Count} из {allRules.Count}";
        }
    }
}