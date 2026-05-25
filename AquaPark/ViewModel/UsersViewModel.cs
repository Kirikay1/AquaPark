using AquaPark.Data;
using AquaPark.Models;
using AquaPark.Services;
using AquaPark.Views;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace AquaPark.ViewModel
{
    public class UsersViewModel : PagedTableViewModel
    {
        private ObservableCollection<User> _users = null!;
        private User _selectedUser = null!;
        private string _searchText = string.Empty;

        public ObservableCollection<User> Users
        {
            get => _users;
            set { _users = value; OnPropertyChanged(); }
        }

        public User SelectedUser
        {
            get => _selectedUser;
            set { _selectedUser = value; OnPropertyChanged(); }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                ResetPage();
                LoadUsers();
            }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand BackCommand { get; }

        public UsersViewModel()
        {
            AddCommand = new RelayCommand(_ => NavigationService.Navigate(new AddUserPage()), _ => RoleAccessService.CanManageUsers());
            EditCommand = new RelayCommand(Edit, _ => RoleAccessService.CanManageUsers());
            DeleteCommand = new RelayCommand(Delete, _ => RoleAccessService.CanManageUsers());
            RefreshCommand = new RelayCommand(_ => LoadUsers());
            ClearSearchCommand = new RelayCommand(_ => SearchText = string.Empty);
            BackCommand = new RelayCommand(_ => NavigationService.Navigate(new MenuPage()));

            LoadUsers();
        }

        protected override void LoadPage()
        {
            LoadUsers();
        }

        private void LoadUsers()
        {
            var query = AppData.db.Users
                .Include(u => u.Role)
                .Include(u => u.Employee)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                string searchText = SearchText.ToLower();

                query = query.Where(u =>
                    u.Login.ToLower().Contains(searchText) ||
                    u.FullName.ToLower().Contains(searchText) ||
                    (u.Email != null && u.Email.ToLower().Contains(searchText)) ||
                    (u.Phone != null && u.Phone.ToLower().Contains(searchText)) ||
                    u.Role.RoleName.ToLower().Contains(searchText) ||
                    (u.Employee != null && u.Employee.Position.ToLower().Contains(searchText)));
            }

            query = ApplyPaging(query.OrderBy(u => u.UserId));
            Users = new ObservableCollection<User>(query.ToList());
        }

        private void Edit(object? parameter)
        {
            if (SelectedUser == null)
            {
                MessageBox.Show("Выберите пользователя для изменения",
                                "Изменение",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            NavigationService.Navigate(new EditUserPage(SelectedUser.UserId));
        }

        private void Delete(object? parameter)
        {
            if (SelectedUser == null)
            {
                MessageBox.Show("Выберите пользователя для удаления",
                                "Удаление",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            if (AppData.CurrentUser?.UserId == SelectedUser.UserId)
            {
                MessageBox.Show("Нельзя удалить текущего пользователя",
                                "Удаление",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult result = MessageBox.Show("Удалить выбранного пользователя?",
                                                      "Подтверждение удаления",
                                                      MessageBoxButton.YesNo,
                                                      MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            var user = AppData.db.Users
                .Include(u => u.Employee)
                .FirstOrDefault(u => u.UserId == SelectedUser.UserId);

            if (user == null)
            {
                MessageBox.Show("Пользователь не найден",
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            if (user.Employee != null)
            {
                AppData.db.Employees.Remove(user.Employee);
            }

            AppData.db.Users.Remove(user);

            if (!DatabaseErrorService.TrySaveChanges("Пользователь успешно удален"))
            {
                return;
            }

            AuditService.Log("Удаление", "Пользователи", user.UserId, user.FullName);
            LoadUsers();
        }
    }
}
