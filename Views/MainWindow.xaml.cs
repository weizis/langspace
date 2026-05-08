using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LangSpace.Views.Pages;

namespace LangSpace.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new DashboardPage());
            if (LoginWindow.currentUser != null && LoginWindow.currentUser.IdRole == 1)
            {
                navAdmin.Visibility = Visibility.Visible;
            }
        }

        private void navHome_Click(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new DashboardPage());
            SetActiveNavItem(navHome);
        }

        private void navCollections_Click(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new CardsPage());
            SetActiveNavItem(navCollections);
        }

        private void navGrammar_Click(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new GrammarPage());
            SetActiveNavItem(navGrammar);
        }

        private void navWorkspace_Click(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new NotesPage());
            SetActiveNavItem(navWorkspace);
        }

        private void navStatistics_Click(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new StatisticsPage());
            SetActiveNavItem(navStatistics);
        }

        private void navSettings_Click(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new SettingsPage());
            SetActiveNavItem(navSettings);
        }
        private void navAdmin_Click(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new AdminPanelPage());
            SetActiveNavItem(navAdmin);
        }
        private void navExit_Click(object sender, MouseButtonEventArgs e)
        {
            var result = MessageBox.Show("Вы действительно хотите выйти?",
                                         "Выход",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }

        private void SetActiveNavItem(Border activeItem)
        {
            var navItems = new[] { navHome, navCollections, navGrammar,
                                   navWorkspace, navStatistics, navSettings, navExit };

            foreach (var item in navItems)
            {
                item.Background = System.Windows.Media.Brushes.Transparent;
            }

            activeItem.Background = new System.Windows.Media.SolidColorBrush(
                System.Windows.Media.Color.FromArgb(50, 255, 255, 255));
        }
    }
}
