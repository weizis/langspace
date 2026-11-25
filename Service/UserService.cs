using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LangSpace.Models;
using Microsoft.EntityFrameworkCore;

namespace LangSpace.Service
{
    public class UserService
    {
        public bool CheckUserExists(string username)
        {
            using (var context = new LangSpaceContext())
            {
                return context.Users.Any(u => u.Username == username);
            }
        }

        public bool CheckEmailExists(string email)
        {
            using (var context = new LangSpaceContext())
            {
                return context.Users.Any(u => u.Email == email);
            }
        }

        public string HashPassword(string password)
        {
            return Verification.HashPassword(password);
        }

        public bool CreateUser(User user)
        {
            try
            {
                using (var context = new LangSpaceContext())
                {
                    context.Users.Add(user);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка создания пользователя: {ex.Message}");
                return false;
            }
        }
        public bool UpdateUserProfile(User user, string newPassword = null)
        {
            try
            {
                using (var context = new LangSpaceContext())
                {
                    var existingUser = context.Users.Find(user.IdUser);
                    if (existingUser == null)
                    {
                        MessageBox.Show("Пользователь не найден");
                        return false;
                    }

                    existingUser.FirstName = user.FirstName;
                    existingUser.LastName = user.LastName;
                    existingUser.Email = user.Email;

                    if (!string.IsNullOrEmpty(user.Avatar))
                    {
                        existingUser.Avatar = user.Avatar;
                    }

                    if (!string.IsNullOrEmpty(newPassword))
                    {
                        existingUser.PasswordHash = HashPassword(newPassword);
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
        public List<User> GetAllUsers()
        {
            using (var context = new LangSpaceContext())
            {
                return context.Users
                    .Include(u => u.IdRoleNavigation)
                    .OrderBy(u => u.IdUser)
                    .ToList();
            }
        }
        public List<Role> GetAllRoles()
        {
            using (var context = new LangSpaceContext())
            {
                return context.Roles.ToList();
            }
        }
        public bool ChangeUserRole(int userId, int newRoleId)
        {
            try
            {
                using (var context = new LangSpaceContext())
                {
                    var user = context.Users.Find(userId);
                    if (user == null)
                    {
                        MessageBox.Show("Пользователь не найден");
                        return false;
                    }

                    user.IdRole = newRoleId;
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка смены роли: {ex.Message}");
                return false;
            }
        }
        public bool ResetPassword(int userId, string newPassword)
        {
            try
            {
                using (var context = new LangSpaceContext())
                {
                    var user = context.Users.Find(userId);
                    if (user == null)
                    {
                        MessageBox.Show("Пользователь не найден");
                        return false;
                    }

                    user.PasswordHash = HashPassword(newPassword);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сброса пароля: {ex.Message}");
                return false;
            }
        }
        public bool DeleteUser(int userId)
        {
            try
            {
                using (var context = new LangSpaceContext())
                {
                    var user = context.Users
                        .Include(u => u.Goals)
                        .Include(u => u.Notes)
                            .ThenInclude(n => n.Resources)
                        .Include(u => u.UserLanguages)
                        .Include(u => u.TestResults)
                        .FirstOrDefault(u => u.IdUser == userId);

                    if (user == null)
                    {
                        MessageBox.Show("Пользователь не найден");
                        return false;
                    }
                    foreach (var note in user.Notes.ToList())
                    {
                        if (note.Resources != null && note.Resources.Any())
                            context.Resources.RemoveRange(note.Resources);
                    }
                    context.Notes.RemoveRange(user.Notes);
                    context.SaveChanges();
                    context.TestResults.RemoveRange(user.TestResults);
                    context.SaveChanges();
                    context.Goals.RemoveRange(user.Goals);
                    context.SaveChanges();
                    context.UserLanguages.RemoveRange(user.UserLanguages);
                    context.SaveChanges();
                    var userCards = context.Cards.Where(c => c.IdUser == userId).ToList();
                    foreach (var card in userCards)
                    {
                        card.IdUser = null;
                    }
                    context.SaveChanges();
                    context.Users.Remove(user);
                    context.SaveChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                string innerMessage = ex.InnerException?.Message ?? "Нет";
                string innerInnerMessage = ex.InnerException?.InnerException?.Message ?? "Нет";

                MessageBox.Show(
                    $"Ошибка удаления пользователя!\n\n" +
                    $"Основная ошибка: {ex.Message}\n\n" +
                    $"Внутренняя ошибка: {innerMessage}\n\n" +
                    $"Вложенная ошибка: {innerInnerMessage}",
                    "Ошибка удаления",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }
        }
        public User GetUserById(int userId)
        {
            using (var context = new LangSpaceContext())
            {
                return context.Users
                    .Include(u => u.IdRoleNavigation)
                    .FirstOrDefault(u => u.IdUser == userId);
            }
        }
    }
}