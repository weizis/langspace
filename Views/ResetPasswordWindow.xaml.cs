using System.Windows;
using LangSpace.Models;
using LangSpace.Service;

namespace LangSpace.Views
{
    public partial class ResetPasswordWindow : Window
    {
        private User _user;
        private UserService _userService;

        public ResetPasswordWindow(User user)
        {
            InitializeComponent();
            _user = user;
            _userService = new UserService();

            if (_user != null)
            {
                txtUserFullName.Text = $"{_user.LastName} {_user.FirstName}";
            }
            else
            {
                txtUserFullName.Text = "Пользователь не найден";
            }
        }

        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            if (_user == null)
            {
                MessageBox.Show("Пользователь не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
                return;
            }

            string newPassword = txtNewPassword.Password;
            string confirmPassword = txtConfirmPassword.Password;

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                MessageBox.Show("Введите новый пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (newPassword.Length < 6)
            {
                MessageBox.Show("Пароль должен быть не менее 6 символов!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!newPassword.Any(char.IsDigit))
            {
                MessageBox.Show("Пароль должен содержать хотя бы одну цифру!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Изменить пароль пользователю {_user.LastName} {_user.FirstName}?",
                                         "Подтверждение",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                bool success = _userService.ResetPassword(_user.IdUser, newPassword);

                if (success)
                {
                    MessageBox.Show("Пароль успешно изменен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}