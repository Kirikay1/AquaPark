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
    public class AttractionSchedulesViewModel : PagedTableViewModel
    {
        private const string SectionName = "Schedules";

        private ObservableCollection<AttractionSchedule> _schedules = null!;
        private ObservableCollection<string> _statuses = null!;
        private AttractionSchedule _selectedSchedule = null!;
        private DateTime? _filterDate;
        private string _selectedStatus = "Все";
        private Visibility _addButtonVisibility = Visibility.Visible;
        private Visibility _editButtonVisibility = Visibility.Visible;
        private Visibility _deleteButtonVisibility = Visibility.Visible;

        public ObservableCollection<AttractionSchedule> Schedules
        {
            get => _schedules;
            set
            {
                _schedules = value;
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

        public AttractionSchedule SelectedSchedule
        {
            get => _selectedSchedule;
            set
            {
                _selectedSchedule = value;
                OnPropertyChanged();
            }
        }

        public DateTime? FilterDate
        {
            get => _filterDate;
            set
            {
                _filterDate = value;
                OnPropertyChanged();
                ResetPage();
                LoadSchedules();
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
                LoadSchedules();
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
        public ICommand ClearFiltersCommand { get; }
        public ICommand BackCommand { get; }

        public AttractionSchedulesViewModel()
        {
            AddCommand = new RelayCommand(Add, _ => RoleAccessService.CanAddOrEdit(SectionName));
            EditCommand = new RelayCommand(Edit, _ => RoleAccessService.CanAddOrEdit(SectionName));
            DeleteCommand = new RelayCommand(Delete, _ => RoleAccessService.CanDelete());
            RefreshCommand = new RelayCommand(_ => LoadSchedules());
            ClearFiltersCommand = new RelayCommand(ClearFilters);
            BackCommand = new RelayCommand(Back);

            Statuses = new ObservableCollection<string>
            {
                "Все",
                "Работает",
                "Ремонт",
                "Закрыт"
            };

            SetRoleAccess();
            LoadSchedules();
        }

        protected override void LoadPage()
        {
            LoadSchedules();
        }

        private void LoadSchedules()
        {
            var query = AppData.db.AttractionSchedules
                .Include(s => s.Attraction)
                    .ThenInclude(a => a.Zone)
                .AsNoTracking()
                .AsQueryable();

            if (FilterDate.HasValue)
            {
                DateOnly date = DateOnly.FromDateTime(FilterDate.Value);
                query = query.Where(s => s.WorkDate == date);
            }

            if (!string.IsNullOrWhiteSpace(SelectedStatus) && SelectedStatus != "Все")
            {
                query = query.Where(s => s.Status == SelectedStatus);
            }

            query = ApplyPaging(query.OrderBy(s => s.WorkDate).ThenBy(s => s.StartTime));

            Schedules = new ObservableCollection<AttractionSchedule>(query.ToList());
        }

        private void Add(object? parameter)
        {
            NavigationService.Navigate(new AddAttractionSchedulePage());
        }

        private void Edit(object? parameter)
        {
            if (SelectedSchedule == null)
            {
                MessageBox.Show("Выберите запись расписания для изменения",
                                "Изменение",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            NavigationService.Navigate(new EditAttractionSchedulePage(SelectedSchedule.ScheduleId));
        }

        private void Delete(object? parameter)
        {
            if (SelectedSchedule == null)
            {
                MessageBox.Show("Выберите запись расписания для удаления",
                                "Удаление",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить выбранную запись расписания?",
                                                      "Подтверждение удаления",
                                                      MessageBoxButton.YesNo,
                                                      MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            var schedule = AppData.db.AttractionSchedules
                .FirstOrDefault(s => s.ScheduleId == SelectedSchedule.ScheduleId);

            if (schedule == null)
            {
                MessageBox.Show("Запись расписания не найдена",
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            AppData.db.AttractionSchedules.Remove(schedule);
            if (!DatabaseErrorService.TrySaveChanges("Запись расписания успешно удалена"))
            {
                return;
            }

            LoadSchedules();
        }

        private void ClearFilters(object? parameter)
        {
            FilterDate = null;
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
