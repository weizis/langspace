using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LangSpace.Models;
using LangSpace.Service;

namespace LangSpace.Views.Pages
{
    public partial class AdminPanelPage : Page
    {
        private UserService _userService;
        private User _selectedUser;

        public AdminPanelPage()
        {
            InitializeComponent();
            _userService = new UserService();
            LoadUsers();
            LoadRoles();
        }

        private void LoadUsers()
        {
            var users = _userService.GetAllUsers();
            UsersGrid.ItemsSource = users;
        }

        private void LoadRoles()
        {
            var roles = _userService.GetAllRoles();
            cmbRoles.ItemsSource = roles;
        }

        

        private void ClearUserInfo()
        {
            txtFullName.Text = "-";
            txtUsername.Text = "-";
            txtEmail.Text = "-";
            cmbRoles.SelectedIndex = -1;
       
        }
        private void UsersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedUser = UsersGrid.SelectedItem as User;

            if (_selectedUser != null)
            {
                txtFullName.Text = $"{_selectedUser.LastName} {_selectedUser.FirstName}";
                txtUsername.Text = _selectedUser.Username;
                txtEmail.Text = _selectedUser.Email;

                cmbRoles.SelectedValue = _selectedUser.IdRole;
                
            }
            else
            {
                txtFullName.Text = "-";
                txtUsername.Text = "-";
                txtEmail.Text = "-";
                cmbRoles.SelectedIndex = -1;
               
            }
        }
        private void btnChangeRole_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedUser == null)
            {
                MessageBox.Show("Выберите пользователя!", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cmbRoles.SelectedItem == null)
            {
                MessageBox.Show("Выберите роль!", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int newRoleId = (int)cmbRoles.SelectedValue;

            if (newRoleId == _selectedUser.IdRole)
            {
                MessageBox.Show("Эта роль уже назначена пользователю", "Информация",
                              MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show($"Сменить роль пользователю {_selectedUser.LastName} {_selectedUser.FirstName}?",
                                         "Смена роли",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                bool success = _userService.ChangeUserRole(_selectedUser.IdUser, newRoleId);

                if (success)
                {
                    MessageBox.Show("Роль успешно изменена!", "Успех",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                    int selectedUserId = _selectedUser.IdUser;
                    LoadUsers(); 

                    foreach (var item in UsersGrid.Items)
                    {
                        var user = item as User;
                        if (user != null && user.IdUser == selectedUserId)
                        {
                            UsersGrid.SelectedItem = item;
                            break;
                        }
                    }
                }
            }
        }

        private void btnResetPassword_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedUser == null)
            {
                MessageBox.Show("Сначала выберите пользователя из таблицы!",
                                "Пользователь не выбран",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            var resetWindow = new ResetPasswordWindow(_selectedUser);
            resetWindow.Owner = Window.GetWindow(this);
            resetWindow.ShowDialog();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedUser == null)
            {
                MessageBox.Show("Сначала выберите пользователя из таблицы!",
                                "Пользователь не выбран",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            if (_selectedUser.IdUser == LoginWindow.currentUser?.IdUser)
            {
                MessageBox.Show("Вы не можете удалить самого себя!", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Удалить пользователя {_selectedUser.LastName} {_selectedUser.FirstName}?\n\nВНИМАНИЕ: Все данные пользователя будут безвозвратно удалены!",
                                         "Удаление пользователя",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                bool success = _userService.DeleteUser(_selectedUser.IdUser);

                if (success)
                {
                    MessageBox.Show("Пользователь успешно удален!", "Успех",
                                  MessageBoxButton.OK, MessageBoxImage.Information);

                    LoadUsers(); 
                    ClearUserInfo(); 
                    _selectedUser = null;
                }
            }
        }

        private void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            var addUserWindow = new AddUserWindow();
            addUserWindow.Owner = Window.GetWindow(this);

            if (addUserWindow.ShowDialog() == true)
            {
                LoadUsers(); 
            }
        }
    }
}