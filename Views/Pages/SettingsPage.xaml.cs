using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LangSpace.Service;
using LangSpace.Models;

namespace LangSpace.Views.Pages
{
    public partial class SettingsPage : Page
    {
        private LevelTestService levelTestService;
        private GoalService goalService;

        public SettingsPage()
        {
            InitializeComponent();
            levelTestService = new LevelTestService();
            goalService = new GoalService();
            LoadUserData();
            LoadGoals();
        }

        private void LoadUserData()
        {
            var currentUser = LoginWindow.currentUser;
            if (currentUser != null)
            {
                tbUsername.Text = currentUser.Username;
                tbPassword.Text = "************";
                tbEmail.Text = currentUser.Email;

                string userLevel = levelTestService.GetUserLevel(currentUser.IdUser);
                if (string.IsNullOrEmpty(userLevel) || userLevel == "Уровень не установлен")
                {
                    tbLevel.Text = "Не установлен";
                }
                else
                {
                    tbLevel.Text = userLevel;
                }
            }
        }

        private void LoadGoals()
        {
            var currentUser = LoginWindow.currentUser;
            if (currentUser != null)
            {
                var goal = goalService.GetGoal(currentUser.IdUser);
                if (goal != null)
                {
                    SetComboBoxValue(cmbCardsPerDay, goal.CardsPerDay.ToString());
                    SetComboBoxValue(cmbTestsPerDay, goal.TestsPerDay.ToString());
                    SetComboBoxValue(cmbDaysStreak, goal.DaysStreak.ToString());
                }
            }
        }

        private void SetComboBoxValue(ComboBox comboBox, string value)
        {
            for (int i = 0; i < comboBox.Items.Count; i++)
            {
                var item = comboBox.Items[i] as ComboBoxItem;
                if (item != null && item.Content.ToString() == value)
                {
                    comboBox.SelectedIndex = i;
                    return;
                }
            }
            comboBox.SelectedIndex = 1; 
        }

        private void btnSaveGoals_Click(object sender, RoutedEventArgs e)
        {
            var currentUser = LoginWindow.currentUser;
            if (currentUser == null) return;

            int cardsPerDay = 20;
            int testsPerDay = 1;
            int daysStreak = 7;
            if (cmbCardsPerDay.SelectedItem is ComboBoxItem cardsItem)
            {
                int.TryParse(cardsItem.Content.ToString(), out cardsPerDay);
            }
            if (cmbTestsPerDay.SelectedItem is ComboBoxItem testsItem)
            {
                int.TryParse(testsItem.Content.ToString(), out testsPerDay);
            }
            if (cmbDaysStreak.SelectedItem is ComboBoxItem daysItem)
            {
                int.TryParse(daysItem.Content.ToString(), out daysStreak);
            }

            goalService.SaveGoal(currentUser.IdUser, cardsPerDay, testsPerDay, daysStreak);
            MessageBox.Show("Цели сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BackButton_Click(object sender, MouseButtonEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
            else
            {
                NavigationService.Navigate(new DashboardPage());
            }
        }
        private void btnEditProfile_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ProfilePage());
        }
    }
}