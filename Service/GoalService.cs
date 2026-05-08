using System;
using System.Linq;
using LangSpace.Models;

namespace LangSpace.Service
{
    public class GoalService
    {
        public Goal GetGoal(int userId)
        {
            using (var context = new LangSpaceContext())
            {
                return context.Goals.FirstOrDefault(g => g.IdUser == userId);
            }
        }

        public void SaveGoal(int userId, int cardsPerDay, int testsPerDay, int daysStreak)
        {
            using (var context = new LangSpaceContext())
            {
                var goal = context.Goals.FirstOrDefault(g => g.IdUser == userId);
                if (goal == null)
                {
                    goal = new Goal
                    {
                        IdUser = userId,
                        CardsPerDay = cardsPerDay,
                        TestsPerDay = testsPerDay,
                        DaysStreak = daysStreak
                    };
                    context.Goals.Add(goal);
                }
                else
                {
                    goal.CardsPerDay = cardsPerDay;
                    goal.TestsPerDay = testsPerDay;
                    goal.DaysStreak = daysStreak;
                }
                context.SaveChanges();
            }
        }
    }
}