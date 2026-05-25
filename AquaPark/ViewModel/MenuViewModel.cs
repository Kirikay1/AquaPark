using AquaPark.Data;
using AquaPark.Services;
using AquaPark.Views;
using Microsoft.EntityFrameworkCore;
using System;
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
        private const string SchedulesSection = "Schedules";
        private const string ReportsSection = "Reports";
        private const string UsersSection = "Users";
        private const string LogsSection = "Logs";

        private string _currentUserName = string.Empty;

        private string _currentUserRole = string.Empty;

        private Visibility _clientsVisibility = Visibility.Visible;

        private Visibility _ticketsVisibility = Visibility.Visible;

        private Visibility _attractionsVisibility = Visibility.Visible;

        private Visibility _salesVisibility = Visibility.Visible;

        private Visibility _paymentsVisibility = Visibility.Visible;

        private Visibility _schedulesVisibility = Visibility.Visible;

        private Visibility _reportsVisibility = Visibility.Visible;
        private Visibility _usersVisibility = Visibility.Visible;
        private Visibility _logsVisibility = Visibility.Visible;

        private int _clientsCount;

        private int _ticketsCount;

        private int _salesCount;

        private decimal _paymentsTotalAmount;

        private int _todaySalesCount;

        private decimal _todayPaymentsTotalAmount;

        private int _activeTicketsCount;

        private int _upcomingSchedulesCount;

        private int _unpaidSalesCount;

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

        public Visibility SchedulesVisibility
        {
            get => _schedulesVisibility;
            set
            {
                _schedulesVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility ReportsVisibility
        {
            get => _reportsVisibility;
            set
            {
                _reportsVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility UsersVisibility
        {
            get => _usersVisibility;
            set
            {
                _usersVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility LogsVisibility
        {
            get => _logsVisibility;
            set
            {
                _logsVisibility = value;
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

        public int TodaySalesCount
        {
            get => _todaySalesCount;
            set
            {
                _todaySalesCount = value;
                OnPropertyChanged();
            }
        }

        public decimal TodayPaymentsTotalAmount
        {
            get => _todayPaymentsTotalAmount;
            set
            {
                _todayPaymentsTotalAmount = value;
                OnPropertyChanged();
            }
        }

        public int ActiveTicketsCount
        {
            get => _activeTicketsCount;
            set
            {
                _activeTicketsCount = value;
                OnPropertyChanged();
            }
        }

        public int UpcomingSchedulesCount
        {
            get => _upcomingSchedulesCount;
            set
            {
                _upcomingSchedulesCount = value;
                OnPropertyChanged();
            }
        }

        public int UnpaidSalesCount
        {
            get => _unpaidSalesCount;
            set
            {
                _unpaidSalesCount = value;
                OnPropertyChanged();
            }
        }

        public ICommand ClientsCommand { get; }
        public ICommand TicketsCommand { get; }
        public ICommand AttractionsCommand { get; }
        public ICommand SalesCommand { get; }
        public ICommand PaymentsCommand { get; }
        public ICommand SchedulesCommand { get; }
        public ICommand ReportsCommand { get; }
        public ICommand UsersCommand { get; }
        public ICommand LogsCommand { get; }
        public ICommand LogoutCommand { get; }

        public MenuViewModel()
        {
            ClientsCommand = new RelayCommand(OpenClientsPage);
            TicketsCommand = new RelayCommand(OpenTicketsPage);
            AttractionsCommand = new RelayCommand(OpenAttractionsPage);
            SalesCommand = new RelayCommand(OpenSalesPage);
            PaymentsCommand = new RelayCommand(OpenPaymentsPage);
            SchedulesCommand = new RelayCommand(OpenSchedulesPage);
            ReportsCommand = new RelayCommand(OpenReportsPage);
            UsersCommand = new RelayCommand(_ => NavigationService.Navigate(new UsersPage()));
            LogsCommand = new RelayCommand(_ => NavigationService.Navigate(new ActionLogsPage()));
            LogoutCommand = new RelayCommand(Logout);

            StatusAutomationService.UpdateOperationalStatuses();
            LoadCurrentUser();
            SetRoleAccess();
            LoadStatistics();
        }

        private void OpenClientsPage(object? parameter)
        {
            NavigationService.Navigate(new ClientsPage());
        }

        private void OpenTicketsPage(object? parameter)
        {
            NavigationService.Navigate(new TicketsPage());
        }

        private void OpenAttractionsPage(object? parameter)
        {
            NavigationService.Navigate(new AttractionsPage());
        }

        private void OpenSalesPage(object? parameter)
        {
            NavigationService.Navigate(new SalesPage());
        }

        private void OpenPaymentsPage(object? parameter)
        {
            NavigationService.Navigate(new PaymentsPage());
        }

        private void OpenSchedulesPage(object? parameter)
        {
            NavigationService.Navigate(new AttractionSchedulesPage());
        }

        private void OpenReportsPage(object? parameter)
        {
            NavigationService.Navigate(new ReportsPage());
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

            AuditService.Log("Выход", "Авторизация", null, "Выход из аккаунта");
            AppData.CurrentUser = null;

            NavigationService.Navigate(new AuthorizationPage());
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
            SchedulesVisibility = RoleAccessService.CanOpenMenuSection(SchedulesSection)
                ? Visibility.Visible
                : Visibility.Collapsed;
            ReportsVisibility = RoleAccessService.CanOpenMenuSection(ReportsSection)
                ? Visibility.Visible
                : Visibility.Collapsed;
            UsersVisibility = RoleAccessService.CanOpenMenuSection(UsersSection)
                ? Visibility.Visible
                : Visibility.Collapsed;
            LogsVisibility = RoleAccessService.CanOpenMenuSection(LogsSection)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void LoadStatistics()
        {
            DateTime today = DateTime.Today;
            DateTime tomorrow = today.AddDays(1);
            DateOnly todayDate = DateOnly.FromDateTime(today);

            ClientsCount = AppData.db.Clients.Count();
            TicketsCount = AppData.db.Tickets.Count();
            SalesCount = AppData.db.Sales.Count();
            PaymentsTotalAmount = AppData.db.Payments.Sum(p => (decimal?)p.Amount) ?? 0;
            TodaySalesCount = AppData.db.Sales.Count(s => s.SaleDate >= today && s.SaleDate < tomorrow);
            TodayPaymentsTotalAmount = AppData.db.Payments
                .Where(p => p.PaymentDate >= today && p.PaymentDate < tomorrow)
                .Sum(p => (decimal?)p.Amount) ?? 0;
            ActiveTicketsCount = AppData.db.Tickets.Count(t => t.Status == "Активен");
            UpcomingSchedulesCount = AppData.db.AttractionSchedules.Count(s => s.WorkDate >= todayDate && s.Status == "Работает");
            UnpaidSalesCount = AppData.db.Sales
                .Include(s => s.Payments)
                .AsEnumerable()
                .Count(s => s.Payments.Sum(p => p.Amount) < s.TotalAmount);
        }
    }
}
