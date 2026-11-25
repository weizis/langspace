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
    public class GrammarService
    {
        public List<GrammarRule> GetAllRulesByUserLevel(int userId, int languageId = 1)
        {
            var levelService = new LevelTestService();
            string userLevel = levelService.GetUserLevel(userId, languageId);
            if (userLevel == "Уровень не установлен")
                userLevel = "A1";
            using(var context = new LangSpaceContext())
            {
                var availableLevelNames = GetAvailableLevelNames(userLevel);
                return context.GrammarRules
                    .Include(g => g.IdLevelNavigation)
                    .Include(g => g.IdLanguageNavigation)
                    .Where(g => availableLevelNames.Contains(g.IdLevelNavigation.LevelName) && g.IdLanguage == languageId)
                    .OrderBy(g => g.IdLevel)
                    .ThenBy(g => g.RuleName)
                    .ToList();

            }
        }
        public List<GrammarRule> GetRulesByLevelName(string levelName, int userId, int languageId = 1)
        {
            var levelService = new LevelTestService();
            string userLevel = levelService.GetUserLevel(userId, languageId);
            if (userLevel == "Уровень не установлен")
                userLevel = "A1";
            using (var context = new LangSpaceContext())
            {
                var availableLevelNames = GetAvailableLevelNames(userLevel);
                var query = context.GrammarRules
                    .Include(g => g.IdLanguageNavigation)
                    .Include(g => g.IdLevelNavigation)
                    .Where(g => availableLevelNames.Contains(g.IdLevelNavigation.LevelName) && g.IdLanguage == languageId);
                if (!string.IsNullOrEmpty(levelName) && levelName != "Все уровни")
                {
                    query = query.Where(g => g.IdLevelNavigation.LevelName == levelName);
                }
                return query.OrderBy(g => g.IdLevel)
                    .ThenBy(g => g.RuleName)
                    .ToList();
            }
        }
        
        public List<GrammarRule> SearchRules(string searchText, int userId, int languageId = 1)
        {
            var levelService = new LevelTestService();
            string userLevel = levelService.GetUserLevel(userId, languageId);
            if (userLevel == "Уровень не установлен")
                userLevel = "A1";
            if (string.IsNullOrWhiteSpace(searchText))
                return GetAllRulesByUserLevel(userId, languageId);
            using (var context = new LangSpaceContext())
            {
                var availableLevelNames = GetAvailableLevelNames(userLevel);
                return context.GrammarRules
                    .Include(g => g.IdLevelNavigation)
                    .Include(g => g.IdLanguageNavigation)
                    .Where(g => availableLevelNames.Contains(g.IdLevelNavigation.LevelName) &&
                               g.IdLanguage == languageId &&
                               (g.RuleName.Contains(searchText) ||
                                g.RuleText.Contains(searchText)))
                    .OrderBy(g => g.IdLevel)
                    .ThenBy(g => g.RuleName)
                    .ToList();

            }
        }
        public GrammarRule GetRuleById(int ruleId)
        {
            using (var context = new LangSpaceContext())
            {
                return context.GrammarRules
                    .Include(g => g.IdLevelNavigation)
                    .Include(g => g.IdLanguageNavigation)
                    .FirstOrDefault(g => g.IdRule == ruleId);
            }
        }
        public bool UpdateRule(GrammarRule rule)
        {
            if (UserSession.CurrentUser?.IdRole != 2) return false;
            try
            {
                using (var context = new LangSpaceContext())
                {
                    var existing = context.GrammarRules.Find(rule.IdRule);
                    if (existing == null) return false;

                    existing.RuleName = rule.RuleName;
                    existing.RuleText = rule.RuleText;
                    existing.IdLevel = rule.IdLevel;

                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления: {ex.Message}");
                return false;
            }
        }
        public List<Level> GetAllLevels()
        {
            using (var context = new LangSpaceContext())
            {
                return context.Levels.OrderBy(l => l.IdLevel).ToList();
            }
        }
        private static List<string> GetAvailableLevelNames(string currentLevel)
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

        public bool CreateRule(GrammarRule rule)
        {
            if (UserSession.CurrentUser?.IdRole != 2) return false;
            try
            {
                using (var context = new LangSpaceContext())
                {
                    context.GrammarRules.Add(rule);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка создания: {ex.Message}");
                return false;
            }
        }
        public bool DeleteRule(int ruleId)
        {
            if (UserSession.CurrentUser?.IdRole != 2) return false;
            try
            {
                using (var context = new LangSpaceContext())
                {
                    var rule = context.GrammarRules
                        .Include(r => r.GrammarQuestions)
                        .FirstOrDefault(r => r.IdRule == ruleId);

                    if (rule != null)
                    {
                        if (rule.GrammarQuestions != null && rule.GrammarQuestions.Any())
                        {
                            context.GrammarQuestions.RemoveRange(rule.GrammarQuestions);
                        }

                        context.GrammarRules.Remove(rule);
                        context.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления: {ex.Message}");
                return false;
            }
        }
    }
}
