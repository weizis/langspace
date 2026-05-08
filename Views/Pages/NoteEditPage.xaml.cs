using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using LangSpace.Models;
using LangSpace.Service;
using Microsoft.Win32;

namespace LangSpace.Views.Pages
{
    public partial class NoteEditPage : Page
    {
        private NoteService noteService;
        private int currentUserId;
        private Note currentNote;
        private string selectedFilePath;
        private bool isEditMode = false;
        private string noteType;

        public NoteEditPage(string type)
        {
            InitializeComponent();
            noteService = new NoteService();
            currentUserId = LoginWindow.currentUser.IdUser;
            noteType = type;
            isEditMode = false;

            tbTitle.Text = type == "Текстовая заметка" ? "Новая заметка" : (type == "Файл" ? "Добавить файл" : "Добавить изображение");

            if (type == "Файл")
            {
                filePanel.Visibility = Visibility.Visible;
                imagePanel.Visibility = Visibility.Collapsed;
                btnSelectFile.Visibility = Visibility.Visible;
                btnSelectFile.Content = "Выбрать файл";
            }
            else if (type == "Изображение")
            {
                filePanel.Visibility = Visibility.Collapsed;
                imagePanel.Visibility = Visibility.Visible;
                btnSelectFile.Visibility = Visibility.Visible;
                btnSelectFile.Content = "Выбрать изображение";
            }
            else
            {
                filePanel.Visibility = Visibility.Collapsed;
                imagePanel.Visibility = Visibility.Collapsed;
                btnSelectFile.Visibility = Visibility.Collapsed;
            }

            currentNote = new Note
            {
                IdUser = currentUserId,
                DateCreated = DateOnly.FromDateTime(DateTime.Now)
            };
        }

        public NoteEditPage(Note note)
        {
            InitializeComponent();
            noteService = new NoteService();
            currentUserId = LoginWindow.currentUser.IdUser;
            currentNote = note;
            isEditMode = true;
            using (var context = new LangSpaceContext())
            {
                var resource = context.Resources.FirstOrDefault(r => r.IdNote == note.IdNote);
                noteType = resource?.ResourceType ?? "Текстовая заметка";
            }

            tbTitle.Text = "Редактировать заметку";
            btnDelete.Visibility = Visibility.Visible;

            LoadNoteData();
        }

        private void LoadNoteData()
        {

            tbNoteText.Text = currentNote.NoteText;
            if (noteType == "Текстовая заметка")
            {
                btnSelectFile.Visibility = Visibility.Collapsed;
                filePanel.Visibility = Visibility.Collapsed;
                imagePanel.Visibility = Visibility.Collapsed;
            }

            using (var context = new LangSpaceContext())
            {
                var resource = context.Resources.FirstOrDefault(r => r.IdNote == currentNote.IdNote);

                if (resource != null && !string.IsNullOrEmpty(resource.ResourceLink))
                {
                    selectedFilePath = resource.ResourceLink;

                    if (resource.ResourceType == "Файл")
                    {
                        filePanel.Visibility = Visibility.Visible;
                        imagePanel.Visibility = Visibility.Collapsed;
                        tbFileName.Text = Path.GetFileName(resource.ResourceLink);
                        btnSelectFile.Visibility = Visibility.Collapsed;
                        btnReplaceFile.Visibility = Visibility.Visible;
                    }
                    else if (resource.ResourceType == "Изображение")
                    {
                        filePanel.Visibility = Visibility.Collapsed;
                        imagePanel.Visibility = Visibility.Visible;
                        tbImageName.Text = Path.GetFileName(resource.ResourceLink);
                        btnSelectFile.Visibility = Visibility.Collapsed;

                        if (noteService.FileExists(resource.ResourceLink))
                        {
                            try
                            {
                                BitmapImage bitmap = new BitmapImage();
                                bitmap.BeginInit();
                                bitmap.UriSource = new Uri(resource.ResourceLink);
                                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                bitmap.EndInit();
                                imgPreview.Source = bitmap;
                            }
                            catch { }
                        }
                    }
                }
                else
                {
                    // Показываем кнопку выбора ТОЛЬКО если это не текстовая заметка
                    if (noteType != "Текстовая заметка")
                    {
                        btnSelectFile.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private void btnSelectFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (noteType == "Изображение")
            {
                openFileDialog.Filter = "Изображения|*.png;*.jpg;*.jpeg;*.bmp|Все файлы|*.*";
            }
            else
            {
                openFileDialog.Filter = "Все файлы|*.*";
            }

            if (openFileDialog.ShowDialog() == true)
            {
                selectedFilePath = openFileDialog.FileName;
                string fileName = Path.GetFileName(selectedFilePath);

                if (noteType == "Изображение")
                {
                    tbImageName.Text = fileName;
                    try
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(selectedFilePath);
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        imgPreview.Source = bitmap;
                    }
                    catch { }

                    btnSelectFile.Visibility = Visibility.Collapsed;
                }
                else
                {
                    tbFileName.Text = fileName;
                    btnSelectFile.Visibility = Visibility.Collapsed;
                    btnReplaceFile.Visibility = Visibility.Visible;
                }
            }
        }

        private void OpenFile_Click(object sender, MouseButtonEventArgs e)
        {
            if (!string.IsNullOrEmpty(selectedFilePath) && File.Exists(selectedFilePath))
            {
                noteService.OpenFile(selectedFilePath);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbNoteText.Text))
            {
                MessageBox.Show("Введите текст заметки");
                return;
            }

            currentNote.NoteText = tbNoteText.Text;

            bool success;
            if (isEditMode)
            {
                string existingType = noteType;
                using (var context = new LangSpaceContext())
                {
                    var resource = context.Resources.FirstOrDefault(r => r.IdNote == currentNote.IdNote);
                    if (resource != null)
                    {
                        existingType = resource.ResourceType;
                    }
                }
                success = noteService.UpdateNote(currentNote, selectedFilePath, existingType);
            }
            else
            {
                string resourcePath = selectedFilePath ?? "";
                success = noteService.CreateNote(currentNote, resourcePath, noteType);
            }

            if (success)
            {
                MessageBox.Show(isEditMode ? "Обновлено" : "Сохранено");
                NavigationService.GoBack();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Удалить заметку?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (noteService.DeleteNote(currentNote.IdNote))
                {
                    MessageBox.Show("Удалено");
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