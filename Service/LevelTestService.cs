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
    public class LevelTestService
    {
        public List<TestQuestion> GetRandomQuestions()
        {
            using (var context = new LangSpaceContext())
            {
                var questions = context.TestQuestions.ToList();
                Random rnd = new Random();
                return questions.OrderBy(q => rnd.Next()).ToList();
            }
        }
        public string GetLevelByScore(int score)
        {
            if (score <= 4) return "A1";
            if (score <= 8) return "A2";
            if (score <= 12) return "B1";
            if (score <= 16) return "B2";
            if (score <= 20) return "C1";
            return "C2";
        }
        public void SaveUserLevel(int userId, string level, int languageId = 1)
        {
            using (var context = new LangSpaceContext())
            {
                var levelUser = context.Levels.FirstOrDefault(l => l.LevelName == level);
                if (level == null)
                {
                    MessageBox.Show("Уровень не найден!");
                    return;
                }

                var userLanguage = context.UserLanguages.FirstOrDefault(ul => ul.IdUser == userId && ul.IdLanguage == languageId);
                if (userLanguage != null)
                {
                    userLanguage.IdLevel = levelUser.IdLevel;
                }
                else
                {
                    var newUserLanguage = new UserLanguage
                    {
                        IdUser = userId,
                        IdLanguage = languageId,
                        IdLevel = levelUser.IdLevel
                    };
                    context.UserLanguages.Add(newUserLanguage);
                }
                context.SaveChanges();

            }
        }
        public bool HasUserLevel(int userId, int languageId = 1)
        {
            using (var context = new LangSpaceContext())
            {
                return context.UserLanguages.Any(ul => ul.IdUser == userId && ul.IdLanguage == languageId && ul.IdLevel != null);
            }
        }
        public string GetUserLevel(int userId, int languageId = 1)
        {
            using (var context = new LangSpaceContext())
            {
                var userLanguage = context.UserLanguages.Include(ul => ul.IdLevelNavigation).FirstOrDefault(ul => ul.IdUser == userId && ul.IdLanguage == languageId);
                if (userLanguage?.IdLevelNavigation != null)
                {
                    return userLanguage.IdLevelNavigation.LevelName;
                }
                return "Уровень не установлен";
            }
        }
    }
}
