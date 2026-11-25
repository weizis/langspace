using LangSpace.Models;
using Microsoft.EntityFrameworkCore;

namespace LangSpace.Service;

public class GoalService
{
    public Goal? GetGoal(int userId)
    {
        if (UserSession.CurrentUser?.IdUser != userId) return null;
        using var context = new LangSpaceContext();
        var goal = context.Goals.AsNoTracking().FirstOrDefault(g => g.IdUser == userId);
        if (goal != null)
            goal.DaysStreak = CalculateStreak(context, userId);
        return goal;
    }

    public void SaveGoal(int userId, int cardsPerDay, int testsPerDay, int daysStreak)
    {
        if (UserSession.CurrentUser?.IdUser != userId) return;
        if (cardsPerDay < 0 || testsPerDay < 0) return;
        using var context = new LangSpaceContext();
        var goal = context.Goals.FirstOrDefault(g => g.IdUser == userId);
        if (goal == null)
        {
            goal = new Goal { IdUser = userId };
            context.Goals.Add(goal);
        }
        goal.CardsPerDay = cardsPerDay;
        goal.TestsPerDay = testsPerDay;
        goal.DaysStreak = CalculateStreak(context, userId);
        context.SaveChanges();
    }

    private static int CalculateStreak(LangSpaceContext context, int userId)
        => context.Database.SqlQuery<int>($"SELECT public.calculate_test_streak({userId}) AS \"Value\"").Single();
}
