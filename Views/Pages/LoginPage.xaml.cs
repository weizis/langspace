using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LangSpace.Models;
using LangSpace.Service;
using LangSpace.Views;

namespace LangSpace.Views.Pages
{
 
    public partial class LoginPage : Page
    {
        AuthService authService = new AuthService();
       
        public LoginPage()
        {
            InitializeComponent();
           
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow mainWindow = (LoginWindow)Window.GetWindow(this);
            mainWindow.NavigateToPage(new RegisterPage());
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                MessageBox.Show("Введите имя пользователя и пароль!");
                return;
            }

            var user = authService.GetUser(txtUsername.Text, txtPassword.Password);
            if (user != null)
            {
                LoginWindow.currentUser = user;
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();

                Window parentWindow = Window.GetWindow(this);
                parentWindow?.Close();
            }
            else
            {
                txtPassword.Password = "";
            }
        }
    }
}
