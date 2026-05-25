using AquaPark.Data;
using AquaPark.Models;
using AquaPark.Services;
using AquaPark.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace AquaPark.ViewModel
{
    public class ReportsViewModel : BaseViewModel
    {
        private ObservableCollection<Sale> _sales = null!;
        private ObservableCollection<Payment> _payments = null!;
        private DateTime? _dateFrom = DateTime.Today.AddDays(-30);
        private DateTime? _dateTo = DateTime.Today;
        private int _salesCount;
        private int _ticketsCount;
        private decimal _salesTotalAmount;
        private decimal _paymentsTotalAmount;
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

        public ObservableCollection<Payment> Payments
        {
            get => _payments;
            set
            {
                _payments = value;
                OnPropertyChanged();
            }
        }

        public DateTime? DateFrom
        {
            get => _dateFrom;
            set
            {
                _dateFrom = value;
                OnPropertyChanged();
                LoadReport();
            }
        }

        public DateTime? DateTo
        {
            get => _dateTo;
            set
            {
                _dateTo = value;
                OnPropertyChanged();
                LoadReport();
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

        public int TicketsCount
        {
            get => _ticketsCount;
            set
            {
                _ticketsCount = value;
                OnPropertyChanged();
            }
        }

        public decimal SalesTotalAmount
        {
            get => _salesTotalAmount;
            set
            {
                _salesTotalAmount = value;
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

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        public ICommand RefreshCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand BackCommand { get; }

        public ReportsViewModel()
        {
            RefreshCommand = new RelayCommand(_ => LoadReport());
            ExportCommand = new RelayCommand(ExportReport);
            BackCommand = new RelayCommand(Back);

            LoadReport();
        }

        private void LoadReport()
        {
            if (DateFrom.HasValue && DateTo.HasValue && DateFrom.Value.Date > DateTo.Value.Date)
            {
                ErrorMessage = "Дата начала не может быть позже даты окончания";
                Sales = new ObservableCollection<Sale>();
                Payments = new ObservableCollection<Payment>();
                ClearTotals();
                return;
            }

            ErrorMessage = string.Empty;

            DateTime from = DateFrom?.Date ?? DateTime.MinValue;
            DateTime to = DateTo.HasValue ? DateTo.Value.Date.AddDays(1) : DateTime.MaxValue;

            var salesList = AppData.db.Sales
                .Include(s => s.Ticket)
                    .ThenInclude(t => t.Client)
                .Include(s => s.Employee)
                    .ThenInclude(e => e.User)
                .AsNoTracking()
                .Where(s => s.SaleDate >= from && s.SaleDate < to)
                .OrderByDescending(s => s.SaleDate)
                .ToList();

            var paymentsList = AppData.db.Payments
                .Include(p => p.Sale)
                    .ThenInclude(s => s.Ticket)
                        .ThenInclude(t => t.Client)
                .AsNoTracking()
                .Where(p => p.PaymentDate >= from && p.PaymentDate < to)
                .OrderByDescending(p => p.PaymentDate)
                .ToList();

            Sales = new ObservableCollection<Sale>(salesList);
            Payments = new ObservableCollection<Payment>(paymentsList);

            SalesCount = salesList.Count;
            TicketsCount = salesList.Select(s => s.TicketId).Distinct().Count();
            SalesTotalAmount = salesList.Sum(s => s.TotalAmount);
            PaymentsTotalAmount = paymentsList.Sum(p => p.Amount);
        }

        private void ExportReport(object? parameter)
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "CSV файл (*.csv)|*.csv",
                FileName = $"AquaPark_Report_{DateTime.Now:yyyyMMdd_HHmm}.csv"
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            try
            {
                StringBuilder builder = new StringBuilder();

                builder.AppendLine("Отчет AquaPark");
                builder.AppendLine($"Период;{DateFrom:dd.MM.yyyy};{DateTo:dd.MM.yyyy}");
                builder.AppendLine($"Количество продаж;{SalesCount}");
                builder.AppendLine($"Количество билетов;{TicketsCount}");
                builder.AppendLine($"Сумма продаж;{SalesTotalAmount:N2}");
                builder.AppendLine($"Сумма оплат;{PaymentsTotalAmount:N2}");
                builder.AppendLine();
                builder.AppendLine("Продажи");
                builder.AppendLine("ID;Дата;Билет;Клиент;Сотрудник;Сумма");

                foreach (Sale sale in Sales)
                {
                    builder.AppendLine($"{sale.SaleId};{sale.SaleDate:dd.MM.yyyy HH:mm};{sale.TicketId};{Escape(sale.Ticket.Client?.FullName)};{Escape(sale.Employee.User.FullName)};{sale.TotalAmount:N2}");
                }

                builder.AppendLine();
                builder.AppendLine("Оплаты");
                builder.AppendLine("ID;Дата;Продажа;Клиент;Способ;Статус;Сумма");

                foreach (Payment payment in Payments)
                {
                    builder.AppendLine($"{payment.PaymentId};{payment.PaymentDate:dd.MM.yyyy HH:mm};{payment.SaleId};{Escape(payment.Sale.Ticket.Client?.FullName)};{Escape(payment.PaymentMethod)};{Escape(payment.PaymentStatus)};{payment.Amount:N2}");
                }

                File.WriteAllText(dialog.FileName, builder.ToString(), new UTF8Encoding(true));

                MessageBox.Show("Отчет успешно экспортирован",
                                "Экспорт",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("Не удалось экспортировать отчет",
                                "Экспорт",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private static string Escape(string? value)
        {
            return (value ?? string.Empty).Replace(";", ",");
        }

        private void ClearTotals()
        {
            SalesCount = 0;
            TicketsCount = 0;
            SalesTotalAmount = 0;
            PaymentsTotalAmount = 0;
        }

        private void Back(object? parameter)
        {
            NavigationService.Navigate(new MenuPage());
        }
    }
}
