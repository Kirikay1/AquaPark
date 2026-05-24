using AquaPark.Data;
using AquaPark.Services;
using AquaPark.Views;
using System.Drawing.Interop;
using System.Windows;
using System.Windows.Input;

namespace AquaPark.ViewModel
{
    public class MenuViewModel : BaseViewModel
    {
        private string _currentUserName = string.Empty;

        private string _currentUserRole = string.Empty;

        public string CurrentUserName
        {
            get => _currentUserName;
            set
            {
                _currentUserName = value;
                OnPropertyChanged();
            }
        }

        public string CurrentUserRole
        {
            get => _currentUserRole;
            set
            {
                _currentUserRole = value;
                OnPropertyChanged();
            }
        }

        public ICommand ClientsCommand { get; }
        public ICommand TicketsCommand { get; }
        public ICommand AttractionsCommand { get; }
        public ICommand SalesCommand { get; }
        public ICommand PaymentsCommand { get; }
        public ICommand LogoutCommand { get; }

        public MenuViewModel()
        {
            ClientsCommand = new RelayCommand(OpenClientsPage);
            TicketsCommand = new RelayCommand(OpenTicketsPage);
            AttractionsCommand = new RelayCommand(OpenAttractionsPage);
            SalesCommand = new RelayCommand(OpenSalesPage);
            PaymentsCommand = new RelayCommand(OpenPaymentsPage);
            LogoutCommand = new RelayCommand(Logout);

            LoadCurrentUser();
        }

        private void OpenClientsPage(object? parameter)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.OpenPage(new ClientsPage());
            }
        }

        private void OpenTicketsPage(object? parameter)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.OpenPage(new TicketsPage());
            }
        }

        private void OpenAttractionsPage(object? parameter)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.OpenPage(new AttractionsPage());
            }
        }

        private void OpenSalesPage(object? parameter)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.OpenPage(new SalesPage());
            }
        }

        private void OpenPaymentsPage(object? parameter)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.OpenPage(new PaymentsPage());
            }
        }

        private void Logout(object? parameter)
        {
            MessageBoxResult result = MessageBox.Show("Вы действительно хотите выйти из аккаунта?",
                                                      "Выход",
                                                      MessageBoxButton.YesNo,
                                                      MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            AppData.CurrentUser = null;

            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.OpenPage(new AuthorizationPage());
            }
        }

        private void LoadCurrentUser()
        {
            if (AppData.CurrentUser == null)
            {
                CurrentUserName = "Пользователь не определен";
                CurrentUserRole = "";
                return;
            }

            CurrentUserName = AppData.CurrentUser.FullName;
            CurrentUserRole = AppData.CurrentUser.Role?.RoleName ?? "Роль не указана";
        }
    }
}