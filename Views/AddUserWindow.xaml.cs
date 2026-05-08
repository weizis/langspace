using System;
using System.Linq;
using System.Windows;
using LangSpace.Models;
using LangSpace.Service;

namespace LangSpace.Views
{
    public partial class AddUserWindow : Window
    {
        private UserService _userService;

        public AddUserWindow()
        {
            InitializeComponent();
            _userService = new UserService();
            LoadRoles();
        }

        private void LoadRoles()
        {
            using (var context = new LangSpaceContext())
            {
                cmbRole.ItemsSource = context.Roles.ToList();
                cmbRole.SelectedValue = 2; 
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show("Введите фамилию!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                MessageBox.Show("Введите имя!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Введите логин!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Введите email!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                MessageBox.Show("Введите пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (txtPassword.Password.Length < 6)
            {
                MessageBox.Show("Пароль должен быть не менее 6 символов!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (txtPassword.Password != txtConfirmPassword.Password)
            {
                MessageBox.Show("Пароли не совпадают!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_userService.CheckUserExists(txtUsername.Text))
            {
                MessageBox.Show("Пользователь с таким логином уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_userService.CheckEmailExists(txtEmail.Text))
            {
                MessageBox.Show("Пользователь с таким email уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cmbRole.SelectedItem == null)
            {
                MessageBox.Show("Выберите роль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newUser = new User
            {
                LastName = txtLastName.Text.Trim(),
                FirstName = txtFirstName.Text.Trim(),
                Username = txtUsername.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                PasswordHash = _userService.HashPassword(txtPassword.Password),
                IdRole = (int)cmbRole.SelectedValue,
                RegistrationDate = DateOnly.FromDateTime(DateTime.Now),
                Avatar = null
            };

            bool success = _userService.CreateUser(newUser);

            if (success)
            {
                MessageBox.Show($"Пользователь {txtLastName.Text} {txtFirstName.Text} успешно добавлен!",
                              "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}