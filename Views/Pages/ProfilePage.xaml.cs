using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using LangSpace.Models;
using LangSpace.Service;
using Microsoft.Win32;

namespace LangSpace.Views.Pages
{
    public partial class ProfilePage : Page
    {
        private UserService userService;
        private User currentUser;
        private string avatarFileName = null;

        public ProfilePage()
        {
            InitializeComponent();
            userService = new UserService();
            currentUser = LoginWindow.currentUser;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (currentUser != null)
            {
                tbUsername.Text = currentUser.Username;
                tbFirstName.Text = currentUser.FirstName;
                tbLastName.Text = currentUser.LastName;
                tbEmail.Text = currentUser.Email;

                if (!string.IsNullOrEmpty(currentUser.Avatar))
                {
                    string avatarPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", currentUser.Avatar);
                    if (System.IO.File.Exists(avatarPath))
                    {
                        imgAvatar.Source = new BitmapImage(new Uri(avatarPath));
                    }
                }
            }
        }
        private void btnChangeAvatar_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Изображения|*.png;*.jpg;*.jpeg;*.bmp|Все файлы|*.*";
            openFileDialog.Title = "Выберите аватар";

            if (openFileDialog.ShowDialog() == true)
            {
                string sourcePath = openFileDialog.FileName;
                string fileName = System.IO.Path.GetFileName(sourcePath);

                string imagesDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
                if (!System.IO.Directory.Exists(imagesDir))
                {
                    System.IO.Directory.CreateDirectory(imagesDir);
                }

                string destPath = System.IO.Path.Combine(imagesDir, fileName);
                if (!System.IO.File.Exists(destPath))
                {
                    System.IO.File.Copy(sourcePath, destPath);
                }

                avatarFileName = fileName;
                imgAvatar.Source = new BitmapImage(new Uri(destPath));
            }
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (currentUser == null) return;

            if (string.IsNullOrWhiteSpace(tbFirstName.Text))
            {
                MessageBox.Show("Имя обязательно для заполнения!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                tbFirstName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(tbLastName.Text))
            {
                MessageBox.Show("Фамилия обязательна для заполнения!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                tbLastName.Focus();
                return;
            }

            if (!IsValidEmail(tbEmail.Text))
            {
                MessageBox.Show("Введите корректный email адрес!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string newPassword = tbNewPassword.Password;
            string confirmPassword = tbConfirmPassword.Password;
            bool isPasswordChanged = !string.IsNullOrEmpty(newPassword);

            if (isPasswordChanged)
            {
                if (newPassword.Length < 6)
                {
                    MessageBox.Show("Пароль должен содержать не менее 6 символов!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (newPassword != confirmPassword)
                {
                    MessageBox.Show("Пароли не совпадают!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            currentUser.FirstName = tbFirstName.Text;
            currentUser.LastName = tbLastName.Text;
            currentUser.Email = tbEmail.Text;

            if (avatarFileName != null)
            {
                currentUser.Avatar = avatarFileName;
            }

            if (userService.UpdateUserProfile(currentUser, isPasswordChanged ? newPassword : null))
            {
                LoginWindow.currentUser = currentUser;
                MessageBox.Show("Профиль успешно обновлен!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.GoBack();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
                NavigationService.Navigate(new SettingsPage());
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