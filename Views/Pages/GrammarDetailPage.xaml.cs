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

namespace LangSpace.Views.Pages
{
    public partial class GrammarDetailPage : Page
    {
        private GrammarRule _rule;
        public GrammarDetailPage(GrammarRule rule)
        {
            InitializeComponent();
            _rule = rule;
            DataContext = _rule;
        }

        private void btnStartTest_Click(object sender, RoutedEventArgs e)
        {
            TestsPage testPage = new TestsPage(_rule.IdRule);
            NavigationService.Navigate(testPage);
        }

        private void BackButton_Click(object sender, MouseButtonEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
                NavigationService.Navigate(new GrammarPage());
        }
    }
}