using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LangSpace.Models;
using Microsoft.EntityFrameworkCore;

namespace LangSpace.Service
{
    public class CardService
    {
        private static bool CanEdit(Card card)
        {
            var actor = UserSession.CurrentUser;
            return actor != null && (card.IdUser == actor.IdUser || (!card.IsPrivate && actor.IdRole == 2));
        }

        public List<Card> GetAllCardsByUserLevel(int userId, int languageId = 1)
        {
            var levelService = new LevelTestService();
            string userLevel = levelService.GetUserLevel(userId, languageId);

            if (userLevel == "Уровень не установлен")
                userLevel = "A1";

            using (var context = new LangSpaceContext())
            {
                var availableLevelNames = GetAvailableLevelNames(userLevel);

                return context.Cards
                    .Include(c => c.IdUserNavigation)
                    .Include(c => c.IdLevelNavigation)
                    .Include(c => c.IdThemeNavigation)
                    .Include(c => c.IdLanguageNavigation)
                    .Where(c => availableLevelNames.Contains(c.IdLevelNavigation.LevelName) &&
                               (!c.IsPrivate || c.IdUser == userId))
                    .ToList();
            }
        }   
        public List<Card> GetCardsByThemeAndLevel(int themeId, int userId, int languageId = 1)
        {
            var levelService = new LevelTestService();
            string userLevel = levelService.GetUserLevel(userId, languageId);

            if (userLevel == "Уровень не установлен")
                userLevel = "A1";

            using (var context = new LangSpaceContext())
            {
                var availableLevelNames = GetAvailableLevelNames(userLevel);

                return context.Cards
                    .Include(c => c.IdUserNavigation)
                    .Include(c => c.IdLevelNavigation)
                    .Include(c => c.IdThemeNavigation)
                    .Include(c => c.IdLanguageNavigation)
                    .Where(c => c.IdTheme == themeId &&
                               availableLevelNames.Contains(c.IdLevelNavigation.LevelName) &&
                               (!c.IsPrivate || c.IdUser == userId))
                    .ToList();
            }
        }
        public int CountCardsByThemeAndLevel(int themeId, int userId, int languageId = 1)
        {
            var levelService = new LevelTestService();
            string userLevel = levelService.GetUserLevel(userId, languageId);

            if (userLevel == "Уровень не установлен")
                userLevel = "A1";

            using (var context = new LangSpaceContext())
            {
                var availableLevelNames = GetAvailableLevelNames(userLevel);

                return context.Cards
                    .Count(c => c.IdTheme == themeId &&
                               availableLevelNames.Contains(c.IdLevelNavigation.LevelName) &&
                               (!c.IsPrivate || c.IdUser == userId));
            }
        }
        public bool CreateCard(Card card, int userId)
        {
            if (UserSession.CurrentUser?.IdUser != userId) return false;
            try
            {
                using (var context = new LangSpaceContext())
                {
                    card.IdUser = userId;
                    context.Cards.Add(card);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании карточки: {ex.Message}");
                return false;
            }
        }
        public bool UpdateCard(Card card)
        {
            try
            {
                using (var context = new LangSpaceContext())
                {
                    var existingCard = context.Cards.Find(card.IdCard);
                    if (existingCard == null || !CanEdit(existingCard))
                    {
                        MessageBox.Show($"Карточка с ID {card.IdCard} не найдена в БД!");
                        return false;
                    }
                   
                    existingCard.FrontText = card.FrontText;
                    existingCard.BackText = card.BackText;
                    existingCard.IdTheme = card.IdTheme;
                    existingCard.IdLevel = card.IdLevel;
                    existingCard.IsPrivate = card.IsPrivate;
                    int result = context.SaveChanges();

                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления карточки: {ex.Message}");
                return false;
            }
        }
        public bool CardExists(string frontText, string backText, int userId, int languageId = 1)
        {
            using (var context = new LangSpaceContext())
            {
                return context.Cards.Any(c =>
                    c.FrontText.ToLower() == frontText.ToLower() &&
                    c.BackText.ToLower() == backText.ToLower() &&
                    c.IdUser == userId &&
                    c.IdLanguage == languageId);
            }
        }
        public bool DeleteCard(int cardId)
        {
            try
            {
                using (var context = new LangSpaceContext())
                {
                    var card = context.Cards
                        .FirstOrDefault(c => c.IdCard == cardId);

                    if (card != null && CanEdit(card))
                    {
                        context.Cards.Remove(card);
                        context.SaveChanges();
                        return true;
                    }
                    else
                    {
                        MessageBox.Show($"Карточка с ID {cardId} не найдена!");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления карточки: {ex.Message}\n\nВнутренняя ошибка: {ex.InnerException?.Message}");
                return false;
            }
        }
        public string GetCreatorName(int cardId)
        {
            using (var context = new LangSpaceContext())
            {
                var card = context.Cards
                    .Include(c => c.IdUserNavigation)
                    .FirstOrDefault(c => c.IdCard == cardId);

                if (card?.IdUserNavigation != null)
                    return card.IdUserNavigation.Username;

                return "Система";
            }
        }
       
        public List<Card> SearchCardsByLevel(string searchText, int userId, int languageId = 1)
        {
            var levelService = new LevelTestService();
            string userLevel = levelService.GetUserLevel(userId, languageId);

            if (userLevel == "Уровень не установлен")
                userLevel = "A1";

            if (string.IsNullOrWhiteSpace(searchText))
                return GetCardsByThemeAndLevel(0, userId, languageId);

            using (var context = new LangSpaceContext())
            {
                var availableLevelNames = GetAvailableLevelNames(userLevel);

                return context.Cards
                    .Include(c => c.IdUserNavigation)
                    .Include(c => c.IdLevelNavigation)
                    .Include(c => c.IdThemeNavigation)
                    .Where(c => availableLevelNames.Contains(c.IdLevelNavigation.LevelName) &&
                               (!c.IsPrivate || c.IdUser == userId) &&
                               (c.FrontText.Contains(searchText) ||
                                c.BackText.Contains(searchText)))
                    .ToList();
            }
        }

        public List<Theme> GetAllThemes()
        {
            using (var context = new LangSpaceContext())
            {
                return context.Themes.ToList();
            }
        }
        public List<Level> GetAllLevels()
        {
            using (var context = new LangSpaceContext())
            {
                return context.Levels.ToList();
            }
        }
        public List<Language> GetAllLanguages()
        {
            using (var context = new LangSpaceContext())
            {
                return context.Languages.ToList();
            }
        }
        public static List<string> GetAvailableLevelNames(string currentLevel)
        {
            var levelOrder = new[] { "A1", "A2", "B1", "B2", "C1", "C2" };
            int currentIndex = Array.IndexOf(levelOrder, currentLevel);

            if (currentIndex < 0)
                currentIndex = 0; 
            int maxAvailableIndex;

            switch (currentLevel)
            {
                case "A1":
                    maxAvailableIndex = 1; 
                    break;
                case "A2":
                    maxAvailableIndex = 2; 
                    break;
                case "B1":
                    maxAvailableIndex = 3; 
                    break;
                case "B2":
                    maxAvailableIndex = 4; 
                    break;
                case "C1":
                case "C2":
                    maxAvailableIndex = 5; 
                    break;
                default:
                    maxAvailableIndex = currentIndex;
                    break;
            }
            return levelOrder.Take(maxAvailableIndex + 1).ToList();
        }
        public Dictionary<int, bool> InitializeTranslationState(List<Card> cards)
        {
            var state = new Dictionary<int, bool>();
            foreach (var card in cards)
            {
                state[card.IdCard] = false;
            }
            return state;
        }

        public List<Card> FilterByTheme(List<Card> cards, string themeName, int userId)
        {
            if (themeName == "Все темы" || string.IsNullOrEmpty(themeName))
                return cards;

            var theme = GetAllThemes().FirstOrDefault(t => t.ThemeName == themeName);
            if (theme == null) return cards;

            return GetCardsByThemeAndLevel(theme.IdTheme, userId);
        }
    }
}
