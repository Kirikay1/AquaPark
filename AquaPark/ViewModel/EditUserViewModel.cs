using AquaPark.Data;
using AquaPark.Models;
using AquaPark.Services;
using AquaPark.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AquaPark.ViewModel
{
    public class EditUserViewModel : AddUserViewModel
    {
        private readonly int _userId;

        public EditUserViewModel(int userId)
        {
            _userId = userId;
            LoadUser();
            MarkAsSaved();
        }

        private void LoadUser()
        {
            var user = AppData.db.Users
                .Include(u => u.Employee)
                .FirstOrDefault(u => u.UserId == _userId);

            if (user == null)
            {
                ErrorMessage = "Пользователь не найден";
                return;
            }

            Login = user.Login;
            Password = user.Password;
            FullName = user.FullName;
            Email = user.Email ?? string.Empty;
            Phone = user.Phone ?? string.Empty;
            IsActive = user.IsActive;
            SelectedRole = Roles.FirstOrDefault(r => r.RoleId == user.RoleId)!;

            if (user.Employee != null)
            {
                IsEmployee = true;
                Position = user.Employee.Position;
                HireDate = user.Employee.HireDate.ToDateTime(TimeOnly.MinValue);
                Salary = user.Employee.Salary;
            }
        }

        protected override void Save(object? parameter)
        {
            if (!Validate())
            {
                return;
            }

            var user = AppData.db.Users
                .Include(u => u.Employee)
                .FirstOrDefault(u => u.UserId == _userId);

            if (user == null)
            {
                ErrorMessage = "Пользователь не найден";
                return;
            }

            user.Login = Login.Trim();
            user.Password = Password.Trim();
            user.FullName = FullName.Trim();
            user.Email = string.IsNullOrWhiteSpace(Email) ? null : Email.Trim();
            user.Phone = string.IsNullOrWhiteSpace(Phone) ? null : Phone.Trim();
            user.RoleId = SelectedRole.RoleId;
            user.IsActive = IsActive;

            if (IsEmployee)
            {
                if (user.Employee == null)
                {
                    user.Employee = new Employee { UserId = user.UserId };
                    AppData.db.Employees.Add(user.Employee);
                }

                user.Employee.Position = Position.Trim();
                user.Employee.HireDate = DateOnly.FromDateTime(HireDate!.Value);
                user.Employee.Salary = Salary;
            }
            else if (user.Employee != null)
            {
                AppData.db.Employees.Remove(user.Employee);
            }

            if (!DatabaseErrorService.TrySaveChanges("Данные пользователя успешно изменены"))
            {
                return;
            }

            AuditService.Log("Изменение", "Пользователи", user.UserId, user.FullName);
            MarkAsSaved();
            NavigationService.Navigate(new UsersPage());
        }

        protected override bool IsLoginDuplicate()
        {
            return AppData.db.Users.Any(u => u.Login == Login.Trim() && u.UserId != _userId);
        }
    }
}
