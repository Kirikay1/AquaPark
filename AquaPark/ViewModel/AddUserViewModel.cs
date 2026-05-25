using AquaPark.Data;
using AquaPark.Models;
using AquaPark.Services;
using AquaPark.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace AquaPark.ViewModel
{
    public class AddUserViewModel : BaseViewModel
    {
        private ObservableCollection<Role> _roles = null!;
        private Role _selectedRole = null!;
        private string _login = string.Empty;
        private string _password = string.Empty;
        private string _fullName = string.Empty;
        private string _email = string.Empty;
        private string _phone = string.Empty;
        private bool _isActive = true;
        private bool _isEmployee;
        private string _position = string.Empty;
        private DateTime? _hireDate = DateTime.Today;
        private decimal? _salary;
        private string _errorMessage = string.Empty;

        public ObservableCollection<Role> Roles { get => _roles; set { _roles = value; OnPropertyChanged(); } }
        public Role SelectedRole { get => _selectedRole; set { _selectedRole = value; OnPropertyChanged(); } }
        public string Login { get => _login; set { _login = value; OnPropertyChanged(); } }
        public string Password { get => _password; set { _password = value; OnPropertyChanged(); } }
        public string FullName { get => _fullName; set { _fullName = value; OnPropertyChanged(); } }
        public string Email { get => _email; set { _email = value; OnPropertyChanged(); } }
        public string Phone { get => _phone; set { _phone = value; OnPropertyChanged(); } }
        public bool IsActive { get => _isActive; set { _isActive = value; OnPropertyChanged(); } }
        public bool IsEmployee { get => _isEmployee; set { _isEmployee = value; OnPropertyChanged(); } }
        public string Position { get => _position; set { _position = value; OnPropertyChanged(); } }
        public DateTime? HireDate { get => _hireDate; set { _hireDate = value; OnPropertyChanged(); } }
        public decimal? Salary { get => _salary; set { _salary = value; OnPropertyChanged(); } }
        public string ErrorMessage { get => _errorMessage; set { _errorMessage = value; OnPropertyChanged(); } }

        public ICommand SaveCommand { get; }
        public ICommand BackCommand { get; }

        public AddUserViewModel()
        {
            SaveCommand = new RelayCommand(Save, _ => RoleAccessService.CanManageUsers());
            BackCommand = new RelayCommand(_ => NavigationService.Navigate(new UsersPage()));

            Roles = new ObservableCollection<Role>(AppData.db.Roles.ToList());
            SelectedRole = Roles.FirstOrDefault()!;
            EnableUnsavedChangesTracking();
        }

        protected virtual void Save(object? parameter)
        {
            if (!Validate())
            {
                return;
            }

            User user = new User
            {
                Login = Login.Trim(),
                Password = Password.Trim(),
                FullName = FullName.Trim(),
                Email = string.IsNullOrWhiteSpace(Email) ? null : Email.Trim(),
                Phone = string.IsNullOrWhiteSpace(Phone) ? null : Phone.Trim(),
                RoleId = SelectedRole.RoleId,
                IsActive = IsActive
            };

            if (IsEmployee)
            {
                user.Employee = new Employee
                {
                    Position = Position.Trim(),
                    HireDate = DateOnly.FromDateTime(HireDate!.Value),
                    Salary = Salary
                };
            }

            AppData.db.Users.Add(user);

            if (!DatabaseErrorService.TrySaveChanges("Пользователь успешно добавлен"))
            {
                return;
            }

            AuditService.Log("Добавление", "Пользователи", user.UserId, user.FullName);
            MarkAsSaved();
            NavigationService.Navigate(new UsersPage());
        }

        protected virtual bool Validate()
        {
            ClearErrors();
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Login)) SetError(nameof(Login), "Введите логин");
            if (string.IsNullOrWhiteSpace(Password)) SetError(nameof(Password), "Введите пароль");
            if (string.IsNullOrWhiteSpace(FullName)) SetError(nameof(FullName), "Введите ФИО");
            if (SelectedRole == null) SetError(nameof(SelectedRole), "Выберите роль");

            if (!string.IsNullOrWhiteSpace(Login) && IsLoginDuplicate())
            {
                SetError(nameof(Login), "Пользователь с таким логином уже существует");
            }

            if (IsEmployee)
            {
                if (string.IsNullOrWhiteSpace(Position)) SetError(nameof(Position), "Введите должность");
                if (!HireDate.HasValue) SetError(nameof(HireDate), "Выберите дату приема");
                if (Salary.HasValue && Salary.Value < 0) SetError(nameof(Salary), "Зарплата не может быть отрицательной");
            }

            if (HasErrors)
            {
                ErrorMessage = "Проверьте подсвеченные поля";
                return false;
            }

            return true;
        }

        protected virtual bool IsLoginDuplicate()
        {
            return AppData.db.Users.Any(u => u.Login == Login.Trim());
        }
    }
}
