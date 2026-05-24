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
    public class AttractionsViewModel : BaseViewModel
    {
        private ObservableCollection<Attraction> _attractions = null!;
        private Attraction _selectedAttraction = null!;

        private string _searchText = string.Empty;

        public ObservableCollection<Attraction> Attractions
        {
            get => _attractions;
            set
            {
                _attractions = value;
                OnPropertyChanged();
            }
        }

        public Attraction SelectedAttraction
        {
            get => _selectedAttraction;
            set
            {
                _selectedAttraction = value;
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
                LoadAttractions();
            }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand BackCommand { get; }

        public AttractionsViewModel()
        {
            AddCommand = new RelayCommand(Add);
            EditCommand = new RelayCommand(Edit);
            DeleteCommand = new RelayCommand(Delete);
            RefreshCommand = new RelayCommand(Refresh);
            BackCommand = new RelayCommand(Back);

            LoadAttractions();
        }

        private void LoadAttractions()
        {
            var query = AppData.db.Attractions
                .Include(a => a.Zone)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                query = query.Where(a =>
                    a.AttractionName.Contains(SearchText) ||
                    (a.Description != null && a.Description.Contains(SearchText)) ||
                    a.Zone.ZoneName.Contains(SearchText));
            }

            Attractions = new ObservableCollection<Attraction>(
                query.ToList()
            );
        }

        private void Add(object? parameter)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.OpenPage(new AddAttractionPage());
            }
        }

        private void Edit(object? parameter)
        {
            if (SelectedAttraction == null)
            {
                MessageBox.Show("Выберите аттракцион для изменения",
                                "Изменение",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.OpenPage(new EditAttractionPage(SelectedAttraction.AttractionId));
            }
        }

        private void Delete(object? parameter)
        {
            if (SelectedAttraction == null)
            {
                MessageBox.Show("Выберите аттракцион для удаления",
                                "Удаление",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить выбранный аттракцион?",
                                                      "Подтверждение удаления",
                                                      MessageBoxButton.YesNo,
                                                      MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            var attraction = AppData.db.Attractions
                .FirstOrDefault(a => a.AttractionId == SelectedAttraction.AttractionId);

            if (attraction == null)
            {
                MessageBox.Show("Аттракцион не найден",
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            AppData.db.Attractions.Remove(attraction);
            AppData.db.SaveChanges();

            LoadAttractions();

            MessageBox.Show("Аттракцион успешно удален",
                            "Удаление",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
        }

        private void Refresh(object? parameter)
        {
            LoadAttractions();
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