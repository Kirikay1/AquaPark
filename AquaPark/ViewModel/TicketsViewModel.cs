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
    public class TicketsViewModel : PagedTableViewModel
    {
        private const string SectionName = "Tickets";

        private ObservableCollection<Ticket> _tickets = null!;
        private ObservableCollection<string> _statuses = null!;
        private Ticket _selectedTicket = null!;

        private string _searchText = string.Empty;
        private string _selectedStatus = "Все";
        private Visibility _addButtonVisibility = Visibility.Visible;
        private Visibility _editButtonVisibility = Visibility.Visible;
        private Visibility _deleteButtonVisibility = Visibility.Visible;

        public ObservableCollection<Ticket> Tickets
        {
            get => _tickets;
            set
            {
                _tickets = value;
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

        public Ticket SelectedTicket
        {
            get => _selectedTicket;
            set
            {
                _selectedTicket = value;
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
                LoadTickets();
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
                LoadTickets();
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

        public TicketsViewModel()
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
                "Активен",
                "Истек",
                "Использован",
                "Отменен"
            };

            SetRoleAccess();
            StatusAutomationService.UpdateTicketStatuses();
            LoadTickets();
        }

        protected override void LoadPage()
        {
            LoadTickets();
        }

        private void LoadTickets()
        {
            var query = AppData.db.Tickets
                .Include(t => t.TicketType)
                .Include(t => t.Client)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                string searchText = SearchText.ToLower();

                query = query.Where(t =>
                    t.Status.ToLower().Contains(searchText) ||
                    t.TicketType.TicketName.ToLower().Contains(searchText) ||
                    (t.Client != null && t.Client.FullName.ToLower().Contains(searchText)));
            }

            if (!string.IsNullOrWhiteSpace(SelectedStatus) && SelectedStatus != "Все")
            {
                query = query.Where(t => t.Status == SelectedStatus);
            }

            query = ApplyPaging(query.OrderByDescending(t => t.PurchaseDate));

            Tickets = new ObservableCollection<Ticket>(query.ToList());
        }

        private void Add(object? parameter)
        {
            NavigationService.Navigate(new AddTicketPage());
        }

        private void Edit(object? parameter)
        {
            if (SelectedTicket == null)
            {
                MessageBox.Show("Выберите билет для изменения",
                                "Изменение",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            NavigationService.Navigate(new EditTicketPage(SelectedTicket.TicketId));
        }

        private void Delete(object? parameter)
        {
            if (SelectedTicket == null)
            {
                MessageBox.Show("Выберите билет для удаления",
                                "Удаление",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить выбранный билет?",
                                                      "Подтверждение удаления",
                                                      MessageBoxButton.YesNo,
                                                      MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            var ticket = AppData.db.Tickets
                .FirstOrDefault(t => t.TicketId == SelectedTicket.TicketId);

            if (ticket == null)
            {
                MessageBox.Show("Билет не найден",
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            try
            {
                AppData.db.Tickets.Remove(ticket);
                AppData.db.SaveChanges();
                AuditService.Log("Удаление", "Билеты", ticket.TicketId, ticket.Status);
            }
            catch (DbUpdateException)
            {
                AppData.db.Entry(ticket).State = EntityState.Unchanged;

                MessageBox.Show("Нельзя удалить билет, так как он связан с продажами или посещениями.",
                                "Удаление",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            LoadTickets();

            MessageBox.Show("Билет успешно удален",
                            "Удаление",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
        }

        private void Refresh(object? parameter)
        {
            LoadTickets();
        }

        private void ClearSearch(object? parameter)
        {
            SearchText = string.Empty;
            SelectedStatus = "Все";
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
