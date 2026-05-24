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
    public class ClientsViewModel : BaseViewModel
    {
        private const string SectionName = "Clients";

        private ObservableCollection<Client> _clients;
        private Client _selectedClient;
        private string _searchText = string.Empty;
        private Visibility _addButtonVisibility = Visibility.Visible;
        private Visibility _editButtonVisibility = Visibility.Visible;
        private Visibility _deleteButtonVisibility = Visibility.Visible;

        public ObservableCollection<Client> Clients
        {
            get => _clients;
            set
            {
                _clients = value;
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

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                LoadClients();
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

        public ICommand RefreshCommand { get; }
        public ICommand BackCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand ClearSearchCommand { get; }

        public ClientsViewModel()
        {
            RefreshCommand = new RelayCommand(Refresh);
            BackCommand = new RelayCommand(Back);
            AddCommand = new RelayCommand(Add, _ => RoleAccessService.CanAddOrEdit(SectionName));
            DeleteCommand = new RelayCommand(Delete, _ => RoleAccessService.CanDelete());
            EditCommand = new RelayCommand(Edit, _ => RoleAccessService.CanAddOrEdit(SectionName));
            ClearSearchCommand = new RelayCommand(ClearSearch);

            SetRoleAccess();
            LoadClients();
        }

        private void LoadClients()
        {
            var query = AppData.db.Clients
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                query = query.Where(c =>
                    c.FullName.Contains(SearchText) ||
                    (c.Phone != null && c.Phone.Contains(SearchText)) ||
                    (c.Email != null && c.Email.Contains(SearchText)));
            }

            Clients = new ObservableCollection<Client>(
                query.ToList()
            );
        }

        private void Refresh(object? parameter)
        {
            LoadClients();
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

        private void Add(object? parameter)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.OpenPage(new AddClientPage());
            }
        }

        private void Delete(object? parameter)
        {
            if (SelectedClient == null)
            {
                MessageBox.Show("Выберите клиента для удаления",
                                "Удаление",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить выбранного клиента?",
                                                      "Подтверждение удаления",
                                                      MessageBoxButton.YesNo,
                                                      MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            var client = AppData.db.Clients
                .FirstOrDefault(c => c.ClientId == SelectedClient.ClientId);

            if (client == null)
            {
                MessageBox.Show("Клиент не найден",
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            try
            {
                AppData.db.Clients.Remove(client);
                AppData.db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                AppData.db.Entry(client).State = EntityState.Unchanged;

                MessageBox.Show("Нельзя удалить клиента, так как у него есть связанные билеты.",
                                "Удаление",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            LoadClients();

            MessageBox.Show("Клиент успешно удален",
                            "Удаление",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
        }

        private void Edit(object? parameter)
        {
            if (SelectedClient == null)
            {
                MessageBox.Show("Выберите клиента для изменения",
                                "Изменение",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.OpenPage(new EditClientPage(SelectedClient.ClientId));
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
