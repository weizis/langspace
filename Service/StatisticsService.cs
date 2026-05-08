using System;
using System.Collections.Generic;
using System.Linq;
using LangSpace.Models;

namespace LangSpace.Service
{
    public class StatisticsService
    {
        private readonly LangSpaceContext _context;

        public StatisticsService()
        {
            _context = new LangSpaceContext();
        }
        public void SaveTestResult(int userId, string testType, int score, int total)
        {
            var result = new TestResult
            {
                IdUser = userId,
                TestType = testType,
                Score = score,
                Total = total,
                Date = DateTime.Now
            };
            _context.TestResults.Add(result);
            _context.SaveChanges();
        }

        public int GetTotalTestsCount(int userId)
        {
            return _context.TestResults.Count(t => t.IdUser == userId);
        }

        public double GetAverageScore(int userId)
        {
            var results = _context.TestResults.Where(t => t.IdUser == userId).ToList();
            if (results.Count == 0) return 0;

            var avg = results.Average(t => (double)t.Score / t.Total) * 100;
            return Math.Round(avg, 1);
        }

        public (int Score, int Total, string TestType, DateTime Date) GetLastTest(int userId)
        {
            var last = _context.TestResults
                .Where(t => t.IdUser == userId)
                .OrderByDescending(t => t.Date)
                .FirstOrDefault();

            if (last == null) return (0, 0, "", DateTime.MinValue);
            return (last.Score, last.Total, last.TestType, last.Date);
        }

        public int GetTodayTestsCount(int userId)
        {
            var today = DateTime.Today;
            return _context.TestResults.Count(t => t.IdUser == userId && t.Date.Date == today);
        }

        public int GetTodayCardsCount(int userId)
        {
            var today = DateTime.Today;
            return _context.TestResults
                .Where(t => t.IdUser == userId && t.Date.Date == today && t.TestType == "card")
                .Sum(t => t.Total);
        }

        public int GetStreak(int userId)
        {
            var dates = _context.TestResults
                .Where(t => t.IdUser == userId)
                .Select(t => t.Date.Date)
                .Distinct()
                .OrderByDescending(d => d)
                .ToList();

            if (dates.Count == 0) return 0;

            int streak = 0;
            DateTime current = DateTime.Today;

            foreach (var date in dates)
            {
                if (date == current)
                {
                    streak++;
                    current = current.AddDays(-1);
                }
                else break;
            }
            return streak;
        }

        public (int CardTests, int GrammarTests) GetTestsByType(int userId)
        {
            int cardTests = _context.TestResults.Count(t => t.IdUser == userId && t.TestType == "card");
            int grammarTests = _context.TestResults.Count(t => t.IdUser == userId && t.TestType == "grammar");
            return (cardTests, grammarTests);
        }
    }
}