using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LangSpace.Models;
using LangSpace.Service;
using LangSpace.ViewModel;

namespace LangSpace.Views.Pages
{
    public partial class CardsPage : Page
    {
        private readonly CardService cardService;
        private readonly CardsPageViewModel viewModel;
        private bool navigationHooked;

        public CardsPage()
        {
            InitializeComponent();
            cardService = new CardService();
            viewModel = new CardsPageViewModel();
            DataContext = viewModel;
        }

       

        private void StudyCard_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Card card = btn?.Tag as Card;
            if (card == null) return;

            card.ShowTranslation = !card.ShowTranslation;
            btn.Content = card.ShowTranslation ? "Слово" : "Посмотреть";
        }

        private void BackButton_Click(object sender, MouseButtonEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
                NavigationService.Navigate(new DashboardPage());
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Card card = btn?.Tag as Card;

            if (card == null || !card.IsOwner)
            {
                MessageBox.Show("Вы можете редактировать только свои карточки.", "Доступ ограничен",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            NavigationService.Navigate(new CardEditPage(card));
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Card card = btn?.Tag as Card;

            if (card == null || !card.IsOwner)
            {
                MessageBox.Show("Вы можете удалять только свои карточки.", "Доступ ограничен",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string warningText = $"Вы действительно хотите удалить карточку \"{card.FrontText}\" ?";
            MessageBoxResult result = MessageBox.Show(warningText, "Предупреждение",
                MessageBoxButton.OKCancel, MessageBoxImage.Warning);

            if (result == MessageBoxResult.OK)
            {
                cardService.DeleteCard(card.IdCard);
                viewModel.Reload();
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CardEditPage());
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

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TestsPage());
        }
    }
}