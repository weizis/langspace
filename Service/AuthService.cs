using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LangSpace.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace LangSpace.Service
{
    public class AuthService
    {
        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;
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
        public User GetUser(string username, string password)
        {
            User user = null;
            try
            {
                using (var context = new LangSpaceContext())
                {
                    var foundUser = context.Users
                    .Include(u => u.IdRoleNavigation)
                    .FirstOrDefault(u => u.Username == username);
                    if (foundUser != null)
                    {
                        var verification = Verification.VerifyPassword(foundUser, password);
                        if (verification != PasswordVerificationResult.Failed)
                        {
                            if (verification == PasswordVerificationResult.SuccessRehashNeeded)
                            {
                                foundUser.PasswordHash = Verification.HashPassword(password);
                                context.SaveChanges();
                            }
                            user = foundUser;
                        }
                        else
                        {
                            MessageBox.Show("Неверный логин или пароль");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Неверный логин или пароль");
                    }
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return user;
        }
        public bool Register(string username, string email, string password, string confirmPassword) {
            if (!IsValidEmail(email))
            {
                MessageBox.Show("Введите корректный email адрес");
                return false;
            }
            if (password != confirmPassword) {
                MessageBox.Show("Пароли не совпадают");
                return false;
            }
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Все поля должны быть заполнены");
                return false;
            }
            if (password.Length < 6)
            {
                MessageBox.Show("Пароль должен содержать не менее 6 символов");
                return false;
            }
            if (!password.Any(char.IsDigit))
            {
              MessageBox.Show("Пароль должен содержать хотя бы одну цифру");
                return false;
            }
            try
            {
                using (var context = new LangSpaceContext())
                {
                    var existingUser = context.Users
               .FirstOrDefault(u => u.Username == username || u.Email == email);
                    if (existingUser != null)
                    {
                        if (existingUser.Username == username)
                            MessageBox.Show("Пользователь с таким именем уже существует");
                        else
                            MessageBox.Show("Пользователь с таким email уже существует");
                        return false;
                    }
                    var defaultRole = context.Roles.FirstOrDefault(r => r.RoleName == "Student");
                    if (defaultRole == null)
                    {
                        MessageBox.Show("Роль по умолчанию не найдена");
                        return false;
                    }
                    var newUser = new User
                    {
                        Username = username,
                        Email = email,
                        PasswordHash = Verification.HashPassword(password),
                        RegistrationDate = DateOnly.FromDateTime(DateTime.Now),
                        IdRole = defaultRole.IdRole
                    };

                    context.Users.Add(newUser);
                    context.SaveChanges();
                    MessageBox.Show("Регистрация успешна!");
                    return true;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return false;
           
        }
        
    }
}
