using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LangSpace.ViewModel;

namespace LangSpace.Views.Pages
{
    public partial class TestsPage : Page
    {
        private readonly TestsPageViewModel viewModel;

        public TestsPage()
        {
            InitializeComponent();
            viewModel = new TestsPageViewModel();
            DataContext = viewModel;
        }

        public TestsPage(int grammarRuleId)
        {
            InitializeComponent();
            viewModel = new TestsPageViewModel(grammarRuleId);
            DataContext = viewModel;
        }

        private void btnStartTest_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.CanStartCardTest())
                viewModel.StartCardTest();
            else
                MessageBox.Show("Выберите тему и уровень");
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.CanGoNext())
                viewModel.NextQuestion();
            else
                MessageBox.Show("Пожалуйста, выберите вариант ответа или введите перевод.");
        }

        private void btnRestart_Click(object sender, RoutedEventArgs e)
        {
            viewModel.RestartTest();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите выйти? Ваш прогресс не будет сохранен.",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                GoBack();
            }
        }

        private void BackButton_Click(object sender, MouseButtonEventArgs e)
        {
            if (viewModel.HasActiveTest())
            {
                if (MessageBox.Show("Вы уверены, что хотите выйти? Ваш прогресс не будет сохранен.",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    GoBack();
                }
            }
            else
            {
                GoBack();
            }
        }

        private void GoBack()
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
                NavigationService.Navigate(new DashboardPage());
        }
    }
}