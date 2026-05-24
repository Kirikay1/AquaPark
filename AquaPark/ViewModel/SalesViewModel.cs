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
        private ObservableCollection<Sale> _sales = null!;
        private Sale _selectedSale = null!;

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

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand BackCommand { get; }

        public SalesViewModel()
        {
            AddCommand = new RelayCommand(Add);
            EditCommand = new RelayCommand(Edit);
            DeleteCommand = new RelayCommand(Delete);
            RefreshCommand = new RelayCommand(Refresh);
            BackCommand = new RelayCommand(Back);

            LoadSales();
        }

        private void LoadSales()
        {
            Sales = new ObservableCollection<Sale>(
                AppData.db.Sales
                    .Include(s => s.Ticket)
                        .ThenInclude(t => t.Client)
                    .Include(s => s.Employee)
                        .ThenInclude(e => e.User)
                    .AsNoTracking()
                    .ToList()
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

            AppData.db.Sales.Remove(sale);
            AppData.db.SaveChanges();

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

        private void Back(object? parameter)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.OpenPage(new MenuPage());
            }
        }
    }
}