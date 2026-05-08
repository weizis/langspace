using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using LangSpace.Service;

namespace LangSpace.Views.Pages
{
    public partial class DashboardPage : Page
    {
        private StatisticsService statisticsService;
        private GoalService goalService;
        private LevelTestService levelTestService;
        private int currentUserId;
        private List<string> funFacts;

        public DashboardPage()
        {
            InitializeComponent();
            statisticsService = new StatisticsService();
            goalService = new GoalService();
            levelTestService = new LevelTestService();
            currentUserId = LoginWindow.currentUser.IdUser;

            funFacts = new List<string>
            {
                "Английский язык — официальный язык международного общения. Его изучают более 1.5 миллиарда человек по всему миру!",
                "Самое длинное слово в английском языке — 'pneumonoultramicroscopicsilicovolcanoconiosis' (45 букв).",
                "Слово 'set' имеет более 430 значений — это самое многозначное слово в английском языке.",
                "Около 4 миллиардов человек говорят на английском, но только 400 миллионов — носители.",
                "Шекспир ввел в английский язык более 1700 новых слов, включая 'bedroom', 'lonely' и 'gossip'.",
                "В английском есть слово 'queue', где 4 буквы подряд не произносятся.",
                "Английский алфавит состоит из 26 букв, но звуков — от 44 до 52.",
                "Самый короткий грамматически правильный английский текст — 'Buffalo buffalo Buffalo buffalo buffalo buffalo Buffalo buffalo'."
            };
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadUserInfo();
            LoadStatistics();
            LoadRandomFact();
        }

        private void LoadUserInfo()
        {
            var user = LoginWindow.currentUser;
            if (user != null)
            {
                string firstName = user.FirstName ?? "";
                string lastName = user.LastName ?? "";
                WelcomeText.Text = $"Добро пожаловать, {firstName} {lastName}!";
                UserName.Text = $"{firstName} {lastName}";
                UserLogin.Text = $"@{user.Username}";
                UserEmail.Text = user.Email;

                string userLevel = levelTestService.GetUserLevel(currentUserId);
                if (string.IsNullOrEmpty(userLevel) || userLevel == "Уровень не установлен")
                {
                    UserLevel.Text = "A1";
                }
                else
                {
                    UserLevel.Text = userLevel;
                }

                if (!string.IsNullOrEmpty(user.Avatar))
                {
                    string avatarPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", user.Avatar);
                    if (System.IO.File.Exists(avatarPath))
                    {
                        imgAvatar.Source = new BitmapImage(new Uri(avatarPath));
                    }
                }
            }
        }

        private void LoadStatistics()
        {
            int totalTests = statisticsService.GetTotalTestsCount(currentUserId);
            double avgScore = statisticsService.GetAverageScore(currentUserId);
            int todayTests = statisticsService.GetTodayTestsCount(currentUserId);
            int streak = statisticsService.GetStreak(currentUserId);

            TotalTests.Text = $"Пройдено тестов: {totalTests}";
            AvgScore.Text = $"Средний результат: {avgScore}%";
            TodayStats.Text = $"Сегодня: {todayTests} тестов";
            Streak.Text = $"Дней подряд: {streak}";

            var goal = goalService.GetGoal(currentUserId);
            if (goal != null)
            {
                int todayCards = statisticsService.GetTodayCardsCount(currentUserId);
                double percent = goal.CardsPerDay > 0 ? (double)todayCards / goal.CardsPerDay * 100 : 0;
                percent = Math.Min(percent, 100);
                ProgressFill.Width = (percent / 100) * 380;
                GoalText.Text = $"Цель на сегодня: {todayCards} / {goal.CardsPerDay} карточек";

                if (todayCards >= goal.CardsPerDay)
                {
                    GoalStatusText.Text = "Цель достигнута!";
                    GoalStatusText.Foreground = System.Windows.Media.Brushes.Green;
                    GoalStatusText.Visibility = Visibility.Visible;
                }
                else
                {
                    GoalStatusText.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                ProgressFill.Width = 0;
                GoalText.Text = "Установите цель в настройках";
                GoalStatusText.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadRandomFact()
        {
            Random rnd = new Random();
            int index = rnd.Next(funFacts.Count);
            FunFact.Text = funFacts[index];
        }

        private void btnNewFact_Click(object sender, RoutedEventArgs e)
        {
            LoadRandomFact();
        }

        private void btnNewNote_Click(object sender, RoutedEventArgs e)
        {
            NoteEditPage editPage = new NoteEditPage("Текстовая заметка");
            NavigationService.Navigate(editPage);
        }

        private void btnLevelTest_Click(object sender, RoutedEventArgs e)
        {
            LevelTestWindow testWindow = new LevelTestWindow();
            testWindow.Owner = Window.GetWindow(this);
            testWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            testWindow.ShowDialog();

            LoadUserInfo();
            LoadStatistics();
        }

        private void btnRepeatGrammar_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new GrammarPage());
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