using AquaPark.Data;
using AquaPark.Models;
using AquaPark.Services;
using AquaPark.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace AquaPark.ViewModel
{
    public class EditTicketViewModel : BaseViewModel
    {
        private readonly int _ticketId;

        private ObservableCollection<TicketType> _ticketTypes = null!;
        private ObservableCollection<Client> _clients = null!;
        private ObservableCollection<string> _statuses = null!;

        private TicketType _selectedTicketType = null!;
        private Client _selectedClient = null!;
        private DateTime? _visitDate;
        private string _selectedStatus = string.Empty;
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

        public ObservableCollection<string> Statuses
        {
            get => _statuses;
            set
            {
                _statuses = value;
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

        public string SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                _selectedStatus = value;
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

        public EditTicketViewModel(int ticketId)
        {
            _ticketId = ticketId;

            SaveCommand = new RelayCommand(Save, _ => RoleAccessService.CanAddOrEdit("Tickets"));
            BackCommand = new RelayCommand(Back);

            LoadData();
            LoadTicket();
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

            Statuses = new ObservableCollection<string>
            {
                "Активен",
                "Использован",
                "Отменен"
            };
        }

        private void LoadTicket()
        {
            var ticket = AppData.db.Tickets
                .FirstOrDefault(t => t.TicketId == _ticketId);

            if (ticket == null)
            {
                ErrorMessage = "Билет не найден";
                return;
            }

            SelectedTicketType = TicketTypes.FirstOrDefault(t => t.TicketTypeId == ticket.TicketTypeId)!;
            SelectedClient = Clients.FirstOrDefault(c => c.ClientId == ticket.ClientId)!;
            VisitDate = ticket.VisitDate.ToDateTime(TimeOnly.MinValue);
            SelectedStatus = ticket.Status;
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

            if (string.IsNullOrWhiteSpace(SelectedStatus))
            {
                ErrorMessage = "Выберите статус билета";
                return;
            }

            var ticket = AppData.db.Tickets
                .FirstOrDefault(t => t.TicketId == _ticketId);

            if (ticket == null)
            {
                ErrorMessage = "Билет не найден";
                return;
            }

            ticket.TicketTypeId = SelectedTicketType.TicketTypeId;
            ticket.ClientId = SelectedClient.ClientId;
            ticket.VisitDate = DateOnly.FromDateTime(visitDate);
            ticket.Status = SelectedStatus;

            if (!DatabaseErrorService.TrySaveChanges("Данные билета успешно изменены"))
            {
                return;
            }

            StatusAutomationService.UpdateTicketStatuses();
            AuditService.Log("Изменение", "Билеты", ticket.TicketId, ticket.Status);
            MarkAsSaved();

            Back(null);
        }

        private void Back(object? parameter)
        {
            NavigationService.Navigate(new TicketsPage());
        }
    }
}
