using AquaPark.Data;
using AquaPark.Services;
using AquaPark.Views;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AquaPark.ViewModel
{
    public class AuthorizationViewModel : BaseViewModel
    {
        private string _login = string.Empty;
        private string _errorMessage = string.Empty;

        public string Login
        {
            get => _login;
            set
            {
                _login = value;
                OnPropertyChanged();
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoginCommand { get; }

        public AuthorizationViewModel()
        {
            LoginCommand = new RelayCommand(Authorize);
        }

        private void Authorize(object? parameter)
        {
            if (parameter is not PasswordBox passwordBox)
            {
                ErrorMessage = "Ошибка получения пароля";
                return;
            }

            string password = passwordBox.Password.Trim();

            if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(password))
            {
                ErrorMessage = "Введите логин и пароль";
                return;
            }

            var user = AppData.db.Users
                .FirstOrDefault(u => u.Login == Login
                                  && u.Password == password
                                  && u.IsActive == true);

            if (user == null)
            {
                ErrorMessage = "Неверный логин или пароль";
                return;
            }

            ErrorMessage = string.Empty;

            MessageBox.Show($"Добро пожаловать, {user.FullName}!",
                            "Успешный вход",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.OpenPage(new MenuPage());
            }
        }
    }
}
