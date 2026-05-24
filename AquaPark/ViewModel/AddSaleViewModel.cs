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
    public class AddSaleViewModel : BaseViewModel
    {
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

                if (_selectedTicket != null && _selectedTicket.TicketType != null)
                {
                    TotalAmount = _selectedTicket.TicketType.Price;
                }
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

        public AddSaleViewModel()
        {
            SaveCommand = new RelayCommand(Save, _ => RoleAccessService.CanAddOrEdit("Sales"));
            BackCommand = new RelayCommand(Back);

            LoadData();
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

            if (!ValidationService.ValidatePositiveAmount(TotalAmount, out string errorMessage))
            {
                ErrorMessage = errorMessage;
                return;
            }

            Sale sale = new Sale
            {
                TicketId = SelectedTicket.TicketId,
                EmployeeId = SelectedEmployee.EmployeeId,
                SaleDate = DateTime.Now,
                TotalAmount = TotalAmount
            };

            AppData.db.Sales.Add(sale);
            AppData.db.SaveChanges();

            MessageBox.Show("Продажа успешно добавлена",
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
