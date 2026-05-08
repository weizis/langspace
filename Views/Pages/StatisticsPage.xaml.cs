using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LangSpace.Service;

namespace LangSpace.Views.Pages
{
    public partial class StatisticsPage : Page
    {
        private StatisticsService statisticsService;
        private GoalService goalService;
        private int currentUserId;

        public StatisticsPage()
        {
            InitializeComponent();
            statisticsService = new StatisticsService();
            goalService = new GoalService();
            currentUserId = LoginWindow.currentUser.IdUser;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadStatistics();
            LoadGoals();
        }

        private void LoadStatistics()
        {
            int totalTests = statisticsService.GetTotalTestsCount(currentUserId);
            double avgScore = statisticsService.GetAverageScore(currentUserId);
            var (cardTests, grammarTests) = statisticsService.GetTestsByType(currentUserId);

            txtTotalTests.Text = $"Пройдено тестов: {totalTests}";
            txtAvgScore.Text = $"Средний результат: {avgScore}%";
            txtCardTests.Text = $"Тесты по карточкам: {cardTests}";
            txtGrammarTests.Text = $"Тесты по грамматике: {grammarTests}";

            var (score, total, testType, date) = statisticsService.GetLastTest(currentUserId);
            string lastTestText = date == DateTime.MinValue ? "Последний тест: —" : $"Последний тест: {score}/{total} ({testType})";
            txtLastTest.Text = lastTestText;

            int todayTests = statisticsService.GetTodayTestsCount(currentUserId);
            txtTodayTests.Text = $"Сегодня: {todayTests} тестов";

            int streak = statisticsService.GetStreak(currentUserId);
            txtStreak.Text = $"Стрик: {streak} дней";
        }

        private void LoadGoals()
        {
            var goal = goalService.GetGoal(currentUserId);
            if (goal != null)
            {
                txtGoalCards.Text = goal.CardsPerDay.ToString();
                int todayCards = statisticsService.GetTodayCardsCount(currentUserId);

                double percent = goal.CardsPerDay > 0 ? (double)todayCards / goal.CardsPerDay : 0;
                percent = Math.Min(percent, 1.0);
                ProgressFill.Width = percent * 700;

                if (todayCards >= goal.CardsPerDay)
                {
                    txtGoalProgress.Text = $" Цель выполнена! {todayCards} из {goal.CardsPerDay}";
                    txtGoalProgress.Foreground = System.Windows.Media.Brushes.Green;
                }
                else
                {
                    txtGoalProgress.Text = $"Сегодня: {todayCards} из {goal.CardsPerDay}";
                    txtGoalProgress.Foreground = System.Windows.Media.Brushes.Gray;
                }
            }
            else
            {
                txtGoalCards.Text = "Не установлены";
                txtGoalProgress.Text = "Установите цели в настройках";
                ProgressFill.Width = 0;
            }
        }

        private void BackButton_Click(object sender, MouseButtonEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
                NavigationService.Navigate(new DashboardPage());
        }
    }
}