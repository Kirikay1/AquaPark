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
    public class AddAttractionScheduleViewModel : BaseViewModel
    {
        private ObservableCollection<Attraction> _attractions = null!;
        private ObservableCollection<string> _statuses = null!;
        private Attraction _selectedAttraction = null!;
        private DateTime? _workDate = DateTime.Today;
        private string _startTimeText = "09:00";
        private string _endTimeText = "18:00";
        private string _selectedStatus = "Работает";
        private string _errorMessage = string.Empty;

        public ObservableCollection<Attraction> Attractions
        {
            get => _attractions;
            set
            {
                _attractions = value;
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

        public Attraction SelectedAttraction
        {
            get => _selectedAttraction;
            set
            {
                _selectedAttraction = value;
                OnPropertyChanged();
            }
        }

        public DateTime? WorkDate
        {
            get => _workDate;
            set
            {
                _workDate = value;
                OnPropertyChanged();
            }
        }

        public string StartTimeText
        {
            get => _startTimeText;
            set
            {
                _startTimeText = value;
                OnPropertyChanged();
            }
        }

        public string EndTimeText
        {
            get => _endTimeText;
            set
            {
                _endTimeText = value;
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

        public AddAttractionScheduleViewModel()
        {
            SaveCommand = new RelayCommand(Save, _ => RoleAccessService.CanAddOrEdit("Schedules"));
            BackCommand = new RelayCommand(Back);

            LoadData();
            EnableUnsavedChangesTracking();
        }

        private void LoadData()
        {
            Attractions = new ObservableCollection<Attraction>(
                AppData.db.Attractions
                    .Include(a => a.Zone)
                    .Where(a => a.IsActive)
                    .ToList()
            );

            Statuses = new ObservableCollection<string>
            {
                "Работает",
                "Ремонт",
                "Закрыт"
            };
        }

        private void Save(object? parameter)
        {
            if (SelectedAttraction == null)
            {
                ErrorMessage = "Выберите аттракцион";
                return;
            }

            if (!ValidationService.ValidateSchedule(WorkDate, StartTimeText, EndTimeText, out string errorMessage))
            {
                ErrorMessage = errorMessage;
                return;
            }

            AttractionSchedule schedule = new AttractionSchedule
            {
                AttractionId = SelectedAttraction.AttractionId,
                WorkDate = DateOnly.FromDateTime(WorkDate.GetValueOrDefault()),
                StartTime = TimeOnly.Parse(StartTimeText),
                EndTime = TimeOnly.Parse(EndTimeText),
                Status = SelectedStatus
            };

            AppData.db.AttractionSchedules.Add(schedule);
            if (!DatabaseErrorService.TrySaveChanges("Запись расписания успешно добавлена"))
            {
                return;
            }

            AuditService.Log("Добавление", "Расписание", schedule.ScheduleId, schedule.Status);
            MarkAsSaved();

            Back(null);
        }

        private void Back(object? parameter)
        {
            NavigationService.Navigate(new AttractionSchedulesPage());
        }
    }
}
