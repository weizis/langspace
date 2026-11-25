using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using LangSpace.Models;
using Microsoft.EntityFrameworkCore;

namespace LangSpace.Service
{
    public class NoteService
    {
        private readonly string _resourcesFolder;

        public NoteService()
        {
            _resourcesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
            if (!Directory.Exists(_resourcesFolder))
            {
                Directory.CreateDirectory(_resourcesFolder);
            }
        }

        public List<Note> GetNotesByUser(int userId)
        {
            if (UserSession.CurrentUser?.IdUser != userId) return new List<Note>();
            using (var context = new LangSpaceContext())
            {
                return context.Notes
                    .Include(n => n.Resources)
                    .Where(n => n.IdUser == userId)
                    .OrderByDescending(n => n.DateCreated)
                    .ThenByDescending(n => n.IdNote)
                    .ToList();
            }
        }

        public string SaveFile(string sourceFilePath, string resourceType)
        {
            try
            {
                if (string.IsNullOrEmpty(sourceFilePath))
                    return "";

                string fileName = Path.GetFileName(sourceFilePath);
                string uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
                string destinationPath = Path.Combine(_resourcesFolder, uniqueFileName);

                File.Copy(sourceFilePath, destinationPath, false);

                return destinationPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return "";
            }
        }

        public bool DeleteFile(string filePath)
        {
            try
            {
                if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении файла: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return false;
        }

        public bool FileExists(string filePath)
        {
            return !string.IsNullOrEmpty(filePath) && File.Exists(filePath);
        }

        public bool CreateNote(Note note, string sourceFilePath, string resourceType)
        {
            if (UserSession.CurrentUser?.IdUser != note.IdUser) return false;
            try
            {
                using (var context = new LangSpaceContext())
                {
                    context.Notes.Add(note);
                    context.SaveChanges();

                    if (resourceType != "Текстовая заметка")
                    {
                        string savedFilePath = "";
                        if (!string.IsNullOrEmpty(sourceFilePath) && File.Exists(sourceFilePath))
                        {
                            savedFilePath = SaveFile(sourceFilePath, resourceType);
                        }

                        var resource = new Resource
                        {
                            IdNote = note.IdNote,
                            ResourceLink = savedFilePath,
                            ResourceType = resourceType,
                            ResourceText = null
                        };
                        context.Resources.Add(resource);
                    }

                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
                return false;
            }
        }

        public List<Note> GetNotesByFilter(int userId, string filter)
        {
            if (UserSession.CurrentUser?.IdUser != userId) return new List<Note>();
            using (var context = new LangSpaceContext())
            {
                var notes = context.Notes
                    .Include(n => n.Resources)
                    .Where(n => n.IdUser == userId)
                    .OrderByDescending(n => n.DateCreated)
                    .ThenByDescending(n => n.IdNote)
                    .ToList();

                if (filter == "Текстовая заметка")
                {
                    return notes.Where(n => n.Resources == null || !n.Resources.Any()).ToList();
                }
                else if (filter != "Все записи")
                {
                    return notes.Where(n => n.Resources.Any(r => r.ResourceType == filter)).ToList();
                }

                return notes;
            }
        }
        public bool DeleteNote(int noteId)
        {
            try
            {
                using (var context = new LangSpaceContext())
                {
                    var note = context.Notes
                        .Include(n => n.Resources)
                        .FirstOrDefault(n => n.IdNote == noteId);

                    if (note != null && UserSession.CurrentUser?.IdUser == note.IdUser)
                    {
                        if (note.Resources != null && note.Resources.Any())
                        {
                            foreach (var resource in note.Resources)
                            {
                                if (!string.IsNullOrEmpty(resource.ResourceLink))
                                {
                                    DeleteFile(resource.ResourceLink);
                                }
                            }
                            context.Resources.RemoveRange(note.Resources);
                        }

                        context.Notes.Remove(note);
                        context.SaveChanges();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления: {ex.Message}\n\n{ex.InnerException?.Message}");
            }
            return false;
        }

        public bool UpdateNote(Note note, string newFilePath, string resourceType)
        {
            try
            {
                using (var context = new LangSpaceContext())
                {
                    var existingNote = context.Notes
                        .Include(n => n.Resources)
                        .FirstOrDefault(n => n.IdNote == note.IdNote);

                    if (existingNote == null || UserSession.CurrentUser?.IdUser != existingNote.IdUser)
                    {
                        MessageBox.Show("Заметка не найдена");
                        return false;
                    }

                    existingNote.NoteText = note.NoteText;

                    var existingResource = existingNote.Resources.FirstOrDefault();

                    if (!string.IsNullOrEmpty(newFilePath) && File.Exists(newFilePath))
                    {
                        if (existingResource != null && !string.IsNullOrEmpty(existingResource.ResourceLink))
                        {
                            DeleteFile(existingResource.ResourceLink);
                        }

                        string savedFilePath = SaveFile(newFilePath, resourceType);

                        if (existingResource != null)
                        {
                            existingResource.ResourceLink = savedFilePath;
                            existingResource.ResourceType = resourceType;
                        }
                        else
                        {
                            var newResource = new Resource
                            {
                                IdNote = note.IdNote,
                                ResourceLink = savedFilePath,
                                ResourceType = resourceType
                            };
                            context.Resources.Add(newResource);
                        }
                    }

                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления: {ex.Message}");
                return false;
            }
        }

        public bool OpenFile(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    MessageBox.Show("Путь к файлу не указан", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                if (File.Exists(filePath))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(filePath)
                    {
                        UseShellExecute = true
                    });
                    return true;
                }
                else
                {
                    MessageBox.Show($"Файл не найден:\n{filePath}\n\nВозможно, файл был перемещен или удален.",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось открыть файл:\n{ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}