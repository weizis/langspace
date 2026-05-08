using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using LangSpace.Models;
using LangSpace.Service;
using LangSpace.ViewModel;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace LangSpace.Views.Pages
{
    
    public partial class NotesPage : Page
    {
        private readonly NoteService noteService;
        private readonly NotesPageViewModel viewModel;
        private bool navigationHooked;

        public NotesPage()
        {
            InitializeComponent();
            noteService = new NoteService();
            viewModel = new NotesPageViewModel();
            DataContext = viewModel;
        }

        private void btnNewNote_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new NoteEditPage("Текстовая заметка"));
        }

        private void btnAddResource_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new NoteEditPage("Файл"));
        }

        private void btnPicNotes_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new NoteEditPage("Изображение"));
        }

        private void cmbFilterType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void EditNote_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Note note = btn?.Tag as Note;
            if (note != null)
            {
                NoteEditPage editPage = new NoteEditPage(note);
                NavigationService.Navigate(editPage);
            }
        }

        private void DeleteNote_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Note note = btn?.Tag as Note;
            if (note == null) return;

            string warningText = "Вы действительно хотите удалить заметку?";
            MessageBoxResult result = MessageBox.Show(warningText, "Предупреждение",
                MessageBoxButton.OKCancel, MessageBoxImage.Warning);

            if (result == MessageBoxResult.OK)
            {
                noteService.DeleteNote(note.IdNote);
                viewModel.Reload();
            }
        }

        private void BackButton_Click(object sender, MouseButtonEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
                NavigationService.Navigate(new DashboardPage());
        }

        private void Page_Loaded_1(object sender, RoutedEventArgs e)
        {
            viewModel.LoadData();

            if (!navigationHooked && NavigationService != null)
            {
                navigationHooked = true;
                NavigationService.Navigated += (_, __) => viewModel.LoadData();
            }
        }

        private void OpenResource_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Note note = btn?.Tag as Note;

            if (note == null || note.Resources == null || !note.Resources.Any())
            {
                MessageBox.Show("Нет прикрепленного файла", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var resource = note.Resources.FirstOrDefault();
            if (resource != null)
            {
                noteService.OpenFile(resource.ResourceLink);
            }
        }
    }
    
}