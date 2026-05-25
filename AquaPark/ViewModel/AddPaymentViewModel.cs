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
    public class AddPaymentViewModel : BaseViewModel
    {
        private ObservableCollection<Sale> _sales = null!;
        private ObservableCollection<string> _paymentMethods = null!;
        private ObservableCollection<string> _paymentStatuses = null!;

        private Sale _selectedSale = null!;
        private decimal _amount;
        private string _selectedPaymentMethod = string.Empty;
        private string _selectedPaymentStatus = string.Empty;
        private string _errorMessage = string.Empty;

        public ObservableCollection<Sale> Sales
        {
            get => _sales;
            set
            {
                _sales = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> PaymentMethods
        {
            get => _paymentMethods;
            set
            {
                _paymentMethods = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> PaymentStatuses
        {
            get => _paymentStatuses;
            set
            {
                _paymentStatuses = value;
                OnPropertyChanged();
            }
        }

        public Sale SelectedSale
        {
            get => _selectedSale;
            set
            {
                _selectedSale = value;
                OnPropertyChanged();

                if (_selectedSale != null)
                {
                    Amount = _selectedSale.TotalAmount;
                }
            }
        }

        public decimal Amount
        {
            get => _amount;
            set
            {
                _amount = value;
                OnPropertyChanged();
            }
        }

        public string SelectedPaymentMethod
        {
            get => _selectedPaymentMethod;
            set
            {
                _selectedPaymentMethod = value;
                OnPropertyChanged();
            }
        }

        public string SelectedPaymentStatus
        {
            get => _selectedPaymentStatus;
            set
            {
                _selectedPaymentStatus = value;
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

        public ICommand SaveCommand { get; }
        public ICommand BackCommand { get; }

        public AddPaymentViewModel()
        {
            SaveCommand = new RelayCommand(Save, _ => RoleAccessService.CanAddOrEdit("Payments"));
            BackCommand = new RelayCommand(Back);

            LoadData();
            EnableUnsavedChangesTracking();
        }

        private void LoadData()
        {
            Sales = new ObservableCollection<Sale>(
                AppData.db.Sales
                    .Include(s => s.Ticket)
                        .ThenInclude(t => t.Client)
                    .ToList()
            );

            PaymentMethods = new ObservableCollection<string>
            {
                "Наличные",
                "Банковская карта",
                "Онлайн-оплата"
            };

            PaymentStatuses = new ObservableCollection<string>
            {
                "Оплачено",
                "Ожидает оплаты",
                "Отменено"
            };

            SelectedPaymentMethod = "Наличные";
            SelectedPaymentStatus = "Оплачено";
        }

        private void Save(object? parameter)
        {
            if (SelectedSale == null)
            {
                ErrorMessage = "Выберите продажу";
                return;
            }

            if (!ValidationService.ValidatePositiveAmount(Amount, out string errorMessage))
            {
                ErrorMessage = errorMessage;
                return;
            }

            if (DuplicateCheckService.SaleIsFullyPaid(SelectedSale.SaleId))
            {
                ErrorMessage = "Выбранная продажа уже полностью оплачена";
                return;
            }

            if (DuplicateCheckService.PaymentExceedsSaleAmount(SelectedSale.SaleId, Amount))
            {
                ErrorMessage = "Сумма оплат не должна превышать сумму продажи";
                return;
            }

            if (string.IsNullOrWhiteSpace(SelectedPaymentMethod))
            {
                ErrorMessage = "Выберите способ оплаты";
                return;
            }

            if (string.IsNullOrWhiteSpace(SelectedPaymentStatus))
            {
                ErrorMessage = "Выберите статус оплаты";
                return;
            }

            Payment payment = new Payment
            {
                SaleId = SelectedSale.SaleId,
                PaymentDate = DateTime.Now,
                Amount = Amount,
                PaymentMethod = SelectedPaymentMethod,
                PaymentStatus = SelectedPaymentStatus == "Отменено"
                    ? "Отменено"
                    : StatusAutomationService.GetSalePaymentStatus(SelectedSale.SaleId, SelectedSale.TotalAmount, null, Amount)
            };

            AppData.db.Payments.Add(payment);
            if (!DatabaseErrorService.TrySaveChanges("Оплата успешно добавлена"))
            {
                return;
            }

            StatusAutomationService.UpdatePaymentStatuses();
            AuditService.Log("Добавление", "Оплаты", payment.PaymentId, $"Сумма: {payment.Amount:N2}");
            MarkAsSaved();

            Back(null);
        }

        private void Back(object? parameter)
        {
            NavigationService.Navigate(new PaymentsPage());
        }
    }
}
