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
        private ObservableCollection<Payment> _payments = null!;
        private Payment _selectedPayment = null!;

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

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand BackCommand { get; }

        public PaymentsViewModel()
        {
            AddCommand = new RelayCommand(Add);
            EditCommand = new RelayCommand(Edit);
            DeleteCommand = new RelayCommand(Delete);
            RefreshCommand = new RelayCommand(Refresh);
            BackCommand = new RelayCommand(Back);

            LoadPayments();
        }

        private void LoadPayments()
        {
            Payments = new ObservableCollection<Payment>(
                AppData.db.Payments
                    .Include(p => p.Sale)
                        .ThenInclude(s => s.Ticket)
                            .ThenInclude(t => t.Client)
                    .AsNoTracking()
                    .ToList()
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

        private void Back(object? parameter)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.OpenPage(new MenuPage());
            }
        }
    }
}