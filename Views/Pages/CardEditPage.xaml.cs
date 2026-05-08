using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LangSpace.Models;
using LangSpace.Service;

namespace LangSpace.Views.Pages
{
    public partial class CardEditPage : Page
    {
        private readonly CardService _cardService;
        private readonly int _currentUserId;
        private Card _editingCard;
        private bool _isEditMode;

        public CardEditPage()
        {
            InitializeComponent();
            _cardService = new CardService();
            _currentUserId = LoginWindow.currentUser.IdUser;
            _isEditMode = false;
            tbTitle.Text = "Создать карточку";
            LoadThemes();
            LoadLevels();
        }

        public CardEditPage(Card card)
        {
            InitializeComponent();
            _cardService = new CardService();
            _currentUserId = LoginWindow.currentUser.IdUser;
            _editingCard = card;
            _isEditMode = true;
            tbTitle.Text = "Редактировать карточку";
            LoadThemes();
            LoadLevels();
            LoadCardData();
        }

        private void LoadThemes()
        {
            var themes = _cardService.GetAllThemes();
            cmbTheme.ItemsSource = themes;
            cmbTheme.DisplayMemberPath = "ThemeName";
            cmbTheme.SelectedValuePath = "IdTheme";
        }

        private void LoadLevels()
        {
            var levels = _cardService.GetAllLevels();
            cmbLevel.ItemsSource = levels;
            cmbLevel.DisplayMemberPath = "LevelName";
            cmbLevel.SelectedValuePath = "IdLevel";
        }

        private void LoadCardData()
        {
            if (_editingCard == null) return;
            tbFrontText.Text = _editingCard.FrontText;
            tbBackText.Text = _editingCard.BackText;
            cmbTheme.SelectedValue = _editingCard.IdTheme;
            cmbLevel.SelectedValue = _editingCard.IdLevel;
            cbIsPrivate.IsChecked = _editingCard.IsPrivate;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbFrontText.Text))
            {
                MessageBox.Show("Введите слово!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(tbBackText.Text))
            {
                MessageBox.Show("Введите перевод!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cmbTheme.SelectedItem == null)
            {
                MessageBox.Show("Выберите тему!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cmbLevel.SelectedItem == null)
            {
                MessageBox.Show("Выберите уровень!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_isEditMode)
            {
                _editingCard.FrontText = tbFrontText.Text.Trim();
                _editingCard.BackText = tbBackText.Text.Trim();
                _editingCard.IdTheme = (int)cmbTheme.SelectedValue;
                _editingCard.IdLevel = (int)cmbLevel.SelectedValue;
                _editingCard.IsPrivate = cbIsPrivate.IsChecked ?? false;
                if (_cardService.UpdateCard(_editingCard))
                {
                    MessageBox.Show("Карточка обновлена!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    NavigationService.GoBack();
                }
            }

            else
            {
                if (_cardService.CardExists(
               tbFrontText.Text.Trim(),
               tbBackText.Text.Trim(),
               _currentUserId))
                {
                    MessageBox.Show("Такая карточка уже существует!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                var newCard = new Card
                {
                    FrontText = tbFrontText.Text.Trim(),
                    BackText = tbBackText.Text.Trim(),
                    IdTheme = (int)cmbTheme.SelectedValue,
                    IdLevel = (int)cmbLevel.SelectedValue,
                    IdUser = _currentUserId,
                    IdLanguage = 1,
                    IsPrivate = cbIsPrivate.IsChecked ?? false
                };

                if (_cardService.CreateCard(newCard, _currentUserId))
                {
                    MessageBox.Show("Карточка создана!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    NavigationService.GoBack();
                }
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void BackButton_Click(object sender, MouseButtonEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}