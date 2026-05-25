using AquaPark.Data;
using AquaPark.Models;
using AquaPark.Services;
using AquaPark.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AquaPark.ViewModel
{
    public class AddTicketViewModel : BaseViewModel
    {
        private ObservableCollection<TicketType> _ticketTypes = null!;
        private ObservableCollection<Client> _clients = null!;

        private TicketType _selectedTicketType = null!;
        private Client _selectedClient = null!;
        private DateTime? _visitDate;
        private object _status = null!;
        private string _errorMessage = string.Empty;

        public ObservableCollection<TicketType> TicketTypes
        {
            get => _ticketTypes;
            set
            {
                _ticketTypes = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Client> Clients
        {
            get => _clients;
            set
            {
                _clients = value;
                OnPropertyChanged();
            }
        }

        public TicketType SelectedTicketType
        {
            get => _selectedTicketType;
            set
            {
                _selectedTicketType = value;
                OnPropertyChanged();
            }
        }

        public Client SelectedClient
        {
            get => _selectedClient;
            set
            {
                _selectedClient = value;
                OnPropertyChanged();
            }
        }

        public DateTime? VisitDate
        {
            get => _visitDate;
            set
            {
                _visitDate = value;
                OnPropertyChanged();
            }
        }

        public object Status
        {
            get => _status;
            set
            {
                _status = value;
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

        public AddTicketViewModel()
        {
            SaveCommand = new RelayCommand(Save, _ => RoleAccessService.CanAddOrEdit("Tickets"));
            BackCommand = new RelayCommand(Back);

            LoadData();
            VisitDate = DateTime.Today;
            EnableUnsavedChangesTracking();
        }

        private void LoadData()
        {
            TicketTypes = new ObservableCollection<TicketType>(
                AppData.db.TicketTypes.ToList()
            );

            Clients = new ObservableCollection<Client>(
                AppData.db.Clients.ToList()
            );
        }

        private void Save(object? parameter)
        {
            if (SelectedTicketType == null)
            {
                ErrorMessage = "Выберите тип билета";
                return;
            }

            if (SelectedClient == null)
            {
                ErrorMessage = "Выберите клиента";
                return;
            }

            if (!ValidationService.ValidateVisitDate(VisitDate, out string errorMessage))
            {
                ErrorMessage = errorMessage;
                return;
            }

            DateTime visitDate = VisitDate.GetValueOrDefault();

            string statusText = "Активен";

            if (Status is ComboBoxItem item && item.Content != null)
            {
                statusText = item.Content.ToString()!;
            }

            Ticket ticket = new Ticket
            {
                TicketTypeId = SelectedTicketType.TicketTypeId,
                ClientId = SelectedClient.ClientId,
                PurchaseDate = DateTime.Now,
                VisitDate = DateOnly.FromDateTime(visitDate),
                Status = statusText
            };

            AppData.db.Tickets.Add(ticket);
            if (!DatabaseErrorService.TrySaveChanges("Билет успешно добавлен"))
            {
                return;
            }

            AuditService.Log("Добавление", "Билеты", ticket.TicketId, ticket.Status);
            MarkAsSaved();

            Back(null);
        }

        private void Back(object? parameter)
        {
            NavigationService.Navigate(new TicketsPage());
        }
    }
}
