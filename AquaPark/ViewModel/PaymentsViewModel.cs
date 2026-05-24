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
    public class PaymentsViewModel : BaseViewModel
    {
        private const string SectionName = "Payments";

        private ObservableCollection<Payment> _payments = null!;
        private Payment _selectedPayment = null!;

        private string _searchText = string.Empty;
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

            SetRoleAccess();
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
                query = query.Where(p =>
                    p.Sale.SaleId.ToString().Contains(SearchText) ||
                    p.Sale.Ticket.Client.FullName.Contains(SearchText) ||
                    p.PaymentMethod.Contains(SearchText) ||
                    p.PaymentStatus.Contains(SearchText) ||
                    p.Amount.ToString().Contains(SearchText));
            }

            Payments = new ObservableCollection<Payment>(
                query.ToList()
            );
        }

        private void Add(object? parameter)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.OpenPage(new AddPaymentPage());
            }
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

            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.OpenPage(new EditPaymentPage(SelectedPayment.PaymentId));
            }
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
        }

        private void Back(object? parameter)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.OpenPage(new MenuPage());
            }
        }

        private void SetRoleAccess()
        {
            AddButtonVisibility = RoleAccessService.AddEditVisibility(SectionName);
            EditButtonVisibility = RoleAccessService.AddEditVisibility(SectionName);
            DeleteButtonVisibility = RoleAccessService.DeleteVisibility();
        }
    }
}
