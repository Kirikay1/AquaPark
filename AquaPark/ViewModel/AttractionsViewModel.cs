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
        private const string SectionName = "Attractions";

        private ObservableCollection<Attraction> _attractions = null!;
        private Attraction _selectedAttraction = null!;

        private string _searchText = string.Empty;
        private Visibility _addButtonVisibility = Visibility.Visible;
        private Visibility _editButtonVisibility = Visibility.Visible;
        private Visibility _deleteButtonVisibility = Visibility.Visible;

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

        public AttractionsViewModel()
        {
            AddCommand = new RelayCommand(Add, _ => RoleAccessService.CanAddOrEdit(SectionName));
            EditCommand = new RelayCommand(Edit, _ => RoleAccessService.CanAddOrEdit(SectionName));
            DeleteCommand = new RelayCommand(Delete, _ => RoleAccessService.CanDelete());
            RefreshCommand = new RelayCommand(Refresh);
            BackCommand = new RelayCommand(Back);
            ClearSearchCommand = new RelayCommand(ClearSearch);

            SetRoleAccess();
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

            try
            {
                AppData.db.Attractions.Remove(attraction);
                AppData.db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                AppData.db.Entry(attraction).State = EntityState.Unchanged;

                MessageBox.Show("Нельзя удалить аттракцион, так как у него есть расписание.",
                                "Удаление",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

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
