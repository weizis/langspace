using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LangSpace.Models;
using LangSpace.Service;
using LangSpace.ViewModel;

namespace LangSpace.Views.Pages
{
    public partial class GrammarPage : Page
    {
        private readonly GrammarService grammarService;
        private readonly GrammarPageViewModel viewModel;
        private bool navigationHooked;

        public GrammarPage()
        {
            InitializeComponent();
            grammarService = new GrammarService();
            viewModel = new GrammarPageViewModel();
            DataContext = viewModel;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel.LoadData();

            if (!navigationHooked && NavigationService != null)
            {
                navigationHooked = true;
                NavigationService.Navigated += (_, __) => viewModel.LoadData();
            }
        }

        private void StudyRule_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            GrammarRule rule = btn?.Tag as GrammarRule;
            if (rule != null)
            {
                NavigationService.Navigate(new GrammarDetailPage(rule));
            }
        }

        private void btnAddRule_Click(object sender, RoutedEventArgs e)
        {
            if (LoginWindow.currentUser?.IdRole != 2)
            {
                MessageBox.Show("У вас нет прав на добавление правил");
                return;
            }
            NavigationService.Navigate(new GrammarEditPage());
        }

        private void btnEditRule_Click(object sender, RoutedEventArgs e)
        {
            if (LoginWindow.currentUser?.IdRole != 2)
            {
                MessageBox.Show("У вас нет прав на редактирование правил");
                return;
            }
            Button btn = sender as Button;
            GrammarRule rule = btn?.Tag as GrammarRule;
            if (rule != null)
            {
                NavigationService.Navigate(new GrammarEditPage(rule));
            }
        }

        private void btnDeleteRule_Click(object sender, RoutedEventArgs e)
        {
            if (LoginWindow.currentUser?.IdRole != 2)
            {
                MessageBox.Show("У вас нет прав на удаление правил");
                return;
            }
            Button btn = sender as Button;
            GrammarRule rule = btn?.Tag as GrammarRule;

            if (rule == null) return;

            string warningText = $"Вы действительно хотите удалить правило \"{rule.RuleName}\" ?\n\nВместе с правилом будут удалены все связанные тесты!";
            MessageBoxResult result = MessageBox.Show(warningText, "Предупреждение",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                if (grammarService.DeleteRule(rule.IdRule))
                {
                    MessageBox.Show("Правило удалено");
                    viewModel.Reload();
                }
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