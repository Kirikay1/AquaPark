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
    public class EditSaleViewModel : BaseViewModel
    {
        private readonly int _saleId;

        private ObservableCollection<Ticket> _tickets = null!;
        private ObservableCollection<Employee> _employees = null!;

        private Ticket _selectedTicket = null!;
        private Employee _selectedEmployee = null!;
        private decimal _totalAmount;
        private string _errorMessage = string.Empty;

        public ObservableCollection<Ticket> Tickets
        {
            get => _tickets;
            set
            {
                _tickets = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Employee> Employees
        {
            get => _employees;
            set
            {
                _employees = value;
                OnPropertyChanged();
            }
        }

        public Ticket SelectedTicket
        {
            get => _selectedTicket;
            set
            {
                _selectedTicket = value;
                OnPropertyChanged();
            }
        }

        public Employee SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                _selectedEmployee = value;
                OnPropertyChanged();
            }
        }

        public decimal TotalAmount
        {
            get => _totalAmount;
            set
            {
                _totalAmount = value;
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

        public EditSaleViewModel(int saleId)
        {
            _saleId = saleId;

            SaveCommand = new RelayCommand(Save);
            BackCommand = new RelayCommand(Back);

            LoadData();
            LoadSale();
        }

        private void LoadData()
        {
            Tickets = new ObservableCollection<Ticket>(
                AppData.db.Tickets
                    .Include(t => t.Client)
                    .Include(t => t.TicketType)
                    .ToList()
            );

            Employees = new ObservableCollection<Employee>(
                AppData.db.Employees
                    .Include(e => e.User)
                    .ToList()
            );
        }

        private void LoadSale()
        {
            var sale = AppData.db.Sales
                .FirstOrDefault(s => s.SaleId == _saleId);

            if (sale == null)
            {
                ErrorMessage = "Продажа не найдена";
                return;
            }

            SelectedTicket = Tickets.FirstOrDefault(t => t.TicketId == sale.TicketId)!;
            SelectedEmployee = Employees.FirstOrDefault(e => e.EmployeeId == sale.EmployeeId)!;
            TotalAmount = sale.TotalAmount;
        }

        private void Save(object? parameter)
        {
            if (SelectedTicket == null)
            {
                ErrorMessage = "Выберите билет";
                return;
            }

            if (SelectedEmployee == null)
            {
                ErrorMessage = "Выберите сотрудника";
                return;
            }

            if (TotalAmount <= 0)
            {
                ErrorMessage = "Введите корректную сумму";
                return;
            }

            var sale = AppData.db.Sales
                .FirstOrDefault(s => s.SaleId == _saleId);

            if (sale == null)
            {
                ErrorMessage = "Продажа не найдена";
                return;
            }

            sale.TicketId = SelectedTicket.TicketId;
            sale.EmployeeId = SelectedEmployee.EmployeeId;
            sale.TotalAmount = TotalAmount;

            AppData.db.SaveChanges();

            MessageBox.Show("Данные продажи успешно изменены",
                            "Сохранение",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

            Back(null);
        }

        private void Back(object? parameter)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.OpenPage(new SalesPage());
            }
        }
    }
}