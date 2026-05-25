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
    public class AttractionsViewModel : PagedTableViewModel
    {
        private const string SectionName = "Attractions";

        private ObservableCollection<Attraction> _attractions = null!;
        private ObservableCollection<string> _zones = null!;
        private ObservableCollection<string> _activeFilters = null!;
        private Attraction _selectedAttraction = null!;

        private string _searchText = string.Empty;
        private string _selectedZone = "Все";
        private string _selectedActiveFilter = "Все";
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

        public ObservableCollection<string> Zones
        {
            get => _zones;
            set
            {
                _zones = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> ActiveFilters
        {
            get => _activeFilters;
            set
            {
                _activeFilters = value;
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
                ResetPage();
                LoadAttractions();
            }
        }

        public string SelectedZone
        {
            get => _selectedZone;
            set
            {
                _selectedZone = value;
                OnPropertyChanged();
                ResetPage();
                LoadAttractions();
            }
        }

        public string SelectedActiveFilter
        {
            get => _selectedActiveFilter;
            set
            {
                _selectedActiveFilter = value;
                OnPropertyChanged();
                ResetPage();
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

            LoadFilters();
            SetRoleAccess();
            LoadAttractions();
        }

        private void LoadFilters()
        {
            Zones = new ObservableCollection<string>(
                new[] { "Все" }.Concat(AppData.db.Zones.Select(z => z.ZoneName).ToList())
            );

            ActiveFilters = new ObservableCollection<string>
            {
                "Все",
                "Активные",
                "Неактивные"
            };
        }

        protected override void LoadPage()
        {
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
                string searchText = SearchText.ToLower();

                query = query.Where(a =>
                    a.AttractionName.ToLower().Contains(searchText) ||
                    (a.Description != null && a.Description.ToLower().Contains(searchText)) ||
                    a.Zone.ZoneName.ToLower().Contains(searchText));
            }

            if (!string.IsNullOrWhiteSpace(SelectedZone) && SelectedZone != "Все")
            {
                query = query.Where(a => a.Zone.ZoneName == SelectedZone);
            }

            if (SelectedActiveFilter == "Активные")
            {
                query = query.Where(a => a.IsActive);
            }
            else if (SelectedActiveFilter == "Неактивные")
            {
                query = query.Where(a => !a.IsActive);
            }

            query = ApplyPaging(query.OrderBy(a => a.AttractionId));

            Attractions = new ObservableCollection<Attraction>(query.ToList());
        }

        private void Add(object? parameter)
        {
            NavigationService.Navigate(new AddAttractionPage());
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

            NavigationService.Navigate(new EditAttractionPage(SelectedAttraction.AttractionId));
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
                AuditService.Log("Удаление", "Аттракционы", attraction.AttractionId, attraction.AttractionName);
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
            SelectedZone = "Все";
            SelectedActiveFilter = "Все";
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
