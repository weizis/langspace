using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Windows;
using System.Windows.Controls;
using LangSpace.Models;
using LangSpace.Service;

namespace LangSpace.Views.Pages
{
    public partial class RegisterPage : Page
    {
        private UserService userService;
        private AuthService authService;

        public RegisterPage()
        {
            InitializeComponent();
            userService = new UserService();
            authService = new AuthService();
        }

        private string GeneratePassword()
        {
            string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string lower = "abcdefghijklmnopqrstuvwxyz";
            string digits = "0123456789";
            string symbols = "!@#$%^";
            Random random = new Random();

            char[] password = new char[8];
            password[0] = upper[random.Next(upper.Length)];
            password[1] = digits[random.Next(digits.Length)];
            password[2] = symbols[random.Next(symbols.Length)];

            string all = upper + lower + digits + symbols;
            for (int i = 3; i < 8; i++)
            {
                password[i] = all[random.Next(all.Length)];
            }

            return new string(password.OrderBy(x => random.Next()).ToArray());
        }

        private void SendEmail(string toEmail, string login, string password, string firstName, string lastName)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("deadinsideking421@gmail.com");
                mail.To.Add(toEmail);
                mail.Subject = "Регистрация в LangSpace";
                mail.Body = $"Здравствуйте, {firstName} {lastName}!\n\n" +
                           $"Вы успешно зарегистрировались в LangSpace.\n\n" +
                           $"Ваш логин: {login}\n" +
                           $"Ваш пароль: {password}\n\n" +
                           $"Сохраните эти данные для входа в приложение.\n\n" +
                           $"С уважением,\nКоманда LangSpace";

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.Credentials = new NetworkCredential("deadinsideking421@gmail.com", "ocaqetblnretlbdl");
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка отправки письма: {ex.Message}\n\nРегистрация выполнена, но письмо не отправлено.");
            }
        }

        private void btnGeneratePassword_Click(object sender, RoutedEventArgs e)
        {
            string password = GeneratePassword();
            txtPassword.Password = password;
            txtConfirmPassword.Password = password;
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            string firstName = txtFirstName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string username = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Password;
            string confirmPassword = txtConfirmPassword.Password;

            if (string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Все поля должны быть заполнены!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cbAgree.IsChecked != true)
            {
                MessageBox.Show("Необходимо согласиться с правилами использования!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!authService.IsValidEmail(email))
            {
                MessageBox.Show("Введите корректный email адрес!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (password.Length < 6)
            {
                MessageBox.Show("Пароль должен содержать не менее 6 символов!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!password.Any(char.IsDigit))
            {
                MessageBox.Show("Пароль должен содержать хотя бы одну цифру!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (userService.CheckUserExists(username))
            {
                MessageBox.Show("Пользователь с таким логином уже существует!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (userService.CheckEmailExists(email))
            {
                MessageBox.Show("Пользователь с таким email уже существует!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newUser = new User
            {
                Username = username,
                Email = email,
                PasswordHash = userService.HashPassword(password),
                FirstName = firstName,
                LastName = lastName,
                RegistrationDate = DateOnly.FromDateTime(DateTime.Now),
                IdRole = 3 
            };

            if (userService.CreateUser(newUser))
            {
                SendEmail(email, username, password, firstName, lastName);
                LoginWindow.currentUser = newUser;

                MessageBox.Show("Регистрация успешна!\n\nПроверьте вашу почту.", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                var result = MessageBox.Show(
                    "Хотите пройти тест на определение уровня языка?\n\nЭто поможет подобрать материалы под ваш уровень.",
                    "Добро пожаловать в LangSpace!",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    LevelTestWindow testWindow = new LevelTestWindow();
                    testWindow.Owner = Window.GetWindow(this);
                    testWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    testWindow.ShowDialog();
                }

                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                Window.GetWindow(this)?.Close();
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow mainWindow = (LoginWindow)Window.GetWindow(this);
            mainWindow?.NavigateToPage(new LoginPage());
        }
    }
}