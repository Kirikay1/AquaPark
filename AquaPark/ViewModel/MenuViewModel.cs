using AquaPark.Data;
using AquaPark.Services;
using AquaPark.Views;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace AquaPark.ViewModel
{
    public class MenuViewModel : BaseViewModel
    {
        private const string ClientsSection = "Clients";
        private const string TicketsSection = "Tickets";
        private const string AttractionsSection = "Attractions";
        private const string SalesSection = "Sales";
        private const string PaymentsSection = "Payments";

        private string _currentUserName = string.Empty;

        private string _currentUserRole = string.Empty;

        private Visibility _clientsVisibility = Visibility.Visible;

        private Visibility _ticketsVisibility = Visibility.Visible;

        private Visibility _attractionsVisibility = Visibility.Visible;

        private Visibility _salesVisibility = Visibility.Visible;

        private Visibility _paymentsVisibility = Visibility.Visible;

        private int _clientsCount;

        private int _ticketsCount;

        private int _salesCount;

        private decimal _paymentsTotalAmount;

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

        public Visibility ClientsVisibility
        {
            get => _clientsVisibility;
            set
            {
                _clientsVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility TicketsVisibility
        {
            get => _ticketsVisibility;
            set
            {
                _ticketsVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility AttractionsVisibility
        {
            get => _attractionsVisibility;
            set
            {
                _attractionsVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility SalesVisibility
        {
            get => _salesVisibility;
            set
            {
                _salesVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility PaymentsVisibility
        {
            get => _paymentsVisibility;
            set
            {
                _paymentsVisibility = value;
                OnPropertyChanged();
            }
        }

        public int ClientsCount
        {
            get => _clientsCount;
            set
            {
                _clientsCount = value;
                OnPropertyChanged();
            }
        }

        public int TicketsCount
        {
            get => _ticketsCount;
            set
            {
                _ticketsCount = value;
                OnPropertyChanged();
            }
        }

        public int SalesCount
        {
            get => _salesCount;
            set
            {
                _salesCount = value;
                OnPropertyChanged();
            }
        }

        public decimal PaymentsTotalAmount
        {
            get => _paymentsTotalAmount;
            set
            {
                _paymentsTotalAmount = value;
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
            SetRoleAccess();
            LoadStatistics();
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

        private void SetRoleAccess()
        {
            ClientsVisibility = RoleAccessService.CanOpenMenuSection(ClientsSection)
                ? Visibility.Visible
                : Visibility.Collapsed;
            TicketsVisibility = RoleAccessService.CanOpenMenuSection(TicketsSection)
                ? Visibility.Visible
                : Visibility.Collapsed;
            AttractionsVisibility = RoleAccessService.CanOpenMenuSection(AttractionsSection)
                ? Visibility.Visible
                : Visibility.Collapsed;
            SalesVisibility = RoleAccessService.CanOpenMenuSection(SalesSection)
                ? Visibility.Visible
                : Visibility.Collapsed;
            PaymentsVisibility = RoleAccessService.CanOpenMenuSection(PaymentsSection)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void LoadStatistics()
        {
            ClientsCount = AppData.db.Clients.Count();
            TicketsCount = AppData.db.Tickets.Count();
            SalesCount = AppData.db.Sales.Count();
            PaymentsTotalAmount = AppData.db.Payments.Sum(p => (decimal?)p.Amount) ?? 0;
        }
    }
}
