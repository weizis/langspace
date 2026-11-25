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
using System.Windows.Shapes;
using LangSpace.Views.Pages;
using LangSpace.Service;
using LangSpace.Models;

namespace LangSpace.Views
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public static User? currentUser { get => UserSession.CurrentUser; set => UserSession.CurrentUser = value; }
        public LoginWindow()
        {
            InitializeComponent();
           
            MainFrame.Navigate(new LoginPage());
        }
        public void NavigateToPage(Page page)
        {
            MainFrame.Navigate(page);
        }
        
    }
}
