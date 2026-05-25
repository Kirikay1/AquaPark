using AquaPark.Data;
using AquaPark.Models;
using AquaPark.Services;
using AquaPark.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace AquaPark.ViewModel
{
    public class PaymentsViewModel : PagedTableViewModel
    {
        private const string SectionName = "Payments";

        private ObservableCollection<Payment> _payments = null!;
        private ObservableCollection<string> _statuses = null!;
        private Payment _selectedPayment = null!;

        private string _searchText = string.Empty;
        private string _selectedStatus = "Все";
        private DateTime? _dateFrom;
        private DateTime? _dateTo;
        private Visibility _addButtonVisibility = Visibility.Visible;
        private Visibility _editButtonVisibility = Visibility.Visible;
        private Visibility _deleteButtonVisibility = Visibility.Visible;

        public ObservableCollection<Payment> Payments
        {
            get => _payments;
            set
            {
                _payments = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Statuses
        {
            get => _statuses;
            set
            {
                _statuses = value;
                OnPropertyChanged();
            }
        }

        public Payment SelectedPayment
        {
            get => _selectedPayment;
            set
            {
                _selectedPayment = value;
                OnPropertyChanged();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                ResetPage();
                LoadPayments();
            }
        }

        public string SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                _selectedStatus = value;
                OnPropertyChanged();
                ResetPage();
                LoadPayments();
            }
        }

        public DateTime? DateFrom
        {
            get => _dateFrom;
            set
            {
                _dateFrom = value;
                OnPropertyChanged();
                ResetPage();
                LoadPayments();
            }
        }

        public DateTime? DateTo
        {
            get => _dateTo;
            set
            {
                _dateTo = value;
                OnPropertyChanged();
                ResetPage();
                LoadPayments();
            }
        }

        public Visibility AddButtonVisibility
        {
            get => _addButtonVisibility;
            set
            {
                _addButtonVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility EditButtonVisibility
        {
            get => _editButtonVisibility;
            set
            {
                _editButtonVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility DeleteButtonVisibility
        {
            get => _deleteButtonVisibility;
            set
            {
                _deleteButtonVisibility = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand BackCommand { get; }
        public ICommand ClearSearchCommand { get; }

        public PaymentsViewModel()
        {
            AddCommand = new RelayCommand(Add, _ => RoleAccessService.CanAddOrEdit(SectionName));
            EditCommand = new RelayCommand(Edit, _ => RoleAccessService.CanAddOrEdit(SectionName));
            DeleteCommand = new RelayCommand(Delete, _ => RoleAccessService.CanDelete());
            RefreshCommand = new RelayCommand(Refresh);
            BackCommand = new RelayCommand(Back);
            ClearSearchCommand = new RelayCommand(ClearSearch);

            Statuses = new ObservableCollection<string>
            {
                "Все",
                "Оплачено",
                "Частично оплачено",
                "Ожидает оплаты",
                "Отменено"
            };

            SetRoleAccess();
            StatusAutomationService.UpdatePaymentStatuses();
            LoadPayments();
        }

        protected override void LoadPage()
        {
            LoadPayments();
        }

        private void LoadPayments()
        {
            var query = AppData.db.Payments
                .Include(p => p.Sale)
                    .ThenInclude(s => s.Ticket)
                        .ThenInclude(t => t.Client)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                string searchText = SearchText.ToLower();

                query = query.Where(p =>
                    p.Sale.SaleId.ToString().Contains(searchText) ||
                    (p.Sale.Ticket.Client != null && p.Sale.Ticket.Client.FullName.ToLower().Contains(searchText)) ||
                    p.PaymentMethod.ToLower().Contains(searchText) ||
                    p.PaymentStatus.ToLower().Contains(searchText) ||
                    p.Amount.ToString().Contains(searchText));
            }

            if (!string.IsNullOrWhiteSpace(SelectedStatus) && SelectedStatus != "Все")
            {
                query = query.Where(p => p.PaymentStatus == SelectedStatus);
            }

            if (DateFrom.HasValue)
            {
                query = query.Where(p => p.PaymentDate >= DateFrom.Value.Date);
            }

            if (DateTo.HasValue)
            {
                DateTime to = DateTo.Value.Date.AddDays(1);
                query = query.Where(p => p.PaymentDate < to);
            }

            query = ApplyPaging(query.OrderByDescending(p => p.PaymentDate));

            Payments = new ObservableCollection<Payment>(query.ToList());
        }

        private void Add(object? parameter)
        {
            NavigationService.Navigate(new AddPaymentPage());
        }

        private void Edit(object? parameter)
        {
            if (SelectedPayment == null)
            {
                MessageBox.Show("Выберите оплату для изменения",
                                "Изменение",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            NavigationService.Navigate(new EditPaymentPage(SelectedPayment.PaymentId));
        }

        private void Delete(object? parameter)
        {
            if (SelectedPayment == null)
            {
                MessageBox.Show("Выберите оплату для удаления",
                                "Удаление",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить выбранную оплату?",
                                                      "Подтверждение удаления",
                                                      MessageBoxButton.YesNo,
                                                      MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            var payment = AppData.db.Payments
                .FirstOrDefault(p => p.PaymentId == SelectedPayment.PaymentId);

            if (payment == null)
            {
                MessageBox.Show("Оплата не найдена",
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            AppData.db.Payments.Remove(payment);
            AppData.db.SaveChanges();
            AuditService.Log("Удаление", "Оплаты", payment.PaymentId, $"Сумма: {payment.Amount:N2}");

            LoadPayments();

            MessageBox.Show("Оплата успешно удалена",
                            "Удаление",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
        }

        private void Refresh(object? parameter)
        {
            LoadPayments();
        }

        private void ClearSearch(object? parameter)
        {
            SearchText = string.Empty;
            SelectedStatus = "Все";
            DateFrom = null;
            DateTo = null;
        }

        private void Back(object? parameter)
        {
            NavigationService.Navigate(new MenuPage());
        }

        private void SetRoleAccess()
        {
            AddButtonVisibility = RoleAccessService.AddEditVisibility(SectionName);
            EditButtonVisibility = RoleAccessService.AddEditVisibility(SectionName);
            DeleteButtonVisibility = RoleAccessService.DeleteVisibility();
        }
    }
}
