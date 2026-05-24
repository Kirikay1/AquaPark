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
    public class SalesViewModel : BaseViewModel
    {
        private const string SectionName = "Sales";

        private ObservableCollection<Sale> _sales = null!;
        private Sale _selectedSale = null!;

        private string _searchText = string.Empty;
        private Visibility _addButtonVisibility = Visibility.Visible;
        private Visibility _editButtonVisibility = Visibility.Visible;
        private Visibility _deleteButtonVisibility = Visibility.Visible;

        public ObservableCollection<Sale> Sales
        {
            get => _sales;
            set
            {
                _sales = value;
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
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                LoadSales();
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

        public SalesViewModel()
        {
            AddCommand = new RelayCommand(Add, _ => RoleAccessService.CanAddOrEdit(SectionName));
            EditCommand = new RelayCommand(Edit, _ => RoleAccessService.CanAddOrEdit(SectionName));
            DeleteCommand = new RelayCommand(Delete, _ => RoleAccessService.CanDelete());
            RefreshCommand = new RelayCommand(Refresh);
            BackCommand = new RelayCommand(Back);
            ClearSearchCommand = new RelayCommand(ClearSearch);

            SetRoleAccess();
            LoadSales();
        }

        private void LoadSales()
        {
            var query = AppData.db.Sales
                .Include(s => s.Ticket)
                    .ThenInclude(t => t.Client)
                .Include(s => s.Employee)
                    .ThenInclude(e => e.User)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                query = query.Where(s =>
                    s.Ticket.TicketId.ToString().Contains(SearchText) ||
                    s.Ticket.Client.FullName.Contains(SearchText) ||
                    s.Employee.User.FullName.Contains(SearchText) ||
                    s.TotalAmount.ToString().Contains(SearchText));
            }

            Sales = new ObservableCollection<Sale>(
                query.ToList()
            );
        }

        private void Add(object? parameter)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.OpenPage(new AddSalePage());
            }
        }

        private void Edit(object? parameter)
        {
            if (SelectedSale == null)
            {
                MessageBox.Show("Выберите продажу для изменения",
                                "Изменение",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.OpenPage(new EditSalePage(SelectedSale.SaleId));
            }
        }

        private void Delete(object? parameter)
        {
            if (SelectedSale == null)
            {
                MessageBox.Show("Выберите продажу для удаления",
                                "Удаление",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить выбранную продажу?",
                                                      "Подтверждение удаления",
                                                      MessageBoxButton.YesNo,
                                                      MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            var sale = AppData.db.Sales.FirstOrDefault(s => s.SaleId == SelectedSale.SaleId);

            if (sale == null)
            {
                MessageBox.Show("Продажа не найдена",
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            try
            {
                AppData.db.Sales.Remove(sale);
                AppData.db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                AppData.db.Entry(sale).State = EntityState.Unchanged;

                MessageBox.Show("Нельзя удалить продажу, так как по ней есть оплаты.",
                                "Удаление",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            LoadSales();

            MessageBox.Show("Продажа успешно удалена",
                            "Удаление",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
        }

        private void Refresh(object? parameter)
        {
            LoadSales();
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
